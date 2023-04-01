@description('The name of the storage account')
param storageAccountName string

@description('The location of the storage account')
param location string

@description('String array of blob containers that should have this rule applied.')
param containerNames array

@description('The name of the policy.')
param policyName string

@description('Blobs older than this number of days will be deleted.')
param blobTimeToLiveInDays int

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name:'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    // supportsHttpsTrafficOnly: true
    // allowSharedKeyAccess: false
    // defaultToOAuthAuthentication: true
    // sasPolicy: {
    //   expirationAction: 'Log'
    //   sasExpirationPeriod: '00.00:15:00'
    // }
    // encryption: {
    //   services: {
    //     file: {
    //       keyType: 'Account'
    //       enabled: true
    //     }
    //     blob: {
    //       keyType: 'Account'
    //       enabled: true
    //     }
    //   }
    //   keySource: 'Microsoft.Storage'
    // }
    // accessTier: 'Hot'
    // allowBlobPublicAccess: false
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2019-06-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource blobPolicy 'Microsoft.Storage/storageAccounts/managementPolicies@2019-06-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    policy: {
      rules: [
        {
          enabled: true
          name: policyName
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                delete: {
                  daysAfterModificationGreaterThan: blobTimeToLiveInDays
                }
              }
              snapshot: {
                delete: {
                  daysAfterCreationGreaterThan: blobTimeToLiveInDays
                }
              }
              version: {
                delete: {
                  daysAfterCreationGreaterThan: blobTimeToLiveInDays
                }
              }
            }
            filters: {
              blobTypes: [
                'blockBlob'
                'appendBlob'
              ]
              prefixMatch: containerNames
            }
          }
        }
      ]
    }
  }
}

module containers 'storage-account-blob-container.bicep'= [for containerName in containerNames: {
  name: containerName
  dependsOn:[
    storageAccount
  ]
  params:{
    containerName: containerName
    storageAccountName: storageAccountName
  }
}]



output storageAccountName string = storageAccount.name
output resourceId string = storageAccount.id
output primaryEndpoints object = storageAccount.properties.primaryEndpoints
