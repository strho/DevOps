resource "random_password" "password" {
  length = 16

  min_upper   = 1
  min_lower   = 1
  min_numeric = 1
  min_special = 1

  special          = true
  override_special = "!_%@"
}

resource "azurerm_subnet" "subnet" {
  name                 = "${var.name}-subnet"

  resource_group_name  = var.resource_group.name
  virtual_network_name = var.virtual_network.name
  
  address_prefixes     = [ var.subnet_prefix ]
  service_endpoints    = ["Microsoft.Storage"]
  delegation {
    name = "fs"
    service_delegation {
      name = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action",
      ]
    }
  }
}
resource "azurerm_private_dns_zone" "private_dns_zone" {
  name                = "${var.name}-zone.postgres.database.azure.com"
  resource_group_name = var.resource_group.name
}

resource "azurerm_private_dns_zone_virtual_network_link" "network_link" {
  name                  = "${var.name}VnetZone.com"
  
  private_dns_zone_name = azurerm_private_dns_zone.private_dns_zone.name
  virtual_network_id    = var.virtual_network.id
  
  resource_group_name   = var.resource_group.name
  
  depends_on            = [ azurerm_subnet.subnet ]
}

resource "azurerm_postgresql_flexible_server" "server" {
    name                = var.name

    location            = var.resource_group.location
    resource_group_name = var.resource_group.name
    
    administrator_login    = "psqladmin"
    administrator_password = random_password.password.result

    delegated_subnet_id    = azurerm_subnet.subnet.id
    private_dns_zone_id    = azurerm_private_dns_zone.private_dns_zone.id

    
    zone = "3"
    storage_mb          = 32768
    sku_name            = "GP_Standard_D2s_v3"
    version             = "12"


    depends_on = [
      azurerm_private_dns_zone_virtual_network_link.network_link
    ]
}

resource "azurerm_postgresql_flexible_server_database" "database" {
  name                = var.database_name
  server_id = azurerm_postgresql_flexible_server.server.id  
}