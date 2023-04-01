/*
 To build ARM template .json run:
 az bicep --file ./Deploy/main-resources.bicep
*/

// parameters
@description('The name of the environment being deployed to.')
@allowed([
  'devtest'
  'stable'
  'prod'
])
param deploymentEnvironment string

@description('The location for the resources')
param location string = resourceGroup().location

// variables
var appName = 'workoutbuddy' 
var appInsightsName = 'appi-${appName}-${deploymentEnvironment}-we-fb'
var logAnaltyicsName = 'la-${appName}-${deploymentEnvironment}-we-fb'
var storageAccountName = 'st${appName}${deploymentEnvironment}wefb'
var identityName = 'id-${appName}-${deploymentEnvironment}-we-fb'
var keyvaultName = 'kv-${appName}-${deploymentEnvironment}-we-fb'
var databaseName = '${appName}-${deploymentEnvironment}'
var webAppName = 'app-${appName}-${deploymentEnvironment}-we-fb'

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
      'test-container-1' 
      'another-container'
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
        objectId: 'a4d2de89-ed7b-4762-b315-6a8d3d0a7b8f' // id of me in AAD
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
