using MartinkMe.Infrastructure.Helpers;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Kind = Pulumi.AzureNative.Storage.Kind;

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
        }); ;

        var deploymentsContainer = new BlobContainer("deployments", new BlobContainerArgs
        {
            AccountName = storageAccount.Name,
            PublicAccess = PublicAccess.None,
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

        var functionsPublishBlob = new Blob("functions.zip", new BlobArgs
        {
            AccountName = storageAccount.Name,
            ContainerName = deploymentsContainer.Name,
            ResourceGroupName = resourceGroup.Name,
            Type = BlobType.Block,
            Source = new FileArchive($"..\\MartinKMe.Functions\\bin\\Release\\netcoreapp3.1\\publish") // This path should be set to the output of `dotnet publish` command
        });

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
                        Value = OutputHelpers.GetConnectionString(resourceGroup.Name, storageAccount.Name),
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
                },
            },
        });

        var webAppServicePlan = new AppServicePlan("web-appserviceplan", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Name = $"web-appserviceplan-{ResourceGroupBaseName.ToLowerInvariant()}",
            Sku = new SkuDescriptionArgs
            {
                Tier = "Shared",
                Name = "D1"
            }
        });

        var webPublishBlob = new Blob("web.zip", new BlobArgs
        {
            AccountName = storageAccount.Name,
            ContainerName = deploymentsContainer.Name,
            ResourceGroupName = resourceGroup.Name,
            Type = BlobType.Block,
            Source = new FileArchive($"..\\MartinKMe.Web\\bin\\Debug\\net6.0\\publish") // This path should be set to the output of `dotnet publish` command
        });

        var webAppService = new WebApp($"web-appservice", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = webAppServicePlan.Id,
            Name = $"web-appservice-{ResourceGroupBaseName.ToLowerInvariant()}",
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs{
                        Name = "WEBSITE_RUN_FROM_PACKAGE",
                        Value = OutputHelpers.SignedBlobReadUrl(webPublishBlob, deploymentsContainer, storageAccount, resourceGroup, 3650),
                    },                    
                    new NameValuePairArgs{
                        Name = "APPINSIGHTS_INSTRUMENTATIONKEY",
                        Value = appInsights.InstrumentationKey
                    },
                    new NameValuePairArgs{
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
                    },
                },
            },
        });

    }
}