# Infrastructure
This project contains Pulumi code to deploy the required infrastrcuture to Azure.

## Publish FunctionsV4 project
1. Open a Terminal in `/MK.Infrastructure`
1. `dotnet publish --no-restore --configuration Release --output ./publishfunctions ../MK.Workflow/Workflow.csproj`

## Deploy Pulumi stack
1. Ensure you have Pulumi and Azure CLI's installed: https://www.pulumi.com/docs/get-started/azure/begin/
1. Open a Terminal in `/MK.Infrastructure`
1. `az login` and follow browser prompts to login to Azure
1. Identify the subscription you want Pulumi to deploy to and use `az account set --subscription <subscription id here>` to set it as the current subscription
1. `pulumi config set azure-native:location uksouth` to set the region and login to Pulumi and create a new stack called `main` as prompted
1. `pulumi up -y -s main` to deploy the main stack
