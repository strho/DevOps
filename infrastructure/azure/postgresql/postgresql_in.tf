variable "name" {
  description = "The name of the PostgreSQL server"
  type = string
}

variable "resource_group" {
    description = "The resource group properties"
    type = object({
        name = string,
        location = string 
    })
  
}

variable "database_name" {
  description = "The name of the database to create"
  type = string
}

variable "virtual_network" {
  description = "values for the virtual network"
  type = object({
    name = string,
    id = string
  })
}

variable "subnet_prefix" {
  description = "The address prefix to use for the PostgreSQL subnet"
  type = string
}