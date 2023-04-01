@description('The name of the blob service.')
param storageAccountName string

@description('The name of the blob container.')
param containerName string

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${storageAccountName}/default/${containerName}'
  properties: {}
}
