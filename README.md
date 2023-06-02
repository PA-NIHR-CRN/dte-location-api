# Location-API

Location-API is a ASP.NET Core Web API project that provides location-based services for applications requiring geolocation data. It is designed to interact and support a variety of applications by providing geographical information. The API is hosted and runs independently, without the need for AWS Lambda or API Gateway.

## Table of Contents
- [Project Description](#project-description)
- [How to Install and Run the Project](#how-to-install-and-run-the-project)
- [Usage](#usage)

## Project Description
Location-API is developed to support applications requiring location-related information. The API provides a seamless service to deliver geolocation data based on input parameters such as coordinates, addresses, or point of interest.

The API interacts and complements other services by providing valuable location data for various operations. The API is built using ASP.NET Core and hosted independently, ensuring a flexible and platform-independent setup.

## How to Install and Run the Project
To run the project, you will need to have the following installed on your machine:

- .NET Core 6 SDK or later

To run the project, follow these steps:
1. Clone the repository to your local machine.
2. Update `appsettings.json` with the appropriate settings.
3. Right click on the location-api project and select properties. When the modal pops up select run/configurations/default and set the environment variable ASPNETCORE_ENVIRONMENT to Development.
4. To access the API directly go to https://localhost:5001/swagger.

## Usage
To use the API, send HTTP requests to the local endpoint.

To use the project, you can follow these steps:

1. Set the environment variable `ASPNETCORE_ENVIRONMENT` to `Development`.
2. Start the project.
3. Access the API through `https://localhost:5001/swagger`.
