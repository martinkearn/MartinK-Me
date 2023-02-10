@description('The name of the Azure Function app.')
param uniqueName string = uniqueString(resourceGroup().id)

@description('Location for all resources.')
param location string = resourceGroup().location

//STORAGE ACCOUNT
var storageAccountName = 'mkstorage${uniqueName}'
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

//ARTICLES STORAGE BLOB CONTAINER
resource storageAccountBlobService 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource storageAccountBlobServiceContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: 'articleblobs'
  parent: storageAccountBlobService
  properties: {
    publicAccess: 'None'
  }
}

//ARTICLES STORAGE TABLE
resource storageAccountTableService 'Microsoft.Storage/storageAccounts/tableServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource storageAccountTableServiceTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'articles'
  parent: storageAccountTableService
}

//APP INSIGHTS
var appInsightsName = 'mk-appinisghts-${uniqueName}'
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: {
    'hidden-link:${resourceId('Microsoft.Web/sites', appInsightsName)}': 'Resource'
  }
  properties: { Application_Type: 'web' }
  kind: 'web'
}

//APP SERVICE PLAN for FUNCTION APP
resource functionAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'mk-functionapp-service-${uniqueName}'
  location: location
  sku: { tier: 'Dynamic', name: 'Y1', family: 'Y', capacity: 1 }
  properties: { reserved: true }
}

//FUNCTION APP
resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'mk-functionapp-${uniqueName}'
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
          value: reference(applicationInsights.id, '2020-02-02').InstrumentationKey
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
        {
          name: 'StorageConfiguration__ArticlesTable'
          value: storageAccountTableServiceTable.name
        }
        {
          name: 'StorageConfiguration__ArticleBlobsContainer'
          value: storageAccountBlobServiceContainer.name
        } 
        {
          name: 'StorageConfiguration__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        }         
      ]
    }
  }
}
output functionAppName string  = functionApp.name

//APP SERVICE PLAN for WEB APP
resource webAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'mk-webapp-service-${uniqueName}'
  location: location
  sku: {
    name: 'B1'
  }
  kind: 'linux'
  properties: { reserved: true }
}

//WEB APP
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: 'mk-webapp-${uniqueName}'
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|7.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(applicationInsights.id, '2020-02-02').InstrumentationKey
        }
        {
          name: 'StorageConfiguration__ArticlesTable'
          value: storageAccountTableServiceTable.name
        }
        {
          name: 'StorageConfiguration__ArticleBlobsContainer'
          value: storageAccountBlobServiceContainer.name
        } 
        {
          name: 'StorageConfiguration__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        } 
      ]
    }
    httpsOnly: true
  }
}
output webAppName string  = webApp.name
