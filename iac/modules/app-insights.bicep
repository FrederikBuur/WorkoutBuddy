@description('The name of the application insights resource.')
param appInsightsName string

@description('The name of the log analtyics workspace.')
param logAnalyticsName string

@description('The location of the resource')
param location string

// application insight
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

output instrumentationKey string = appInsights.properties.InstrumentationKey
output resourceId string = appInsights.id
output logAnalyticsId string = workspace.id
