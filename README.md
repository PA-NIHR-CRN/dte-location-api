# DTE Location API
This project is an ASP.NET Core Web API project as an AWS Lambda exposed through Amazon API Gateway. 

## Run the application locally

The Serverless Application Model Command Line Interface (SAM CLI) is an extension of the AWS CLI that adds functionality for building and testing Lambda applications. It uses Docker to run your functions in an Amazon Linux environment that matches Lambda. It can also emulate your application's build environment and API.

To use the SAM CLI, you need the following tools.

* SAM CLI - [Install the SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html)
* .NET Core - [Install .NET Core](https://www.microsoft.com/net/download)
* Docker - [Install Docker community edition](https://hub.docker.com/search/?type=edition&offering=community)

## Here are some steps to follow to get started from the command line:

Execute build
```
    dotnet build
```

Execute unit tests
```
    dotnet test
```

Execute SAM client to run the function locally, you can choose or create a new event that points to the controller action
```
sam build LocationApiFunction --template template.yaml --build-dir .aws-sam/build
sam local invoke LocationApiFunction --template .aws-sam/build/template.yaml --event "./events/GettAddressByPostCode.json"
```