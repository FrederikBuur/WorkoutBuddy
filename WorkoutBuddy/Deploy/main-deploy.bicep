
/*
 To build ARM template .json run:
 az bicep --file ./Deploy/main-deploy.bicep
*/

// parameters
@allowed(['dev', 'test', 'prod'])
param environment string
param location string = deployment().location
param appName string
param locationPrepend string = 'we'

// variables'
var rgName = 'rg-${environment}-${appName}-${locationPrepend}'

// scope
targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

module mainDeploy 'workoutbuddy-template.bicep' = {
  name: 'mainDeploy'
  scope: resourceGroup(rg.name)
  params: {
    environment: environment
    location: rg.location
    appName: appName
    locationPrepend: locationPrepend
  }
}
