# Brief
* This one focuses on:
    * 1) An ASP.NET API
        * That has an xunit http test project

    * 2) Setup and integrate the database
        * That has a sql database
            * Using Entity to handle the interaction

    * 3) Bundle it all into a container
        * Docker
        * Setting the environment variables

    * 4) Deploy onto azure:
        * That's deployed on azure
        * Via terraform

    * 5) Using VIM To handle all the linting

# ASP.NET

## Setup the project
* dotnet new webapi --name api

* Install the code generator    
    * Add the design
    * build
    * then scaffold

    * dotnet tool uninstall --global dotnet-aspnet-codegenerator
    * dotnet tool install --global dotnet-aspnet-codegenerator --version 3.1.0
    * dotnet tool update dotnet --global

    * dotnet tool install -g dotnet-aspnet-codegenerator --version 3.1
    * dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Tools
    * dotnet add pacakge Microsoft.AspNetCore.All

* Create a new controller
    * via the generator

* Query that through the browser
    * Yeah

## Create a test project
* Test the endpoint
    * Happy path
    * Bad path

## Create a post endpoint
* Take a model
* form a string using it
* return it

## Test it
* Good values
* Bad values
* Stuff
* things
