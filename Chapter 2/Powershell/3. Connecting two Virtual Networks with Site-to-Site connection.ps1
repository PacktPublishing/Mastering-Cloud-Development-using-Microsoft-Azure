##############################################################
#
# Connecting two Virtual Networks with Site-to-Site connection
#
##############################################################

#create a local network gateway to CloudMakersClassic
$CloudMakersClassicGW = New-AzureRmLocalNetworkGateway  -ResourceGroupName CloudMakers -Name CloudMakersClassicGW -Location NorthEurope -GatewayIpAddress "GW Classic IP Address assigned" -AddressPrefix "10.1.0.0/16"
$CloudMakersClassicGW = Get-AzureRmLocalNetworkGateway  -ResourceGroupName CloudMakers -Name CloudMakersClassicGW

#create a gateway subnet
$vn = Get-AzureRmVirtualNetwork -ResourceGroupName "CloudMakers" -Name "CloudMakers"
Add-AzureRmVirtualNetworkSubnetConfig -Name "GatewaySubnet" -VirtualNetwork $vn -AddressPrefix 192.168.0.0/24
Set-AzureRmVirtualNetwork -VirtualNetwork $vn #required to commit changes
$GatewaySN = Get-AzureRmVirtualNetworkSubnetConfig -Name "GatewaySubnet" -VirtualNetwork $vn

#create a public IP
$GatewayIp = New-AzureRmPublicIpAddress -ResourceGroupName CloudMakers -Location NorthEurope -Name CloudMakersGWIp -AllocationMethod Dynamic
$GatewayIp = Get-AzureRmPublicIpAddress -ResourceGroupName CloudMakers -Name CloudMakersGWIp

#create a virtual gateway to be connected
$CloudMakersGWIpConfig  = New-AzureRmVirtualNetworkGatewayIpConfig -Name CloudMakersGWIpConfig -PublicIpAddressId $GatewayIp.Id -SubnetId $GatewaySN.Id
$CloudMakersGW = New-AzureRmVirtualNetworkGateway -ResourceGroupName CloudMakers -Location NorthEurope -Name CloudMakersGW -GatewayType Vpn -VpnType RouteBased -IpConfiguration $CloudMakersGWIpConfig
$CloudMakersGW = Get-AzureRmVirtualNetworkGateway -ResourceGroupName CloudMakers -Name CloudMakersGW 

#create a connection with classic (other) VN
$sharedSecred = "..." #a string secret
$CloudMakersClassicConnection = New-AzureVirtualNetworkGatewayConnection -ResourceGroupName CloudMakers -Location NorthEurope -Name CloudMakersRMClassicConnection -VirtualNetworkGateway1 $CloudMakersGW -LocalNetworkGateway2 $CloudMakersClassicGW -ConnectionType IPSec -RoutingWeight 10 -SharedKey $sharedSecret

