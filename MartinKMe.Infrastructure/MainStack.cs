using MartinKMe.Infrastructure.Helpers;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System;
using Kind = Pulumi.AzureNative.Storage.Kind;

/// <summary>
/// Main stack
/// See https://www.pulumi.com/docs/intro/cloud-providers/azure/setup/ for guidance on setting up the Azure App Registration/Service Principle in the Pulumi config.
/// Requires publish .net projects. Run this command at the same location as the Pulumi stack to put the Functions porject publish output as a sub folder (which is where Pulumi will look for it)
/// dotnet publish --no-restore --configuration Release --output ./publishfunctions ../MartinKMe.Functions/MartinKMe.Functions.csproj
/// </summary>
class MainStack : Stack
{
    private const string ResourceGroupBaseName = "MartinKMe";
    public MainStack()
    {
        var resourceGroup = new ResourceGroup("MartinKMe", new ResourceGroupArgs 
        { 
            ResourceGroupName = ResourceGroupBaseName,
        });

        var storageAccount = new StorageAccount("storage", new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AllowBlobPublicAccess = false,
            AccountName = $"storage{ResourceGroupBaseName.ToLowerInvariant()}",
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS
            },
            Kind = Kind.StorageV2
        });

        var appInsights = new Component("appinsights", new ComponentArgs
        {
            ApplicationType = ApplicationType.Web,
            Kind = "web",
            ResourceGroupName = resourceGroup.Name,
            ResourceName = $"appinsights-{ResourceGroupBaseName.ToLowerInvariant()}"
        });

        var deploymentsContainer = new BlobContainer("deployments", new BlobContainerArgs
        {
            AccountName = storageAccount.Name,
            PublicAccess = PublicAccess.None,
            ResourceGroupName = resourceGroup.Name,
        });

        var articleBlobsContainer = new BlobContainer("articleblobs", new BlobContainerArgs
        {
            AccountName = storageAccount.Name,
            PublicAccess = PublicAccess.None,
            ResourceGroupName = resourceGroup.Name,
        });

        var articlesTable = new Table("articles", new TableArgs
        {
            AccountName = storageAccount.Name,
            ResourceGroupName = resourceGroup.Name,
        });

        var functionsAppServicePlan = new AppServicePlan("functions-appserviceplan", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Name = $"functions-appserviceplan-{ResourceGroupBaseName.ToLowerInvariant()}",
            Sku = new SkuDescriptionArgs
            {
                Tier = "Dynamic",
                Name = "Y1"
            }
        });

        var functionsPublishBlob = new Blob($"functions-{DateTime.UtcNow:yyyy-MM-dd-HH-mm}.zip", new BlobArgs
        {
            AccountName = storageAccount.Name,
            ContainerName = deploymentsContainer.Name,
            ResourceGroupName = resourceGroup.Name,
            Type = BlobType.Block,
            Source = new FileArchive("./publishfunctions") 
        });

        var storageConnectionString = OutputHelpers.GetConnectionString(resourceGroup.Name, storageAccount.Name);
        var functionsAppService = new WebApp($"functions-appservice", new WebAppArgs
        {
            Kind = "FunctionApp",
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = functionsAppServicePlan.Id,
            Name = $"functions-appservice-{ResourceGroupBaseName.ToLowerInvariant()}",
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs{
                        Name = "AzureWebJobsStorage",
                        Value = storageConnectionString,
                    },
                    new NameValuePairArgs{
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet",
                    },
                    new NameValuePairArgs{
                        Name = "FUNCTIONS_EXTENSION_VERSION",
                        Value = "~3",
                    },
                    new NameValuePairArgs{
                        Name = "WEBSITE_RUN_FROM_PACKAGE",
                        Value = OutputHelpers.SignedBlobReadUrl(functionsPublishBlob, deploymentsContainer, storageAccount, resourceGroup, 3650),
                    },                    
                    new NameValuePairArgs{
                        Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                        Value = appInsights.InstrumentationKey
                    },
                    new NameValuePairArgs{
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
                    },
                    new NameValuePairArgs{
                        Name = "StorageConfiguration:ArticleBlobsContainer",
                        Value = articleBlobsContainer.Name,
                    },
                    new NameValuePairArgs{
                        Name = "StorageConfiguration:ConnectionString",
                        Value = storageConnectionString,
                    },
                    new NameValuePairArgs{
                        Name = "StorageConfiguration:ArticlesTable",
                        Value = articlesTable.Name,
                    },
                },
            },
        });

        // Export outputs
        this.StorageConnectionString = storageConnectionString;
    }

    [Output]
    public Output<string> StorageConnectionString { get; set; }
}
