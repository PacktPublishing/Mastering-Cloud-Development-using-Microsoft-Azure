##############################################################
#
# PREREQUISITES
#
##############################################################

#authenticate to Azure in service manager mode
Add-AzureAccount #perform login
Get-AzureSubscription #get a list of the subscriptions usable by login
Select-AzureSubscription -SubscriptionName "<put your subscription name>"

#authenticate to Azure in resource manager mode
Login-AzureRmAccount #to login to the default subscription
Login-AzureRmAccount -SubscriptionId #<put you subscriptionId>
 
#create new resource group if not already created
New-AzureRmResourceGroup -Name "CloudMakers" -Location NorthEurope