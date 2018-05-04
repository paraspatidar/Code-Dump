#Login-AzureRmAccount
 

$siteName = 'WebAppAXXXXousAccess'
$rgGroup = 'WebAppAnxXXXXccess' 
$sub = '6224b7d2-XXXXXe03ca1fa1' 

$webSiteInstances = @()

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
                
               
            }            
       }
    }
} 
