Param(
[parameter(Mandatory=$true)]
$resourceGroupName,
[parameter(Mandatory=$true)]
$appLogContainerName,
[parameter(Mandatory=$true)]
$httpLogContainerName,
[parameter(Mandatory=$true)]
$tableName,
[parameter(Mandatory=$true)]
$webappName
)
	$azureModule=Get-Module -ListAvailable -Name AzureRM -Refresh
	$Message = "Executing script on Azure PowerShell Version: {0}.{1}.{2}" -f $azureModule.Version.Major, $azureModule.Version.Minor, $azureModule.Version.Build
	Write-Host $Message

	$azureModule2=Get-Module -ListAvailable -Name Azure -Refresh
	$Message2 = "Executing script on Azure PowerShell Version: {0}.{1}.{2}" -f $azureModule2.Version.Major, $azureModule2.Version.Minor, $azureModule2.Version.Build
	Write-Host $Message2
	
	Write-Host $resourceGroupName
	Write-Host $appLogContainerName
	Write-Host $httpLogContainerName
	Write-Host $tableName
	Write-Host $webappName
	
	Write-Host "going for Get-AzureRmResource"
	$webAppLogResource = Get-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceType Microsoft.Web/sites/config -ResourceName $webappName/logs -ApiVersion 2015-08-01
	
	Write-Host $webAppLogResource
	
	Write-Host "going for propertyObject"
	
	$propertyObject = $webAppLogResource.Properties
	
	Write-Host $propertyObject
	
	
	Write-Host "going for saas urls"
	
	Write-Host $propertyObject.applicationLogs
	Write-Host $propertyObject.applicationLogs.azureBlobStorage
	Write-Host $propertyObject.applicationLogs.azureTableStorage.sasUrl
	Write-Host $propertyObject.httpLogs.azureBlobStorage.sasUrl
	
	$appLogContainerSasUrl = $propertyObject.applicationLogs.azureBlobStorage.sasUrl
	$tableSasUrl = $propertyObject.applicationLogs.azureTableStorage.sasUrl
	$httpLogContainerSasUrl = $propertyObject.httpLogs.azureBlobStorage.sasUrl

	Write-Host "done saas urls processing"
	
	$storageAccountName = Get-AzureRmStorageAccount  -ResourceGroupName  $resourceGroupName
	$storageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName -Name $storageAccountName.StorageAccountName)[0].Value

	$context = New-AzureStorageContext  -StorageAccountName $storageAccountName.StorageAccountName -StorageAccountKey $storageAccountKey

	$appLogContainer = Get-AzureStorageContainer -Name $appLogContainerName -Context $context -ErrorAction SilentlyContinue
	$httpLogContainer = Get-AzureStorageContainer -Name $httpLogContainerName -Context $context -ErrorAction SilentlyContinue
	$storageTable = Get-AzureStorageTable -Name $tableName -Context $context -ErrorAction SilentlyContinue
	if($storageTable -eq $null)
	{
		$storageTable = New-AzureStorageTable -Name $tableName -Context $context
	}

	if($appLogContainer -eq $null)
	{
		$appLogContainer = New-AzureStorageContainer -Name $appLogContainerName -Context $context
	}

	if($httpLogContainer -eq $null)
	{
		$httpLogContainer = New-AzureStorageContainer -Name $httpLogContainerName  -Context $context
	}
	if($appLogContainerSasUrl.Length -eq 0)
	{
		$appLogContainerSasUrl = New-AzureStorageContainerSASToken -Name $appLogcontainer.Name -Context $context -Permission rwl -FullUri -ExpiryTime (Get-Date).AddDays(99999)   
	}
	if($tableSasUrl.Length -eq 0)
	{
		$tableSasUrl = New-AzureStorageTableSASToken -Name $storageTable.Name -Context $context -Permission rau -FullUri -ExpiryTime (Get-Date).AddDays(99999)
	}
	if($httpLogContainerSasUrl.Length -eq 0)
	{
		$httpLogContainerSasUrl = New-AzureStorageContainerSASToken -Name $httpLogContainer.Name -Context $context -Permission rwl -FullUri -ExpiryTime (Get-Date).AddDays(99999)
	}
	
		$propertyObject.applicationLogs.azureTableStorage.level = "Error"		
		$propertyObject.applicationLogs.azureTableStorage.sasUrl = "$tableSasUrl"
		$propertyObject.applicationLogs.azureBlobStorage.level = "Information"
		$propertyObject.applicationLogs.azureBlobStorage.sasUrl = "$appLogContainerSasUrl"
		$propertyObject.applicationLogs.azureBlobStorage.retentionInDays = 90
		$propertyObject.httpLogs.azureBlobStorage=[pscustomobject]@{
                                                    sasUrl = "$httpLogContainerSasUrl"
                                                    retentionInDays = 90 
                                                    enabled=$true
                                                    }
		$propertyObject.failedRequestsTracing.enabled = $true
		$propertyObject.detailedErrorMessages.enabled = $true
	
	Set-AzureRmResource -PropertyObject $propertyObject -ResourceGroupName $resourceGroupName -ResourceType Microsoft.Web/sites/config `
								-ResourceName $webappName/logs -ApiVersion 2015-08-01 -Force
	