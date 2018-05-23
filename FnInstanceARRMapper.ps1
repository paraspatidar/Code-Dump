# POST method: $req
$requestBody = Get-Content $req -Raw | ConvertFrom-Json
$name = $requestBody.name

# this will support get and url will look like 
# ex : https://functionproxyresponce.azurewebsites.net/api/HttpTriggerPowerShell1?RG=WebAppAnonomousAccess&SiteRG=WebAppAnonomousAccess
# which is : https://<functionAppName>.azurewebsites.net/api/<HttpTrigger_Powershell_Function_Name>?RG=<WebApp_Resource_Group_Name>&Site=<WebApp_Name>

# GET method: each querystring parameter is its own variable
#thus in URL write the resource group name in RG parameter and sitename in Site parameter
# so url will look like :  /api/<Powershell_Function_Name>?RG={myRG}&Site={mySite}
# Note site name will not contains .azurewebsites.net , it will be just name on app service 

if ($req_query_RG) 
{
    $rgGroup = $req_query_RG 
}

if ($req_query_Site) 
{
    $siteName = $req_query_Site 
}


'Resource Group is ' + $rgGroup
'Site is ' +  $siteName

'Reading App Settings'
# read the application settings
$sub = $ENV:APPSETTING_SubscriptionId
$tenentId = $ENV:APPSETTING_TenantID
$clientId = $ENV:APPSETTING_AADApplicationId
$password = $ENV:APPSETTING_AADApplicationKey


$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force

$mycreds = New-Object System.Management.Automation.PSCredential ($clientId, $secpasswd)

'Authenticating Using Service Principal'

Add-AzureRmAccount -ServicePrincipal -Tenant $tenentId -Credential $mycreds
Select-AzureRmSubscription -SubscriptionId $sub


$webSiteInstances = @()
$ResponceValue = @{}

'Getting AAR affinity List'
#This gives you list of instances
$webSiteInstances = Get-AzureRmResource -ResourceGroupName $rgGroup -ResourceType Microsoft.Web/sites/instances -ResourceName $siteName -ApiVersion 2015-11-01 
 

foreach ($instance in $webSiteInstances)
{
    $instanceId = $instance.Name
        
    # This gives you list of processes running
    # on a particular instance
    $processList =  Get-AzureRmResource `
                    -ResourceId /subscriptions/$sub/resourceGroups/$rgGroup/providers/Microsoft.Web/sites/$sitename/instances/$instanceId/processes `
                    -ApiVersion 2015-08-01 
 
    foreach ($process in $processList)
    {               
        if ($process.Properties.Name -eq "w3wp")
        {            
            $resourceId = "/subscriptions/$sub/resourceGroups/$rgGroup/providers/Microsoft.Web/sites/$sitename/instances/$instanceId/processes/" + $process.Properties.Id            
            $processInfoJson = Get-AzureRmResource -ResourceId  $resourceId  -ApiVersion 2015-08-01                                     
 
            # is_scm_site is a property which is set
            # on the worker process for the KUDU 
            if ($processInfoJson.Properties.is_scm_site -ne $true)
            {
                $computerName = $processInfoJson.Properties.Environment_variables.COMPUTERNAME
                "Instance ID  " + $instanceId  + " is for " +   $computerName
                
               $ResponceValue.Add($computerName,$instanceId)
               
            }            
       }
    }

    
}

$newValue=ConvertTo-Json -InputObject $ResponceValue

Out-File -Encoding Ascii -FilePath $res -inputObject  $newValue