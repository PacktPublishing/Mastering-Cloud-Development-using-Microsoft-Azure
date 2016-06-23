######################################################################
#
# Configuring a Resource Mode Virtual Network for the frontend servers
#
######################################################################

#create virtual network
$vn = New-AzureRmVirtualNetwork -ResourceGroupName "CloudMakers" -Name "CloudMakers" -AddressPrefix 192.168.0.0/16 -Location "NorthEurope"
#get a virtual network
$vn = Get-AzureRmVirtualNetwork -ResourceGroupName "CloudMakers" -Name "CloudMakers"

#Subnets
$webServerSN = Add-AzureRmVirtualNetworkSubnetConfig -Name "WebServer" -VirtualNetwork $vn -AddressPrefix 192.168.1.0/24
$webServerSN = Get-AzureRmVirtualNetworkSubnetConfig -Name "WebServer" -VirtualNetwork $vn
$dataServerSN = Add-AzureRmVirtualNetworkSubnetConfig -Name "DataServer" -VirtualNetwork $vn -AddressPrefix 192.168.2.0/24
$dataServerSN = Get-AzureRmVirtualNetworkSubnetConfig -Name "DataServer" -VirtualNetwork $vn
Set-AzureRmVirtualNetwork -VirtualNetwork $vn

#Define WebServer Network Security Group
$WebServerNSG = New-AzureRmNetworkSecurityGroup -Name "WebServer" -location "NorthEurope" -ResourceGroupName "CloudMakers"
$WebServerNSG = Get-AzureRmNetworkSecurityGroup -Name "WebServer" -ResourceGroupName "CloudMakers" 
$httpIn = New-AzureRmNetworkSecurityRuleConfig -Name HttpIn -DestinationPortRange 80 -Direction Inbound -Protocol Tcp -SourcePortRange * -SourceAddressPrefix * -DestinationAddressPrefix * -Access Allow -Priority 1000
$WebServerNSG.SecurityRules.Add($httpIn)
$rdpIn = New-AzureRmNetworkSecurityRuleConfig -Name RdpIn -DestinationPortRange 3389 -Direction Inbound -Protocol Tcp -SourcePortRange * -SourceAddressPrefix * -DestinationAddressPrefix * -Access Allow -Priority 1100
$WebServerNSG.SecurityRules.Add($rdpIn)
Set-AzureRmNetworkSecurityGroup -NetworkSecurityGroup $WebServerNSG

#Define DataServer Network Security Group
$DataServerNSG = New-AzureRmNetworkSecurityGroup -Name "DataServer" -location "NorthEurope" -ResourceGroupName "CloudMakers"
$DataServerNSG = Get-AzureRmNetworkSecurityGroup -Name "DataServer" -ResourceGroupName "CloudMakers" 
$rdpIn = New-AzureRmNetworkSecurityRuleConfig -Name RdpIn -DestinationPortRange 3389 -Direction Inbound -Protocol Tcp -SourcePortRange * -SourceAddressPrefix * -DestinationAddressPrefix * -Access Allow -Priority 1100
$DataServerNSG.SecurityRules.Add($rdpIn)
Set-AzureRmNetworkSecurityGroup -NetworkSecurityGroup $DataServerNSG

#associate WebServer NSG to Virtual Network
$WebServerSN = Get-AzureRmVirtualNetworkSubnetConfig -Name "WebServer" -VirtualNetwork $vn
$WebServerSN.NetworkSecurityGroup  = $WebServerNSG
Set-AzureRmVirtualNetwork -VirtualNetwork $vn

#associate DataServer NSG to Virtual Network
$DataServerSN = Get-AzureRmVirtualNetworkSubnetConfig -Name "DataServer" -VirtualNetwork $vn
$DataServerSN.NetworkSecurityGroup  = $DataServerNSG
Set-AzureRmVirtualNetwork -VirtualNetwork $vn