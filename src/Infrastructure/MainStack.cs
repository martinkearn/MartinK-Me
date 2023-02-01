using MK.Infrastructure.Helpers;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System;
using Kind = Pulumi.AzureNative.Storage.Kind;

/// <summary>
/// Main stack
/// </summary>
class MainStack : Stack
{
    private const string ResourceGroupBaseName = "MK2023Dev";
    public MainStack()
    {
        var resourceGroup = new ResourceGroup(ResourceGroupBaseName, new ResourceGroupArgs{
            Location = "uksouth"
        });

        var storageAccount = new StorageAccount("storage", new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AllowBlobPublicAccess = false,
            AccountName = $"storage{ResourceGroupBaseName.ToLowerInvariant()}",
            Sku = new Pulumi.AzureNative.Storage.Inputs.SkuArgs
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

        var functionsAppServicePlan = new AppServicePlan($"functions-appserviceplan-linux-{ResourceGroupBaseName.ToLowerInvariant()}", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "functionapp",
            Reserved = true,
            Sku = new SkuDescriptionArgs
            {
                Capacity = 0,
                Family = "Y",
                Name = "Y1",
                Size = "Y1",
                Tier = "Dynamic",
            }
        });

        var functionsPublishBlob = new Blob($"functions-{DateTime.UtcNow:yyyy-MM-dd-HH-mm}.zip", new BlobArgs
        {
            AccountName = storageAccount.Name,
            ContainerName = deploymentsContainer.Name,
            ResourceGroupName = resourceGroup.Name,
            Type = BlobType.Block,
            Source = new FileArchive("../workflow/bin/Release/net7.0") 
        });

        var storageConnectionString = OutputHelpers.GetConnectionString(resourceGroup.Name, storageAccount.Name);

        var functionsApp = new WebApp($"functions-app-{ResourceGroupBaseName.ToLowerInvariant()}", new WebAppArgs
        {
            Kind = "functionapp",
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = functionsAppServicePlan.Id,
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
                        Value = "dotnet-isolated",
                    },
                    new NameValuePairArgs{
                        Name = "FUNCTIONS_EXTENSION_VERSION",
                        Value = "~4",
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
                    }
                },
            },
        });

        // Export outputs
        this.SecretStorageConnectionString = storageConnectionString.Apply(Output.CreateSecret);
    }

    [Output]
    public Output<string> SecretStorageConnectionString { get; set; }
}
