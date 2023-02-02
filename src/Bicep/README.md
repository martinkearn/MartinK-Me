# Deployment with Bicep

## Deploy Resources
1. Open a Terminal at the root of the `/src/Bicep` folder
1. Login to Azure using `az login`
1. Create a resource group using `az group create --name resourcegroupnamehere --location uksouth`
1. Deploy the Bicep template to your new resource group using `az deployment group create --resource-group resourcegroupnamehere --template-file main.bicep`

## Deploy Azure Function code
1. Open a Terminal at the root of the `/src/Workflow` folder
1. Login to Azure using `az login`
1. Deploy the function code using `func azure functionapp publish functionapp-k2akhwhqvpt5u --dotnet-isolated --dotnet-version 7.0`
