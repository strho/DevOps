resource "azurerm_virtual_network" "network" {
  name = "bugtracker-${var.suffix}"

  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name

  address_space = ["10.1.0.0/16"]
}

resource "azurerm_subnet" "app_gateway" {
  name = "app-gateway-subnet"

  resource_group_name  = azurerm_resource_group.group.name
  virtual_network_name = azurerm_virtual_network.network.name

  address_prefixes = ["10.1.4.0/24"]
}

resource "azurerm_public_ip" "app_gateway" {
  name = "app-gateway-ip-${var.suffix}"

  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name

  allocation_method = "Static"
  sku               = "Standard"
}

resource "azurerm_subnet" "aks_subnet" {
  name = "aks-subnet"

  resource_group_name  = azurerm_resource_group.group.name
  virtual_network_name = azurerm_virtual_network.network.name

  address_prefixes  = ["10.1.1.0/24"]
  service_endpoints = ["Microsoft.Sql"]
}

resource "azurerm_application_gateway" "app_gateway" {
  name = "app-gateway-${var.suffix}"

  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name

  sku {
    name     = "Standard_v2"
    tier     = "Standard_v2"
    capacity = 2
  }

  gateway_ip_configuration {
    name      = "app-gateway-ip-${var.suffix}"
    subnet_id = azurerm_subnet.app_gateway.id
  }

  frontend_port {
    name = "http"
    port = 80
  }

  frontend_ip_configuration {
    name                 = "app-gateway-ip-${var.suffix}"
    public_ip_address_id = azurerm_public_ip.app_gateway.id
  }

  backend_address_pool {
    name = "app-gateway-pool-${var.suffix}"
  }

  backend_http_settings {
    name                  = "app-gateway-settings-${var.suffix}"
    cookie_based_affinity = "Disabled"
    port                  = 80
    protocol              = "Http"
    request_timeout       = 60
  }

  http_listener {
    name                           = "app-gateway-http-listener-${var.suffix}"
    frontend_ip_configuration_name = "app-gateway-ip-${var.suffix}"
    frontend_port_name             = "http"
    protocol                       = "Http"
  }

  request_routing_rule {
    name                       = "app-gateway-rule-${var.suffix}"
    rule_type                  = "Basic"
    priority                   = 100
    http_listener_name         = "app-gateway-http-listener-${var.suffix}"
    backend_address_pool_name  = "app-gateway-pool-${var.suffix}"
    backend_http_settings_name = "app-gateway-settings-${var.suffix}"
  }

  lifecycle {
    # These properties need to be ignored because they are managed by the Kubernetes cluster as App GW is used as ingress controller
    ignore_changes = [
      tags,
      ssl_certificate,
      trusted_root_certificate,
      frontend_port,
      backend_address_pool,
      backend_http_settings,
      http_listener,
      url_path_map,
      request_routing_rule,
      probe,
      redirect_configuration,
      ssl_policy,
    ]
  }
}
