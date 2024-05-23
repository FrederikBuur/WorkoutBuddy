## To enable pipelines

- Manually created app registration in Entra/AAD
- created admin group in Entra/AAD added myself, the app registration/service principal and managed identity. used in iac for sql server
- created resource group, iac pipeline needs RG to be created beforehand
- manually needs to set firebase secrets in kv
- in devops create service connection witch points to app registration. us this in ci/cd pipeline (azureSubscription)
- manually added admin group as contributer to web app. otherwise pipelin cant deploy code
- manually added admin group as contributer to subscription. otherwise pipeline cant deploy code

if deploy pipeline gives sql connection error. run again. sql is free tier so might need to warm up
