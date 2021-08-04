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


-----------------------------------------------------------------------------
# Move this into the next kata
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
