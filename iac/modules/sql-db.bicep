param sqlServerName string
param sqlDbName string
param location string
param deploymentEnvironment string
param managedIdentityId string
param myAADId string

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  identity: {
    type:'UserAssigned'
    userAssignedIdentities:{
      '${managedIdentityId}': {}
    }
  }
  properties: {
    primaryUserAssignedIdentityId: managedIdentityId
    publicNetworkAccess: 'Disabled'
    restrictOutboundNetworkAccess: 'Disabled'
    administrators: {
      login: 'Frede Buur'
      sid: myAADId
      tenantId: tenant().tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  location: location
  identity: {
    type:'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  sku: {
    name: 'GP_S_Gen5_2'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 2
  }
  // kind: 'v12.0,user,vcore,serverless,freelimit'
  properties: {
    useFreeLimit: true
    freeLimitExhaustionBehavior: 'AutoPause'
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

output connectionString string = 'Server=tcp:${sqlServerName}.database.windows.net,1433;Initial Catalog=${sqlDbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
