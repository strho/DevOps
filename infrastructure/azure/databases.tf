resource "azurerm_key_vault_secret" "userservicedb_connectionstring" {
  name = "userservicedb-connection-string"

  key_vault_id = azurerm_key_vault.vault.id
  value        = module.userservicedb.connection_string
}

resource "azurerm_key_vault_secret" "bugservicedb_connectionstring" {
  name = "bugservicedb-connection-string"

  key_vault_id = azurerm_key_vault.vault.id
  value        = module.bugservicedb.connection_string
}

module "userservicedb" {
  source = "./postgresql"

  name          = "userservicedb-${var.suffix}"
  database_name = "users"
  resource_group = {
    name     = azurerm_resource_group.group.name
    location = azurerm_resource_group.group.location
  }
  virtual_network = {
    name = azurerm_virtual_network.network.name
    id   = azurerm_virtual_network.network.id
  }
  subnet_prefix = "10.1.2.0/24"

}

module "bugservicedb" {
  source = "./postgresql"

  name            = "bugservicedb-${var.suffix}"
  database_name   = "bugs"
  resource_group  = azurerm_resource_group.group
  virtual_network = azurerm_virtual_network.network
  subnet_prefix   = "10.1.3.0/24"
}