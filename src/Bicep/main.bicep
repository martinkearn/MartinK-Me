@description('The name of the Azure Function app.')
param uniqueName string = uniqueString(resourceGroup().id)

@description('Location for all resources.')
param location string = resourceGroup().location

//STORAGE ACCOUNT
var storageAccountName = 'storage${uniqueName}'
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

//APP INSIGHTS
var appInsightsName = 'appinisghts-${uniqueName}'
resource applicationInsights 'microsoft.insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: {
    'hidden-link:${resourceId('Microsoft.Web/sites', appInsightsName)}': 'Resource'
  }
  properties: { Application_Type: 'web' }
  kind: 'web'
}

//APP SERVICE PLAN for FUNCTION APP
resource functionAppServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'appservice-${uniqueName}'
  location: location
  sku: { tier: 'Dynamic', name: 'Y1', family: 'Y', capacity: 1 }
  properties: { reserved: true }
}

//FUNCTION APP
resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: 'functionapp-${uniqueName}'
  location: location
  kind: 'functionapp,linux'
  properties: {
    reserved: true
    serverFarmId: functionAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|7.0'
      netFrameworkVersion:'7.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(applicationInsights.id, '2015-05-01').InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }          
      ]
    }
  }
}

