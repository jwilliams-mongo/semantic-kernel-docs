---
title: Deploy Chat Copilot to Azure as a web app service
description: How-to guide on how to deploy Semantic Kernel to Azure in a web app service
author: nilesha
ms.topic: Azure
ms.author: nilesha
ms.date: 05/19/2023
ms.service: semantic-kernel
---

# Deploy Chat Copilot to Azure as a web app service
[!INCLUDE [subheader.md](../includes/pat_large.md)]

In this how-to guide we will provide steps to deploy Semantic Kernel to Azure as a web app service. 
Deploying Semantic Kernel as web service to Azure provides a great pathway for developers to take advantage of Azure compute and other services such as Azure Cognitive Services for responsible AI and vectorized databases.  

## Prerequisites

Chat Copilot deployments use Azure Active Directory to authenticate users and secure access to the backend web service. These steps will walk you through the configuration needed to ensure smooth access to your deployment.

### App registrations (identity)

You will need two Azure Active Directory (AAD) [application registrations](/azure/active-directory/develop/quickstart-register-app) -- one for the frontend web app and one for the backend API.

> NOTE: Other account types can be used to allow multitenant and personal Microsoft accounts to use your application if you desire. Doing so may result in more users and therefore higher costs.

#### Frontend app registration

- Select `Single-page application (SPA)` as platform type, and set the redirect URI to `http://localhost:3000`
- Select `Accounts in this organizational directory only ({YOUR TENANT} only - Single tenant)` as supported account types.
- Make a note of the `Application (client) ID` from the Azure Portal. This is the value for `YOUR_FRONTEND_CLIENT_ID` referenced below.

#### Backend app registration

- Do not set a redirect URI
- Select `Accounts in this organizational directory only ({YOUR TENANT} only - Single tenant)` as supported account types.
- Make a note of the `Application (client) ID` from the Azure Portal. This is the value for `YOUR_BACKEND_CLIENT_ID` referenced below.

#### Linking the frontend to the backend

In the backend app registration:
- Expose an API
    - Select `Expose an API` from the menu
    - Add an `Application ID URI`
- Add a scope
    - Set `Scope name` to `access_as_user`
    - Set `Who can consent` to `Admins and users`
- Add a client application
    - For `Client ID`, enter the frontend's application (client) ID
    - Be sure to check the box under `Authorized scopes`

In the frontend app registration:
- Select `API Permissions` from the menu
- Add a permission
    - Select the tab `APIs my organization uses`
    - Choose the app registration for the backend
    - Select the `access_as_user` permission

## Considerations
You can use one of the deployment options to deploy based on your use case and preference. Below are some considerations to keep in mind when choosing a deployment option.

1. Azure currently limits the number of Azure OpenAI resources per region per subscription to 3. Azure OpenAI is not available in every region.
(Refer to this [availability map](https://azure.microsoft.com/explore/global-infrastructure/products-by-region/?products=cognitive-services)) Bearing this in mind, you might want to use the same Azure OpenAI instance for multiple deployments of Semantic Kernel to Azure.

1. F1 and D1 App Service SKU's (the Free and Shared ones) are not supported for this deployment.

1. Ensure you have sufficient permissions to create resources in the target subscription.

1. Using web frontends to access your deployment: make sure to include your frontend's URL as an allowed origin in your deployment's CORS settings. Otherwise, web browsers will refuse to let JavaScript make calls to your deployment.
<Br></br>

## Deployment Options
[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://aka.ms/sk-deploy-existing-azureopenai-portal)<br>
[PowerShell File](https://aka.ms/sk-deploy-existing-azureopenai-powershell)<br>
[Bash File](https://aka.ms/sk-deploy-existing-azureopenai-bash)

 
## Script Parameters
Below are examples on how to run the PowerShell and bash scripts. Refer to each of the script files for the complete list of available parameters and usage. 
### <b>PowerShell</b>

* Creating new Azure OpenAI Resources
```powershell
./deploy-azure.ps1 -Subscription {YOUR_SUBSCRIPTION_ID} -DeploymentName {YOUR_DEPLOYMENT_NAME} -AIService {AzureOpenAI or OpenAI} -AIApiKey {YOUR_AI_KEY} -AIEndpoint {YOUR_AZURE_OPENAI_ENDPOINT} -BackendClientId {YOUR_BACKEND_APPLICATION_ID} -FrontendClientId {YOUR_FRONTEND_APPLICATION_ID} -TenantId {YOUR_TENANT_ID}
```

- To use an existing Azure OpenAI resource, set `-AIService` to `AzureOpenAI` and include `-AIApiKey` and `-AIEndpoint`.
- To deploy a new Azure OpenAI resource, set `-AIService` to `AzureOpenAI` and omit `-AIApiKey` and `-AIEndpoint`.
- To use an an OpenAI account, set `-AIService` to `OpenAI` and include `-AIApiKey`.

### <b>Bash</b>
```bash
chmod +x ./deploy-azure.sh
./deploy-azure.sh --subscription {YOUR_SUBSCRIPTION_ID} --deployment-name {YOUR_DEPLOYMENT_NAME} --ai-service {AzureOpenAI or OpenAI} --ai-service-key {YOUR_AI_KEY} --ai-endpoint {YOUR_AZURE_OPENAI_ENDPOINT} --client-id {YOUR_BACKEND_APPLICATION_ID} --frontend-client-id {YOUR_FRONTEND_APPLICATION_ID} --tenant-id {YOUR_TENANT_ID}
```

- To use an existing Azure OpenAI resource, set `--ai-service` to `AzureOpenAI` and include `--ai-service-key` and `--ai-endpoint`.
- To deploy a new Azure OpenAI resource, set `--ai-service` to `AzureOpenAI` and omit `--ai-service-key` and `--ai-endpoint`.
- To use an an OpenAI account, set `--ai-service` to `OpenAI` and include `--ai-service-key`.

### Azure Portal Template
If you choose to use Azure Portal as your deployment method, you will need to review and update the template form to create the resources. Below is a list of items you will need to review and update.
1. Subscription: decide which Azure subscription you want to use. This will house the resource group for the Semantic Kernel web application.
1. Resource Group: the resource group in which your deployment will go. Creating a new resource group helps isolate resources, especially if you are still in active development.
1. Region: select the geo-region for deployment. Note: Azure OpenAI is not available in all regions and is currently to three instances per region per subscription.
1. Name: used to identify the app.
1. App Service SKU: select the pricing tier based on your usage. Click [here](https://azure.microsoft.com/pricing/details/app-service/windows/) to learn more about Azure App Service plans.
1. Package URI: there is no need to change this unless you want to deploy a customized version of Semantic Kernel. (See [this page](./publish-changes-to-azure.md) for more information on publishing your own version of the Semantic Kernel web app service)
1. Completion, Embedding and Planner Models: these are by default using the appropriate models based on the current use case - that is Azure OpenAI or OpenAI. You can update these based on your needs.
1. Endpoint: this is only applicable if using Azure OpenAI and is the Azure OpenAI endpoint to use.
1. API Key: enter the API key for the instance of Azure OpenAI or OpenAI to use.
1. Web API Client ID: the application (client) ID associated with your backend app registration.
1. Azure AD Tenant ID: The Azure AD tenant against which to authenticate users. For single tenant applications (recommended), this will match the tenant ID of your backend app registration.
1. Azure AD Instance: This is the Azure cloud instance to use for authenticating users. The default is https://login.microsoftonline.com/. If you are using a sovereign cloud, you will need to update this value.
1. CosmosDB: whether to deploy a CosmosDB resource to store chats. Otherwise, volatile memory will be used.
1. Speech Services: whether to deploy an instance of the Azure Speech service to provide speech-to-text for input.

## What resources are deployed?
Below is a list of the key resources created within the resource group when you deploy Semantic Kernel to Azure as a web app service.
1. Azure web app service: hosts Semantic Kernel
1. Application Insights: application logs and debugging 
1. Azure Cosmos DB: used for chat storage (optional)
1. Qdrant vector database (within a container): used for embeddings storage (optional)
1. Azure Speech service: used for speech-to-text (optional)

## Verifying the deployment

To make sure your web app service is running, go to <!-- markdown-link-check-disable -->https://YOUR_INSTANCE_NAME.azurewebsites.net/healthz<!-- markdown-link-check-enable-->

To get your instance's URL, go to your deployment's resource group (by clicking on the "Go to resource group" button seen at the conclusion of your deployment if you use the "Deploy to Azure" button). Then click on the resource whose name ends with "-webapi".

This will bring you to the Overview page on your web service. Your instance's URL is the value that appears next to the "Default domain" field.

## Changing your configuration and monitoring your deployment

After your deployment is complete, you can change your configuration in the Azure Portal by clicking on the "Configuration" item in the "Settings" section of the left pane found in the Semantic Kernel web app service page.

Scrolling down in that same pane to the "Monitoring" section gives you access to a multitude of ways to monitor your deployment.

In addition to this, the "Diagnose and solve problems" item near the top of the pane can yield crucial insight into some problems your deployment may be experiencing.

## How to clean up resources
When you want to clean up the resources from this deployment, use the Azure portal or run the following [Azure CLI](/cli/azure/) command:
```powershell
az group delete --name YOUR_RESOURCE_GROUP
```

## Deploy Chat Copilot to AzureML as an Online Endpoint

Advance your Semantic Kernel app by deploying to an AzureML Online Endpoint which helps to manage your real-time inferencing workload. 

### Prerequisites

* An Azure Machine Learning Workspace - [Create a new one](/azure/machine-learning/concept-workspace#create-a-workspace)
* Get an OpenAI API Key or create an [Azure OpenAI Deployment](/azure/ai-services/openai/how-to/create-resource?pivots=web-portal)

### Follow the sample notebook

In the [azureml-examples github repo](https://github.com/Azure/azureml-examples/blob/main/sdk/python/endpoints/online/llm/semantic-kernel/1_semantic_http_server.ipynb), fill in the sample notebook with your AzureML resources. Executing the notebook deploys a sample Semantic Kernel app to an Online Endpoint in your AzureML Workspace. The sample uses a custom container to deploy a Flask web server which exposes the Semantic Kernel app's API endpoints for inferencing. 

## Take the next step
>Learn [how to make changes to your Semantic Kernel web app](./publish-changes-to-azure.md), such as adding new skills.

>If you have not already done so, please star the GitHub repo and join the Semantic Kernel community! 
[Star the Semantic Kernel repo](https://github.com/microsoft/semantic-kernel)

[!INCLUDE [footer.md](../includes/footer.md)]
