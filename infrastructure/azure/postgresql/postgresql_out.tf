output "connection_string" {
    description = "The connection string to use when connecting to the PostgreSQL server"
    sensitive = true

    value = "Server=${azurerm_postgresql_flexible_server.server.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.database.name};User Id=${azurerm_postgresql_flexible_server.server.administrator_login};Password=${random_password.password.result};Ssl Mode=Require;"
}