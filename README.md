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
1. Fill in the name of the application, Service for example and keep the other fields to their defauls and press **Register**
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
* Application (clien) ID
* Application ID URI

### Create the *Consumer* application
