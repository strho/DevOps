resource "azurerm_role_assignment" "gw_reader" {
  role_definition_name = "Reader"

  principal_id = azurerm_kubernetes_cluster.cluster.ingress_application_gateway[0].ingress_application_gateway_identity[0].object_id
  scope        = azurerm_application_gateway.app_gateway.id
}

resource "azurerm_role_assignment" "gw_contributor" {
  role_definition_name = "Contributor"

  principal_id = azurerm_kubernetes_cluster.cluster.ingress_application_gateway[0].ingress_application_gateway_identity[0].object_id
  scope        = azurerm_application_gateway.app_gateway.id
}

resource "azurerm_role_assignment" "gw_subnet" {
  role_definition_name = "Network Contributor"

  principal_id = azurerm_kubernetes_cluster.cluster.ingress_application_gateway[0].ingress_application_gateway_identity[0].object_id
  scope        = azurerm_subnet.app_gateway.id
}

resource "azurerm_role_assignment" "gv_vnet" {
  role_definition_name = "Network Contributor"

  principal_id = azurerm_kubernetes_cluster.cluster.ingress_application_gateway[0].ingress_application_gateway_identity[0].object_id
  scope        = azurerm_virtual_network.network.id
}

resource "azurerm_kubernetes_cluster" "cluster" {
  name = "bugtracker-${var.suffix}"

  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name

  dns_prefix = "bugtracker-${var.suffix}"

  ingress_application_gateway {
    gateway_id = azurerm_application_gateway.app_gateway.id
  }

  # If deploying Managed Prometheus, the monitor_metrics properties are required to configure the cluster for metrics collection. If no value is needed, set properties to null.
  monitor_metrics {
    annotations_allowed = null
    labels_allowed      = null
  }

  default_node_pool {
    name           = "default"
    node_count     = 1
    vm_size        = "Standard_D2_v2"
    vnet_subnet_id = azurerm_subnet.aks_subnet.id
  }

  oms_agent {
    log_analytics_workspace_id      = azurerm_log_analytics_workspace.workspace.id
    msi_auth_for_monitoring_enabled = true
  }

  key_vault_secrets_provider {
    secret_rotation_enabled = true
  }

  identity {
    type = "SystemAssigned"
  }
}

provider "helm" {
  kubernetes {
    host                   = azurerm_kubernetes_cluster.cluster.kube_config[0].host
    client_certificate     = base64decode(azurerm_kubernetes_cluster.cluster.kube_config[0].client_certificate)
    client_key             = base64decode(azurerm_kubernetes_cluster.cluster.kube_config[0].client_key)
    cluster_ca_certificate = base64decode(azurerm_kubernetes_cluster.cluster.kube_config[0].cluster_ca_certificate)
  }
}

resource "helm_release" "aks_secret_provider" {
  name = "aks-secret-provider"

  chart   = "${path.module}/../helm/aks-secret-provider"
  version = "1.0.0"

  values = [yamlencode({
    connection = {
      clientId  = azurerm_kubernetes_cluster.cluster.key_vault_secrets_provider[0].secret_identity[0].client_id
      tenantId  = data.azurerm_client_config.current.tenant_id
      vaultName = azurerm_key_vault.vault.name
    }
    secrets = [
      azurerm_key_vault_secret.userservicedb_connectionstring.name,
      azurerm_key_vault_secret.bugservicedb_connectionstring.name
    ]
  })]
  force_update = true
}
