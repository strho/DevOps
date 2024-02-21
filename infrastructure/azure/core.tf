provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "group" {
  name     = "bugtracker-${var.suffix}"
  location = "West Europe"
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "vault" {
  name = "bugtracker-${var.suffix}"

  location            = azurerm_resource_group.group.location
  resource_group_name = azurerm_resource_group.group.name

  tenant_id = data.azurerm_client_config.current.tenant_id
  sku_name  = "standard"
}

resource "azurerm_key_vault_access_policy" "policy" {
  key_vault_id = azurerm_key_vault.vault.id

  tenant_id = data.azurerm_client_config.current.tenant_id
  object_id = data.azurerm_client_config.current.object_id

  secret_permissions = [
    "Get",
    "List",
    "Set"
  ]
}

resource "azurerm_key_vault_access_policy" "cluster" {
  key_vault_id = azurerm_key_vault.vault.id

  tenant_id = data.azurerm_client_config.current.tenant_id
  object_id = azurerm_kubernetes_cluster.cluster.key_vault_secrets_provider[0].secret_identity[0].object_id

  secret_permissions      = ["Get"]
}
