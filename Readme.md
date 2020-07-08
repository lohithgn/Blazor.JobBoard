# Blazor JobBoard

This is a Blazor Web Assembly application inspired by [https://pory.io][pory-io] job board example app [here][pory-jobboard-app].

# Demo
Blazor JobBoard is hosted on azure and can be accessed here: [https://blazjobboard.azurewebsites.net](https://blazjobboard.azurewebsites.net)

# Architecture
Architecture of Blazor JobBoard comprises of the following components:
- JobBoard.Proxy -  Azure Functions Proxy feature is used to proxy the front end pages & back end API. Deployed to Azure Functions App
- JobBoard.Server - Azure Functions app is used to create the backend API. It provides the following endpoints
  - Types: Provides a list of job types in the system.
  - Locations: Provides a list of locations in the system.
  - Jobs: Provides a list of jobs in the system. Supports locations & types query filters to be passed as querystring
- JobBoard.Client: Blaxor WebAssembly project which acts as the job board front end. Communicates with backend api via the proxy
- Cosmos DB: The datastore is Cosmos DB

Here is the architecture diagram:
![Architecture Diagram](https://raw.githubusercontent.com/lohithgn/Blazor.JobBoard/master/Assets/JobBoardArchitecture.png "Architecture Diagram")

# Infrastructure Deployment
Deployment folder contains the Azure CLI script to deploy Blazor JobBoard infrastructure to Azure. The deployment script creates the following components
- Application Insights
- Storage Accounts for Function Apps
- Proxy Function App
- API Function App
- Storage Account for Web App
- Storage Account Static Site Container 

# Code Deployment
You can deploy your code to your infrastructure using Visual Studio or set up a CI/CD pipeline.

# Seed Data
Once you create Cosmos DB, you can use the jobs.json present in Data folder to seed your data store.

# Screen Shots
![Job Listing](https://raw.githubusercontent.com/lohithgn/Blazor.JobBoard/master/Assets/JobBoard.Listing.png "Job Listing")

![Job Details](https://raw.githubusercontent.com/lohithgn/Blazor.JobBoard/master/Assets/JobBoard.Details.png "Job Listing")
 
[pory-io]:https://pory.io
[pory-jobboard-app]:https://indeed-airtable-template.pory.app/