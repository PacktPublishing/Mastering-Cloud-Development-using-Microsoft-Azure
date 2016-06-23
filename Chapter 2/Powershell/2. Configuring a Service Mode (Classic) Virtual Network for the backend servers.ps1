##############################################################################
#
# Configuring a Service Mode (Classic) Virtual Network for the backend servers
#
##############################################################################

#obtain vnet configuration file
Get-AzureVNetConfig -ExportToFile classicvnets.netcfg #you need to put a path to the file

#edit file locally with text editor
<?xml version="1.0" encoding="utf-8"?>
<NetworkConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration">
  <VirtualNetworkConfiguration>
    <Dns />
    <VirtualNetworkSites>
      <VirtualNetworkSite name="Group CloudMakers CloudMakersClassic" Location="North Europe">
        <AddressSpace>
          <AddressPrefix>10.1.0.0/16</AddressPrefix>
        </AddressSpace>
        <Subnets>
          <Subnet name="DataServer">
            <AddressPrefix>10.1.2.0/24</AddressPrefix>
          </Subnet>
        </Subnets>
      </VirtualNetworkSite>
     </VirtualNetworkSites>
  </VirtualNetworkConfiguration>
</NetworkConfiguration>
#end of edit file locally with text editor

#write back configuration to 
Set-AzureVNetConfig -ConfigurationPath classicvnets.netcfg  #you need to put a path to the file

#configure the gateway from portal

#wait for gateway provisioning: it can take several minutes

#get the VN IP address
$gw = (Get-AzureVNetGateway -VNetName "Group CloudMakers CloudMakersClassic")
$gw.VIPAddress