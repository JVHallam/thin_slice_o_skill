# Brief
* This one focuses on:
    * 1) An ASP.NET API
        * That has an xunit http test project

    * 2) Setup and integrate the database
        * That has a sql database
            * Using Entity to handle the interaction
         * Use a SQL container here

    * 3) Bundle it all into a container
        * Docker
        * Buildling the application 
        * Setting the environment variables
        * docker-compose.yml

    * 4) Work it out. Something C# focused, DI or something similar would be nice
      * Something C# Focused
      * DI?
      * Something related to the above would be nice
      * Look through what the work application has to do, go from there

    ---------------------------------
    * TBD:
        * 8) Pipeline?
            * Create a repo
            * Put the yaml in that repo
            * Get it to build an image

        * 6) Checking our tools?
            * Using VIM to lint C#?
            * Using VIM to lint in general
            * Using VIM to jump between tags?
            * Using VIM to navigate like i'm in VS?
            * using VIM to find files by name, faster
            * setting a base point
            * CLI Linting
            * w/e

        * 6.5)
            * Add a razor controller
            * A JS page that can save a customer
            * Get all the customers, render them out

        * 7) More C# Stuff?
            * Database initialisation in program.cs
            * http factory
            * automapper for endpoint models to EF models?
            * using endpoint models in tests
            * Configs?


# ASP.NET

## Setup the project
* dotnet new webapi --name api

* Install the code generator    
    * dotnet tool uninstall --global dotnet-aspnet-codegenerator
    * dotnet tool install --global dotnet-aspnet-codegenerator 

    * dotnet aspnet-codegenerator -h
        * should yield nothing

    * dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    * dotnet aspnet-codegenerator -h
        * Should now yield controller stuff

    * dotnet aspnet-codegenerator controller -name Test -api -outDir Controllers

* Create a new controller
    * call it UserController
    * Have a get endpoint that returns an OkObjectResult("Dingus");
    * via the generator
    * make it's route the root of the website

* Query that through the browser

## Create a test project
* Create an xunit project
    * dotnet new xunit --name SE2E

* Get the dotnet and it ran with one script
    ```sh
    #!/bin/bash

    # Build the test project
    dotnet build;
    if [ $? -ne 0 ]
    then
        exit;
    fi

    pushd .;
    cd ../api;
    dotnet build;
    if [ $? -ne 0 ]
    then
        popd;
        exit;
    fi

    dotnet run &
    thread=$!
    sleep 3;

    popd;
    dotnet test;

    kill $!;
    ```

* Test the endpoint, and log the resulting message
    * Create an http client, that ignores the certificate errors
        * 
    * dotnet test --logger "console"
    * Happy path
        * Check the result is a 200
        * Check log the message

    ```C#
    using System;
    using Xunit;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    namespace se2e
    {
        public class UnitTest1
        {
            string _baseUrl = "https://localhost:5001";
            HttpClient _client;

            public UnitTest1(){
                var httpClientHandler = new HttpClientHandler();

                httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                httpClientHandler.AllowAutoRedirect = false;

                _client = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_baseUrl) };
            }

            [Fact]
            public async Task Test1()
            {
                var response = await _client.GetAsync("");
                var body = await response.Content.ReadAsStringAsync();
                Assert.Equal( (int)HttpStatusCode.OK, (int)response.StatusCode);
                Assert.Equal( "Dingus", body );
            }
        }
    }

    ```

# Database Integration

* Write the infra for an azure sql database
    * This is in my infra repo
    ```
    provider "azurerm" {
      features {}
    }

    module "naming" {
      source = "github.com/azure/terraform-azurerm-naming"
      suffix = ["jvh", "sql"]
    }

    resource "azurerm_resource_group" "sql" {
      name     = module.naming.resource_group.name_unique
      location = "UK South"
    }

    resource "azurerm_mssql_server" "sql" {
      name                = module.naming.sql_server.name_unique
      resource_group_name = azurerm_resource_group.sql.name
      location            = azurerm_resource_group.sql.location

      version                      = "12.0"
      administrator_login          = "jvhadmin"
      administrator_login_password = "Jvhpassword1"
    }

    resource "azurerm_mssql_database" "sql" {
      name      = "infra_database"
      server_id = azurerm_mssql_server.sql.id
    }

    resource "azurerm_mssql_firewall_rule" "example" {
      name             = "AllowHomeAddress"
      server_id        = azurerm_mssql_server.sql.id
      start_ip_address = var.client_ip_address
      end_ip_address   = var.client_ip_address
    }

    locals {
      test = { "key" = "value" }
      connection = {
        "Server"                 = azurerm_mssql_server.sql.fully_qualified_domain_name
        "Initial Catalog"        = azurerm_mssql_database.sql.name
        "User ID"                = azurerm_mssql_server.sql.administrator_login
        "Password"               = azurerm_mssql_server.sql.administrator_login_password
        "TrustServerCertificate" = "False"
        "Connection Timeout"     = "30"
      }
      connection_array  = [for key in keys(local.connection) : "${key}=${local.connection[key]}"]
      connection_string = join(";", local.connection_array)
    }
    ```

* Connect to it
    * how grab global settings?
        * There has to be a better way than my own object, right?

    * Entity 
    ```sh
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```

    * Create the db context stuff
    ```C#
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    public class CustomerContext : DbContext{
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options){
            var connectionString = Environment.GetEnvironmentVariable("connection_string");
            options.UseSqlServer(connectionString);
        }
    }

    public class Customer{
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
    }
    ```

    * Handle the migrations
        * At the base of the project
        ```ps1
        $ENV:connection_string="Server=localhost,1433;Database=customer;User Id=sa;Password=thisismypassword1!;"
        dotnet tool install --global dotnet-ef
        dotnet-ef migrations add customer
        dotnet-ef database update
        ```
        * Will check that the schema is now in the database

    * Integrate this into the api, somehow
    ```C#
    using var dbContext = new CustomerContext();

    var customer = new Customer{
        Name = "Peen",
        Age = 10 
    };

    dbContext.Customers.Add( customer );

    dbContext.SaveChanges();
    ```

* Use entity to read-write 
    * Create the test:

    * Setup deserialization
        * TO DESIERALIZE:
            ```C#
                    services.AddControllers()
                        .AddNewtonsoftJson();
            ```
            ```
                dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson 
            ```

    * Create an endpoint, that takes a customer object
    ```
    public class Customer{
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
    }
    ```
        * save it to the database
        * returns an id

    * Create an endpoint that
        * takes the id
        * returns the object

* test the values?
    * Use the existing setup
    * Create a test where
        * You post an object with random values to one endpoint
            ```sh
            dotnet add package Bogus
            ```
        * You retrieve it from another endpoint, checking the values are the same

    * Implement the saving to the endpoint

    * Run it

    * Setup the controller + services + shit
        * Create the context + handle the migration stuff ( Until i know more about how Context works, put the env variable stuff here )
        ```C#
        public class CustomerContext : DbContext{
            public DbSet<Customer> Customers { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder options){
                var connectionString = Environment.GetEnvironmentVariable("connection_string");
                options.UseSqlServer(connectionString);
            }
        }
        ```

        * startup.cs injection:
        ```C#
        services.AddDbContext<CustomerContext>();
        ```

        * To save the customer object into the database
        ```C#
        _customerContext.Customers.Add( customer );
        _customerContext.SaveChanges();
        ```

        * To query, you use linq:
        ```C#
        var customer = _customerContext.Customers.First(customer => customer.Id == customerId);
        ```

# Docker:
* Create a docker image, using the given api
    * Create the dockerfile
    ```Dockerfile
    # https://hub.docker.com/_/microsoft-dotnet
    FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
    WORKDIR /source

    # copy csproj and restore as distinct layers
    COPY *.csproj .
    RUN dotnet restore --disable-parallel

    # copy everything else and build app
    COPY . .
    # RUN dotnet publish -c release -o /app --no-restore
    RUN dotnet publish -c release -o /app 

    # final stage/image
    FROM mcr.microsoft.com/dotnet/aspnet:5.0
    WORKDIR /app
    COPY --from=build /app .
    EXPOSE 80
    ENTRYPOINT ["dotnet", "api.dll"]
    ```

    * Run the commands to build and test the image:
    ```bash
    sudo docker build . --file Dockerfile -t thinsliceapi

    # List the images
    sudo docker images 

    # Run the image
    # Bind host port 5000, to container port 80
    sudo docker run -p 5000:80 -d thinsliceapi 

    sudo docker kill <container_name>
    ```

        * run the image
        * stop the image


* Make sure to set the environment variables



* Run the SE2E tests against said image
    * Pass in the url
* Maybe:
    * Deploy this to azure, using terraform

------------------------------------------------------------------------------------------------------------------------------
# Update: Using the web application factory for tests, instead of spinning up and testing an instance
* Get the testing packages to the xunit project
```ps1
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.NET.Test.Sdk
```
    
* Add a reference to the other project in the .csproj
```xml
<ItemGroup>
 <ProjectReference Include="..\api\api.csproj" />
</ItemGroup>
```
   
* (Dotnet core 6) In the api project, expose the partial program class, Program.cs:
```c#
//Final line
public partial class Program { }
```

* Create the client and the application factory in you test.cs
```c#
var application = new WebApplicationFactory<Program>()
   .WithWebHostBuilder(builder =>
           {
           // ... Configure test services
           });

var client = application.CreateClient();
```
------------------------------------------------------------------------------------------------------------------------------
* DOTNET CORE 6 WITH EF - Handling migrations inside the code
* The customer model:
```c#
using System;
using System.ComponentModel.DataAnnotations;

namespace api.entity.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name{ get; set; }
        public int Age { get; set; }
    }
}
```

* The customer context
```c#
using System;
using api.entity.Models;
using Microsoft.EntityFrameworkCore;

namespace api.entity.Context
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public CustomerContext(DbContextOptions options) : base(options)
        {
        }
    }
}
```

* The Program.cs
```c#
using api.entity.Context;
using Microsoft.EntityFrameworkCore;

//Checking the connection string
var connectionString = Environment.GetEnvironmentVariable("connection_string");
if(String.IsNullOrEmpty(connectionString)){
    throw new Exception("CONNECTION STRING BOYO");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registering the contexts
builder.Services.AddDbContext<CustomerContext>(
            options => options.UseSqlServer(connectionString)
        );

//Using the contexts to setup the migration
var context = builder.Services.BuildServiceProvider()
                   .GetService<CustomerContext>();
context.Database.Migrate();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
```



------------------------------------------------------------------------------------------------------------------------------

# For later:
* Environment variables:
    * What's a clean way to handle checking these exist
    * Clean way of passing these in
    * clean way of failing when those aren't found

* Initialising the database:
    * Like the above
    * Done in the program.cs
    * https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-5.0#:~:text=services.AddDbContext%3CSchoolContext%3E(options%20%3D%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20options.UseSqlServer(Configuration.GetConnectionString(%22DefaultConnection%22)))%3B

* Controller models to database model mapping
    * Auto mapper.
    * Just mapping controller models to EF models

* Middleware
    * Use fluent validation, to create a middleware, to validate incoming objects
