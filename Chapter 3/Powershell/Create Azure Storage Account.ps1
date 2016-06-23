##############################################################
#
# Create Azure Storage Account
#
##############################################################


Get-AzureLocation | format-Table -Property Name, AvailableServices, StorageAccountTypes

# Create a new storage account.
New-AzureStorageAccount –StorageAccountName "cmadventureworks" -Location "West Europe"

# Set a default storage account.
Set-AzureSubscription –StorageAccountName "cmadventureworks" -CurrentStorageAccountName $StorageAccountName -SubscriptionName $SubscriptionName

$storageAccount = Get-AzureStorageAccount -StorageAccountName "cmadventureworks"

# Create a new container.
New-AzureStorageContainer -Context $storageAccount.Context -Name "webfrontendimages" -Permission Off

$storageAccountContext.StorageAccountStatus
$storageAccountContext.Context.

$storageAccountName = "cmadventureworks"
$accountKey = (Get-AzureStorageKey –StorageAccountName $storageAccountName).Primary

$blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=" + $storageAccountName + ";AccountKey=" + $accountKey + ";BlobEndpoint=http://" + $storageAccountName + ".blob.core.windows.net;"


$webfrontendimages = Get-AzureStorageContainer -Context $storageAccountContext.Context -Name webfrontendimages

Set-AzureStorageContainerAcl -Context $storageAccount.Context -Name webfrontendimages -Permission Container

$storageAccount.Context.BlobEndPoint

$webfrontendimages.PublicAccess

$webfrontendimages.ToString

#create SAS Token
New-AzureStorageContainerSASToken -FullUri -Container webfrontendimages -Context $storageAccount.Context

$sasToken = New-AzureStorageContainerSASToken -Container webfrontendimages -Permission r -Context $storageAccount.Context

Set-AzureStorageContainerStoredAccessPolicy -Context $storageAccount.Context -Container webfrontendimages

New-AzureStorageBlobStoredAccessPolicy -Name webfrontendimages -Policy webfrontendimages_read -Permission "rd" -StartTime "2015-01-01" -ExpiryTime "2025-01-01" -Context $storageAccount.Context
$webfrontendimages_read = New-AzureStorageBlobSASToken -Name webfrontendimages -Policy webfrontendimages_read -Context $storageAccount.Context

Set-AzureStorageContainerAcl -Context $storageAccount.Context -Permission

$storageAccount = Get-AzureStorageAccount -StorageAccountName "cmadventureworks"
$sasToken = New-AzureStorageContainerSASToken -Context $storageAccount.Context -Container webfrontendimages -Permission rl
$sasToken

$storageAccount = Get-AzureStorageAccount -StorageAccountName "cmadventureworks"

$container = Get-AzureStorageContainer -Context $storageAccount.Context -Name webfrontendimages
$cbc = $container.CloudBlobContainer

# Sets up a Stored Access Policy and a Shared Access Signature for the new container
#
$permissions = $cbc.GetPermissions();
$policyName = 'webfrontendimages_read'
$policy = new-object 'Microsoft.WindowsAzure.Storage.Blob.SharedAccessBlobPolicy'
$policy.SharedAccessStartTime = $(Get-Date).ToUniversalTime().AddMinutes(-5)
$policy.SharedAccessExpiryTime = $(Get-Date).ToUniversalTime().AddYears(10)
$policy.Permissions = "Read"
$permissions.SharedAccessPolicies.Add($policyName, $policy)
$cbc.SetPermissions($permissions);
