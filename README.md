# MartinK.me

This is the code for my own personal site is published at [MartinK.me](http://MartinK.me).

This is a website which operates a simple blog with a backend content management system driven from GitHub. You can view the files which drive the site at https://github.com/martinkearn/Content.

The website deployment is automated using [Azure Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep) and [Github Actions](https://docs.github.com/en/actions).

## Continuous Deployment
GitHub Actions are used to enforce a continuous deployment approach as follows:
- PR's to `main` branch are be deployed to the Azure "dev" resource group; this worklow must suceed for the PR to be closed. This is automated by `cd-dev.yml` which uses `deployment.yml` to deploy to the `mknet-dev` resource group
- Commits to the `main` branch (following a merged PR) are deployed to the Azure "prod" resource group. This is automated by `cd-prod.yml` which uses `deployment.yml` to deploy to the `mknet-prod` resource group
