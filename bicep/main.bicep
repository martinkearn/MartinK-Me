@description('Github PAT.')
param githubPat string

@description('A unique name for all resources.')
param uniqueLabel string

@description('Location for all resources.')
param location string = resourceGroup().location // Nothing is being passed so this will use the default

//STORAGE ACCOUNT
var storageAccountName = toLower('storage${uniqueLabel}')
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

//STORAGE BLOB CONTAINERs
resource storageAccountBlobService 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
  dependsOn:{
    storageAccount
  }
}

resource storageAccountArticlesBlobServiceContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: 'articleblobs'
  parent: storageAccountBlobService
  properties: {
    publicAccess: 'None'
  }
  dependsOn:{
    storageAccountBlobService
  }
}

resource storageAccountWallpaperBlobServiceContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: 'wallpaperblobs'
  parent: storageAccountBlobService
  properties: {
    publicAccess: 'None'
  }
  dependsOn:{
    storageAccountBlobService
  }
}

//STORAGE TABLEs
resource storageAccountTableService 'Microsoft.Storage/storageAccounts/tableServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
  dependsOn:{
    storageAccount
  }
}

resource storageAccountTableServiceArticlesTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'articles'
  parent: storageAccountTableService
  dependsOn:{
    storageAccountTableService
  }
}

resource storageAccountTableServiceShortcutsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
  name: 'shortcuts'
  parent: storageAccountTableService
  dependsOn:{
    storageAccountTableService
  }
}

//APP INSIGHTS
var appInsightsName = toLower('appinisghts-${uniqueLabel}')
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
  name: toLower('functionapp-service-${uniqueLabel}')
  location: location
  sku: { tier: 'Dynamic', name: 'Y1', family: 'Y', capacity: 1 }
  properties: { reserved: true }
}

//FUNCTION APP
resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: toLower('functionapp-${uniqueLabel}')
  location: location
  kind: 'functionapp,linux'
  properties: {
    reserved: true
    serverFarmId: functionAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      netFrameworkVersion: '8.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
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
          value: 'dotnet-isolated' // .NET isolated worker process for .NET 8
        }
        {
          name: 'DOTNET_VERSION'
          value: '8'
        }
        {
          name: 'StorageConfiguration__ArticlesTable'
          value: storageAccountTableServiceArticlesTable.name
        }
        {
          name: 'StorageConfiguration__ShortcutsTable'
          value: storageAccountTableServiceShortcutsTable.name
        }
        {
          name: 'StorageConfiguration__ArticleBlobsContainer'
          value: storageAccountArticlesBlobServiceContainer.name
        }
        {
          name: 'StorageConfiguration__WallpaperBlobsContainer'
          value: storageAccountWallpaperBlobServiceContainer.name
        }
        {
          name: 'StorageConfiguration__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value}'
        }
        {
          name: 'GithubConfiguration__Pat'
          value: githubPat
        }
      ]
    }
  }
}
output functionAppName string = functionApp.name

//APP SERVICE PLAN for WEB APP
resource webAppServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: toLower('webapp-service-${uniqueLabel}')
  location: location
  sku: {
    name: 'B1'
  }
  kind: 'linux'
  properties: { reserved: true }
}

//WEB APP
resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: toLower('webapp-${uniqueLabel}')
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'StorageConfiguration__ArticlesTable'
          value: storageAccountTableServiceArticlesTable.name
        }
        {
          name: 'StorageConfiguration__ShortcutsTable'
          value: storageAccountTableServiceShortcutsTable.name
        }
        {
          name: 'StorageConfiguration__ArticleBlobsContainer'
          value: storageAccountArticlesBlobServiceContainer.name
        }
        {
          name: 'StorageConfiguration__WallpaperBlobsContainer'
          value: storageAccountWallpaperBlobServiceContainer.name
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
output webAppName string = webApp.name
