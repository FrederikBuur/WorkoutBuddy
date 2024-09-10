@description('The name of the keyvault')
param keyvaultName string

@description('The location of the key vault')
param location string

@description('List of access policies')
param accessPolicies array

@description('List of secrets')
param secrets array = []


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
        secrets: accessPolicy.secretsPermissions
        certificates: accessPolicy.certificatesPermissions
      }
    }]
    
  }
}

// todo
// resource secret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = [for secret in secrets: {
//   name: secret.name
//   parent: keyVault
//   properties: {
//     value: secret.value
//   }
// }]

resource secret1 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'test-secret1'
  parent: keyVault
  properties: {
    value: 'super secret1'
  }
}

resource secret2 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'test-secret2'
  parent: keyVault
  properties: {
    value: 'super secret2 updated'
  }
}

resource secret3 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'test-secret3'
  parent: keyVault
  properties: {
    value: 'super secret3'
  }
}

resource secret4 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  name: 'test-secret4'
  parent: keyVault
  properties: {
    value: 'super secret4'
  }
}

