# AzureADAuth

Example of a Consumer API that consults a downstream API, called Service, to return data. The Consumer API is 
anonymously accessible yet the Service API required AD authentication. In this example the Consumer authenticates
as itself (using client credentials grant) and with the acquired token calls the Service API. 

## How to run this example

We need to register two applications within an Azure AD Tenant. For this example it is the best option to create
your own Azure AD Tenant to be used. In the overview of the Azure AD Tenant please record the:
* domain (e.g. contose.onmicrosoft.com)
* tenant id (a guid)

### Create the *Service* application
1. In the Azure AD Tenant go to the section called **App registrations**
1. Click on the button **+ New registration**
1. Fill in the name of the application, **Service** for example and keep the other fields to their defauls and press **Register**
1. In the overview screen of the application press on the **Add an Application ID URI** at the top right section
1. In the top of the screen press the **Set** button and press **Save** in the diaglog
1. Go to the section **Manifest** in the left hand menu
1. Locate the *appRoles* attribute in the JSON and replace it with the following:
```javascript
"appRoles": [
  {
    "allowedMemberTypes": [
      "Application"
    ],
    "description": "Apps in this role can consume the web api.",
    "displayName": "ReaderRole",
    "id": "7489c77e-0f34-4fe9-bf84-0ce8b74a03c4",
    "isEnabled": true,
    "lang": null,
    "origin": "Application",
    "value": "ReaderRole"
  }
],
```
Finally go to the overview of the application and record the following information:
* *Application (client) ID*
* *Application ID URI*

### Create the *Consumer* application
1. Go back to the Azure AD Tenant and go to the section called **App registrations**
1. Click on the button **+ New registration**
1. Fill in the name of the application, **Consumer** for example and keep the other fields to their defauls and press **Register**
1. Register the *Application (client) ID* of the application
1. Go to the **Certificates & secrets** section of the application
1. Under the *Client secrets* press on the button **+ New client secret** to add a new client secret
    1. Give it a description, for example *client secret*
    1. Put *never* for the *Expires* section
    1. Click on the **Add** button
1. After adding the secret it is displayed as plain text, this is the moment to copy it and record it
1. Go to the **API permissions** section of the application
1. Click on the **+ Add a permission** button
1. In the dialog select the *My APIs* section
1. Select the API of the *Service* application registered before and select the *ReaderRole* permission
1. Click the **Add permissions** button in the dialog
1. Now the application has the permission, but it still needs to be consented. Do this by clicking on the **Grant admin consent for [tenant]** button

### Create a KeyVault
1. Create a KeyVault within Azure and record its *DNS Name*
1. Ensure under the **Access policies** section ensure that you, as a user, have all permission on the KeyVault
1. Go to the **Secrets** session of the KeyVault
1. Click on the **+ Generate/Import** 
1. Fill in the following information in the dialog
    1. **Upload options**: Manual
    1. **Name**: consumerSecret
    1. **Value**: the recorded client secret of the *Consumer* application registered before
    1. Leave the rest as default
1. Click the **Create** button to store the secret

### Configure the *Service* project
1. Open the *appsettings.json* file
1. Fill in the recorded values under the *AzureAD* section in the JSON:
    1. **Domain**: The domain name of your Azure AD Tenant
    1. **TenantId**: The tenant id of your Azure AD Tenant
    1. **ClientId**: The Application (client) ID of the registered *Service* application
    1. **AppIDUri**: The Application ID URI of the registered *Service* application

### Configure the *Consumer* project
1. Open the *appsettings.json* file
1. Fill in the recorded values under the *AzureAd* section in the JSON:
    1. **Domain**: The domain name of your Azure AD Tenant
    1. **TenantId**: The tenant id of your Azure AD Tenant
    1. **ClientId**: The Application (client) ID of the registered *Service* application
1. Fill in the recorded values under the *Service.AzureAD* section in the JSON:
    1. **AppIdScope**: Insert the Application (Client) ID of the *Service* application
1. Fill in the recorded values un der the *KeyVault* section in the JSON:
    1. **Url**: The recorded *DNS Name* of your KeyVault created before
    
### Run the Solution
1. Configure the Solution to start both the *Consumer* and *Service* solution
1. Go to the following URL to see the response of the *Consumer* API: **https://localhost/44384/api/consumer
