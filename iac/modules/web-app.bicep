@description('The name of the web app.')
param webAppName string

@description('The name of the app service plan')
param appServicePlanName string

@description('The connection string of the application insights to log to.')
param appInsightsConnectionString string

@description('Log analtyics workspace id for diagnostic settings.')
param logAnalyticsId string

@description('The id of the user assigned managed identity to use.')
param managedIdentityId string

@description('App settings configuration values.')
param appSettings object

@description('The location of the resource')
param location string

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
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

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    keyVaultReferenceIdentity: managedIdentityId
    clientAffinityEnabled: false
    siteConfig: {
      netFrameworkVersion: 'v7.0'
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      virtualApplications: [ // this should enable to deploy api and website to samme webapp
        {
          virtualPath: '/'
          physicalPath: 'site\\wwwroot'
        }
        {
          virtualPath: '/api'
          physicalPath: 'site\\api'
        }
      ]
    }
    
  }
}

module webAppSettings './appsettings.bicep' = {
  name: 'webAppSettings'
  params: {
    currentAppSettings: list('${webApp.id}/config/appsettings', '2021-02-01').properties
    appSettings: union(appSettings, {
      WEBSITE_RUN_FROM_PACKAGE: '1'
    })
    name: '${webApp.name}/appsettings'
  }
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'tologanaltyics'
  scope: webApp
  properties: {
    workspaceId: logAnalyticsId
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}


