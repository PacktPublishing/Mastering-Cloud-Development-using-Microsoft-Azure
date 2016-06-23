########################################################
#
# Configuring Office Connectivity with Point-to-Site VPN
#
########################################################

#create gateway with address space
#makecert.exe needs to be on path
makecert -sky exchange -r -n "CN=OwenCloudMakers01" -pe -a sha1 -len 2048 -ss My "OwenCloudMakers01.cer"
makecert.exe -n "CN=OwenClientCloudMakers01" -pe -sky exchange -m 96 -ss My -in "OwenCloudMakers01" -is my -a sha256

#Site-To-Site VPN
$sharedSecred = "..." #a string secret
Set-AzureVNetGatewayKey -VNetName "Group CloudMakers CloudMakersClassic" -LocalNetworkSiteName CloudMakersARM -SharedKey $sharedSecred -Verbose
(Get-AzureVNetGateway -VNetName "Group CloudMakers CloudMakersClassic").VIPAddress


