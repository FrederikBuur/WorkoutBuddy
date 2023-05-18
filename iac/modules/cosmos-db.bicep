param cosmosDbAccountName string 
param cosmosDbName string 
param location string
param deploymentEnvironment string
param keyVaultName string
param managedIdentityId string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: cosmosDbAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    enableFreeTier: deploymentEnvironment == 'dev'
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
  // todo kan godt lade sig gøre at sætte cosmos op med managed identity?
  // https://dev.to/willvelida/using-managed-identities-to-authenticate-with-azure-cosmos-db-23ga
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
}

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

// todo cant add connectionstring/keys from cosmos to keyvault
// resource secret3 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
//   name: '${keyVaultName}/Cosmos_ConnectionString'
//   properties: {
//     value: 'AccountEndpoint=https://${cosmosDbAccountName}.documents.azure.com:443/;AccountKey=${cosmosDbAccount.listKeys().primaryMasterKey};'
//   }
// }
