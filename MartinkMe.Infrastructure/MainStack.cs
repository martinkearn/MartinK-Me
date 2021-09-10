using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

class MainStack : Stack
{
    public MainStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("MartinKMe");

        // Create an Azure Storage Account
        var storageAccount = new StorageAccount("corestorage", new StorageAccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS
            },
            Kind = Kind.StorageV2
        });

    }


}
