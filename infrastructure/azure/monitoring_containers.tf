### Enable container insights for AKS ###
locals {
  streams = [
    "Microsoft-ContainerLog",
    "Microsoft-ContainerLogV2",
    "Microsoft-KubeEvents",
    "Microsoft-KubePodInventory",
    "Microsoft-KubeNodeInventory",
    "Microsoft-KubePVInventory",
    "Microsoft-KubeServices",
    "Microsoft-KubeMonAgentEvents",
    "Microsoft-InsightsMetrics",
    "Microsoft-ContainerInventory",
    "Microsoft-ContainerNodeInventory",
    "Microsoft-Perf"
  ]

  syslog_facilities = [
    "auth",
    "authpriv",
    "cron",
    "daemon",
    "mark",
    "kern",
    "local0",
    "local1",
    "local2",
    "local3",
    "local4",
    "local5",
    "local6",
    "local7",
    "lpr",
    "mail",
    "news",
    "syslog",
    "user",
    "uucp"
  ]

  syslog_levels = [
    "Debug",
    "Info",
    "Notice",
    "Warning",
    "Error",
    "Critical",
    "Alert",
    "Emergency"
  ]

  destination_name = "ciworkspace"
}

resource "azurerm_log_analytics_workspace" "workspace" {
  name                = "bugtracker-${var.suffix}-logs"
  
  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name
  
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_monitor_data_collection_rule" "ci_collection_rule" {
  name = "MS-ContainerInsights-${var.suffix}"

  description = "Data Collection Rule for Azure Monitor Container Insights"


  resource_group_name = azurerm_resource_group.group.name
  location            = azurerm_resource_group.group.location

  destinations {
    log_analytics {
      workspace_resource_id = azurerm_log_analytics_workspace.workspace.id
      name                  = local.destination_name
    }
  }

  data_flow {
    streams = local.streams
    destinations = [
      local.destination_name
    ]
  }

  data_flow {
    streams = [
      "Microsoft-Syslog"
    ]
    destinations = [
      local.destination_name
    ]
  }

  data_sources {
    syslog {
      streams = [
        "Microsoft-Syslog"
      ]
      facility_names = local.syslog_facilities
      log_levels     = local.syslog_levels
      name           = "sysLogsDataSource"
    }

    extension {
      streams        = local.streams
      extension_name = "ContainerInsights"
      extension_json = jsonencode({
        "dataCollectionSettings" : {
          "interval" : "1m",
          "namespaceFilteringMode" : "Off",
          "namespaces" : [
            "kube-system",
            "gatekeeper-system",
            "azure-arc"
          ],
          "enableContainerLogV2" : true
        }
      })
      name = "ContainerInsightsExtension"
    }
  }

}

resource "azurerm_monitor_data_collection_rule_association" "ci_collection_rule_association" {
  name = "ContainerInsightsExtension"

  description = "Association of container insights data collection rule. Deleting this association will break the data collection for this AKS Cluster."

  target_resource_id      = azurerm_kubernetes_cluster.cluster.id
  data_collection_rule_id = azurerm_monitor_data_collection_rule.ci_collection_rule.id
}
