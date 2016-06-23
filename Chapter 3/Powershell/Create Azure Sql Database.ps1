##############################################################
#
# Create Azure SQL Database
#
##############################################################


#create a new server
$credentials = Get-Credential #prompts a UI to retrieve a username and a password
$server = New-AzureRmSqlServer -ResourceGroupName CloudMakers -ServerName cloudmakers04 -SqlAdministratorCredentials $credentials -Location "West Europe" -ServerVersion "12.0"
#get an existing server
$server = Get-AzureRmSqlServer -ResourceGroupName CloudMakers -ServerName cloudmakers03

#get the database
$database = Get-AzureRmSqlDatabase -ServerName cloudmakers03 -ResourceGroupName CLoudMakers

$database = New-AzureRmSqlDatabase -ResourceGroupName CloudMakers -ServerName cloudmakers03 -DatabaseName AdventureWorksLT2 -Edition Basic

$currentIP = (New-Object net.webclient).downloadstring("http://checkip.dyndns.com") -replace "[^\d\.]"
If ((Get-AzureRmSqlServerFirewallRule -ResourceGroupName CloudMakers -ServerName cloudmakers03 -FirewallRuleName "OnPremise") -eq $null) {
	New-AzureRmSqlServerFirewallRule -ResourceGroupName CloudMakers -ServerName cloudmakers03 -FirewallRuleName "OnPremise" -StartIpAddress $currentIP -EndIpAddress $currentIP
}
else {
	Set-AzureRmSqlServerFirewallRule -ResourceGroupName CloudMakers -ServerName cloudmakers03 -FirewallRuleName "OnPremise" -StartIpAddress $currentIP -EndIpAddress $currentIP
}
