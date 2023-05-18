param currentAppSettings object
param appSettings object
param name string

resource siteConfig 'Microsoft.Web/sites/config@2021-02-01' = {
  name: name
  properties: union(currentAppSettings, appSettings)
}
