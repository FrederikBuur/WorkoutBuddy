/*
 To build ARM template .json run:
 az bicep --file ./Deploy/main-resources.bicep
*/

// parameters
@description('The name of the environment being deployed to.')
@allowed([
  'staging'
  'prod'
])
param deploymentEnvironment string

@description('The location for the resources')
param location string = resourceGroup().location

// variables
var appName = 'workoutbuddy' 
var appInsightsName = 'appi-${appName}-${deploymentEnvironment}-we'
var logAnaltyicsName = 'la-${appName}-${deploymentEnvironment}-we'
var appServicePlanName = 'asp-${appName}-${deploymentEnvironment}-we'
var storageAccountName = 'st${appName}${deploymentEnvironment}we'
var containerName1 = 'test-container-1' 
var containerName2 = 'another-container'
var identityName = 'id-${appName}-${deploymentEnvironment}-we'
var keyvaultName = 'kv-${appName}-${deploymentEnvironment == 'staging' ? 'sta' : deploymentEnvironment}-we' // must bee between 3-24
// var cosmosDbAccountName = 'cosdb-${appName}-${deploymentEnvironment}-we'
// var cosmosDbName = '${appName}-${deploymentEnvironment}'
var sqlServerName = 'sql-server-${appName}-${deploymentEnvironment}-we'
var sqlDbName = 'sql-db-${appName}-${deploymentEnvironment}-we'
var webAppName = 'app-${appName}-${deploymentEnvironment}-we'

var myAADId = 'a4d2de89-ed7b-4762-b315-6a8d3d0a7b8f' // id of me in AAD HARDCODED
var adminGroupId = '86ad9283-4097-4793-87fd-7462339d4371' // id of group WorkoutBuddyAdmins HARDCODED

// resources
module appInsights 'modules/app-insights.bicep' = {
  name: 'appInsights'
  params: {
    appInsightsName: appInsightsName
    logAnalyticsName: logAnaltyicsName
    location: location
  }
}

module storageAccount 'modules/storage-account.bicep' = {
  name: 'storageAccount'
  params:{
    location: location
    containerNames: [
      containerName1
      containerName2
    ]
    blobTimeToLiveInDays: 1
    policyName: 'deleteOldBlobs'
    storageAccountName: storageAccountName
  }
}

module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params:{
    keyvaultName: keyvaultName
    location: location
    accessPolicies: [
      {
        objectId: myAADId
        keysPermissions: ['all']
        secretsPermissions: ['all']
        certificatesPermissions: ['all']
      }
      {
        objectId: identity.properties.principalId
        keysPermissions: ['get', 'list']
        secretsPermissions: ['get', 'list']
        certificatesPermissions: ['get', 'list']
      }
    ]
  }
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  location: location
  name: identityName
}

// module cosmosDb 'modules/cosmos-db.bicep' = {
//   name: 'cosmosDb'
//   dependsOn: [
//     keyvault
//   ]
//   params:{
//     location: location
//     cosmosDbAccountName: cosmosDbAccountName
//     cosmosDbName: cosmosDbName
//     deploymentEnvironment: deploymentEnvironment
//     keyVaultName: keyvaultName
//     managedIdentityId: identity.id
//   }
// }

module sqlDb 'modules/sql-db.bicep' = {
  name: 'sqlDb'
  params:{
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
    location: location
    deploymentEnvironment: deploymentEnvironment
    managedIdentityId: identity.id
    adminId: adminGroupId
  }
}

module webApp 'modules/web-app.bicep' = {
  name: 'webApp'
  params: {
    webAppName: webAppName
    location: location
    appServicePlanName: appServicePlanName
    appInsightsConnectionString: appInsights.outputs.connectionString
    logAnalyticsId: appInsights.outputs.logAnalyticsId
    managedIdentityId: identity.id
    appSettings: {
      WEBSITE_NODE_DEFAULT_VERSION: 20
      AZURE_CLIENT_ID: identity.properties.clientId
      APPLICATIONINSIGHTS_CONNECTION_STRING: appInsights.outputs.connectionString
      //APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.outputs.instrumentationKey // use connectionstring instead
      ASPNETCORE_ENVIRONMENT: deploymentEnvironment == 'staging' ? 'Staging' : deploymentEnvironment == 'prod' ? 'Production' : 'unsupported-env-${deploymentEnvironment}'
      Blob__ServiceUri: storageAccount.outputs.primaryEndpoints.blob
      Blob__TestContainer1: containerName1
      Blob__TestContainer2: containerName2
      KeyVault__Url: 'https://${keyvaultName}${environment().suffixes.keyvaultDns}'
      ConnectionStrings__SQL: sqlDb.outputs.connectionString
      // 'Cosmos__DbName': cosmosDbName
      // 'Cosmos__Uri': 'https://${cosmosDbAccountName}.documents.azure.com:443/'
      // PUBLIC_SUPABASE_URL: 'https://iwejadjjejamlxztxsbs.supabase.co'
      // PUBLIC_SUPABASE_ANON_KEY: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Iml3ZWphZGpqZWphbWx4enR4c2JzIiwicm9sZSI6ImFub24iLCJpYXQiOjE2NzQ2Nzc5ODcsImV4cCI6MTk5MDI1Mzk4N30.aLqD7SEJqKz0nOL5Yvh_hKW-kqnm1DNT3TvP_y6uUo8'
    }
  }
}
