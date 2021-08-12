module "sql_server"{
    source = "github.com/JVHallam/infrastructure/azure_sql"
    client_ip_address = "91.110.158.178"
}

output "connection_string"{
    value = module.sql_server.connection_string
    sensitive = true
}
