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
