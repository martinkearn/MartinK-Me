# Workflows
The `deployment.yml` workflow is a GitHub Actions pipeline which deploys both the infrastructure (Bicep) and application (.net) to Azure.

It can deploy to different resource groups based on parameters passed in and is initially configured for a "dev" (`cd-dev.yaml`) and "prod" (`cd-prod.yml`) deployment.

This method is based on https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect.

## Create Resource Group
Create resource groups in Azure for `Dev` and `Prod`.

## Create Application Registration
The Actions pipeline uses OIDC authentication and needs an Azure Application Registration in Microsoft Entra ID.

Create the Microsoft Entra Application Registration and obtain these values:

- `AZURE_CLIENT_ID`: Get this from the `Application (client) ID` on the Overview page of the Application Registration.
- `AZURE_TENANT_ID`: Get this from the `Directory (tenant) ID` on the Overview page of the Application Registration.

## Create Federated Credential

It is best practice to use Federated Credentials in lieu  of a client serret. This needs to be created at Microsoft Entra ID > Application Registration > Certificates & secrets > Federated credentials.

Follow steps in https://learn.microsoft.com/en-us/entra/workload-id/workload-identity-federation-create-trust?pivots=identity-wif-apps-methods-azp

Create one for each resource group i.e `Dev` and `Prod`.

Use these details:

- Federated credential scenario: GitHub Actions deploying Azure resources
- Organisation: The GitHub Organisation i.e https://github.com/martinkearn
- Repository: The GitHub Repository i.e https://github.com/martinkearn/MartinK-Me
- Entity type: For `Dev`, use `Pull request`
- Name: Set its for the deployment type i.e.e `Dev` or `Prod` 

## Assign Application Registration Contributor role in subscription

The application registration needs contributor rights in the subscription in order to create the resource groups and resources.

Assign contributor rights in Azure Portal > Subscriptions > [Subscription Name] > Access control (IAM) > Add > Role Assignment > Privileged administrator roles > Contributor. 

## Configure GitHub Actions Secrets
Follow the steps in https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect#create-github-secrets to configure 3 GitHub secrets using the details obtained in "Create Application Registration"

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`. Get this from the Azure Portal > Subscriptions blade (not part of the Application Registration pages) 

