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
    * dotnet tool uninstall --global dotnet-aspnet-codegenerator
    * dotnet tool install --global dotnet-aspnet-codegenerator 

    * dotnet aspnet-codegenerator -h
        * should yield nothing

    * dotnet aspnet-codegenerator controller -name Controllers/test

* Create a new controller
    * Have a get endpoint that returns an OkObjectResult("Dingus");
    * via the generator

* Query that through the browser
    * Yeah

## Create a test project
* Create an xunit project
    * dotnet new xunit --name SE2E

* Get the dotnet and it ran with one script
    ```sh
    #!/bin/bash

    cd ../app;
    dotnet build;
    if [ $? -ne 0 ]
    then
        exit;
    fi

    dotnet run &
    thread=$!
    sleep 3;

    cd ../SE2E;
    dotnet test;

    kill $!
    ```

* Test the endpoint, and log the resulting message
    * dotnet test --logger "console"
    * Happy path
        * Check the result is a 200
        * Check log the message


## Create a post endpoint
* TO DESIERALIZE:
    ```C#
            services.AddControllers()
                .AddNewtonsoftJson();
    ```
    ```
        dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson 
    ```

* Take a model, that contains a field called "name"
* returns 200
* returns the 
* return it

## Test it
* Good values
    * Test it output is what's expected 

* Bad values
* Stuff
* things
