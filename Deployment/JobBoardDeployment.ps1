# Variables
$resourceGroup = 'rg-blazjobboard'
$location = 'southeastasia'

$appInsightName = 'appi-blazjobboard'

$funcStorageAccountName = 'stfuncblazjobboard001'
$apiFunctionName = 'func-blazjobboardapi-prod'
$proxyFunctionName = 'blazjobboard'

$webappStorageAccountName = 'stblazjobboardweb001'

$cosmosAccountName = 'cosmos-account-blazjobboard'
$cosmosDbName = 'cosmos-blazjobboard-prod'
$cosmosContainerName = 'jobs'


# resource group
Write-Host "Creating Resource Group: $resourceGroup"
$resGroup = az group create --name $resourceGroup --location $location
Write-Host "Finished"

#set defaults
az configure --defaults location=$location group=$resourceGroup

#application insights
Write-Host "Creating Application Insights: $appInsightName"
$ext = az extension add -n application-insights
$appInsightsInstKey = az monitor app-insights component create --app $appInsightName --kind web --application-type web --query instrumentationKey
Write-Host "App Insights Inst Key: $appInsightsInstKey"
Write-Host "Finished"

# cosmos db
Write-Host "Creating Cosmos Account: $cosmosAccountName"
$cosmosEndPoint = az cosmosdb create -n $cosmosAccountName --default-consistency-level Eventual --query documentEndpoint
Write-Host "Cosmos Endpoint: $cosmosEndPoint"
Write-Host "Creating Database: $cosmosDbName"
$comosDb=az cosmosdb sql database create -a $cosmosAccountName -n $cosmosDbName
Write-Host "Creating Container: $cosmosContainerName"
$cosmosContainer=az cosmosdb sql container create -a $cosmosAccountName d $cosmosDbName -n $cosmosContainerName -p '/id' --throughput 400 
$cosmosPrimaryKey=az cosmosdb keys list --name $cosmosAccountName -g $resourceGroup --query primaryMasterKey
Write-Host "Cosmos Primary Key: $cosmosPrimaryKey"
Write-Host "Finished"

#function app
Write-Host "Creating Storage Account for Functions"
$saFuncAccount = az storage account create -n $funcStorageAccountName --sku Standard_LRS --kind StorageV2
Write-Host "Finished"
Write-Host "Creating Function App " $apiFunctionName 
$serverHostName = az functionapp create --consumption-plan-location $location --name $apiFunctionName  --os-type Windows --runtime dotnet --storage-account $funcStorageAccountName --app-insights-key $appInsightsInstKey --query defaultHostName 
$serverHostName = "https://"+$serverHostName+"/"
Write-Host "Server Host Name: $serverHostName"
Write-Host "Finished"
Write-Host "Updating $apiFunctionName Configuration"
$settings=az functionapp config appsettings set --name $apiFunctionName --resource-group $resourceGroup --settings "FUNCTIONS_EXTENSION_VERSION=~3"
$settings=az functionapp config appsettings set --name $apiFunctionName --resource-group $resourceGroup --settings "CosmosConnectionString=AccountEndpoint=$cosmosEndPoint;AccountKey=$cosmosPrimaryKey;"
$settings=az functionapp config appsettings set --name $apiFunctionName --settings "DatabaseName=$cosmosDbName"
$settings=az functionapp config appsettings set --name $apiFunctionName --settings "ContainerName=$cosmosContainerName"
Write-Host "Finished"
Write-Host "Creating Function App " $proxyFunctionName
$funcAppProxy = az functionapp create --consumption-plan-location $location --name $proxyFunctionName --os-type Windows --runtime dotnet --storage-account $funcStorageAccountName --app-insights-key $appInsightsInstKey
Write-Host "Finished"

#web app
Write-Host "Creating Storage Account for Web App"
$webEndPoint = az storage account create -n $webappStorageAccountName --sku Standard_LRS --kind StorageV2 --query primaryEndpoints.web
Write-Host "Web Endpoint $webEndPoint"
$webStorageAccountUpdate = az storage blob service-properties update --account-name $webappStorageAccountName --static-website --404-document index.html --index-document index.html 
Write-Host "Finished"

#update proxy configuration
Write-Host "Updating Proxy Configuration"
$settings=az functionapp config appsettings set --name $proxyFunctionName  --settings "FUNCTIONS_EXTENSION_VERSION=~3"
$settings=az functionapp config appsettings set --name $proxyFunctionName  --settings "JOBBOARD_CLIENT_URI=$webEndPoint"
$settings=az functionapp config appsettings set --name $proxyFunctionName --settings "JOBBOARD_API_URI=$serverHostName"
Write-Host "Finished"