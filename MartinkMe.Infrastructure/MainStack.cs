using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

class MainStack : Stack
{
    private const string ResourceGroupBaseName = "MartinKMe";
    public MainStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("MartinKMe", new ResourceGroupArgs 
        { 
            ResourceGroupName = ResourceGroupBaseName,
        });

        // Create an Azure Storage Account
        var storageAccount = new StorageAccount("storage", new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AllowBlobPublicAccess = false,
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS
            },
            Kind = Kind.StorageV2
        });

    }


}
