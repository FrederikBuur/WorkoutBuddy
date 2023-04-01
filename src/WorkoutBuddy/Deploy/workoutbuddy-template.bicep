
// parameters
@allowed(['dev', 'test', 'prod'])
param environment string
param appName string
param location string = resourceGroup().location
param locationPrepend string

// variables
var identityName = 'id-${environment}-${appName}-${locationPrepend}'
var appInsightName = 'appi-${environment}-${appName}-${locationPrepend}'
var planName = 'plan-${environment}-${appName}-${locationPrepend}'
var appServiceName = 'as-${environment}-${appName}-${locationPrepend}'
var keyvaultName = 'kv-${environment}-${appName}-${locationPrepend}'
var cosmosDbAccountName = 'cosdb-${environment}-${appName}-${locationPrepend}'
var cosmosDbName = '${appName}-${environment}'

// user assinged managed identity
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName
  location: location
}

// application insight
resource appInsight 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// hosting plan / server farm
resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: planName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
  kind: 'app'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
}

// app service
resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { 
      '${managedIdentity.id}' : {}
    }
  }
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [ // inject app settings
        {
          name: 'SomeConfig'
          value: 'some-config-value'
        }
      ]
    }
  }
}

// keyvault
resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyvaultName
  location: location
  properties: {
    enabledForDeployment: false
    enabledForTemplateDeployment: false
    enabledForDiskEncryption: false
    softDeleteRetentionInDays: 90
    tenantId: tenant().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: [
      {
        tenantId: tenant().tenantId
        objectId: managedIdentity.properties.principalId
        permissions: {
          keys: [
            'get'
            'list'
          ]
          secrets: [
            'get'
            'list'
          ]
          certificates: [
          ]
        }
      }
      {
        tenantId: tenant().tenantId
        objectId: 'a4d2de89-ed7b-4762-b315-6a8d3d0a7b8f' // id of me in AAD
        permissions: {
          keys: [
            'all'
          ]
          secrets: [
            'all'
          ]
          certificates: [
            'all'
          ]
        }
      }
    ]
  }
}

// cosmos db account
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    enableFreeTier: true //environment == 'dev'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Eventual'
      maxStalenessPrefix: 1
      maxIntervalInSeconds: 5
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

// cosmos database
resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  name: cosmosDbName
  parent: cosmosDbAccount
  properties: {
    resource: {
      id: cosmosDbName
    }
    options: {
      throughput: 1000 // limit for free tier
    }
  }
}

