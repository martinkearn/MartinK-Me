# Workflows
The `deployment.yml` workflow is a GitHub Actions pipeline which deploys both the infrastructure (Bicep) and application (.net) to Azure.

It can deploy to different resource groups based on parameters passed in and is initially configured for a "dev" (`cd-dev.yaml`) and "prod" (`cd-prod.yml`) deployment.

For the pipeline to work in GitHub Actions, it needs two GitHub Actions secrets to be set as follows.

> Based on https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect

## AZURE_SUBSCRIPTION
The `AZURE_SUBSCRIPTION_ID` subscription secret should contain just the subscription ID for the deployment, for example 

```
fe4379bc-ed4f-48ee-8685-70ac78f34c59
```
