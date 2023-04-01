@description('The name of the keyvault')
param keyvaultName string

@description('The location of the key vault')
param location string

@description('List of access policies')
param accessPolicies array


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
    accessPolicies: [for accessPolicy in accessPolicies: {
      objectId: accessPolicy.objectId
      tenantId: tenant().tenantId
      permissions: {
        keys: accessPolicy.keysPermissions
        secrets: accessPolicy.secretsPermiossions
        certificates: accessPolicy.certificatesPermissions
      }
    }]
  }
}
