##################################################################
#
# Deploy a Windows Azure Virtual Machine in an ARM Virtual Network
#
##################################################################

#Create a public IP Address for Web Server
$WebServer01PublicIp = New-AzureRmPublicIpAddress -Name WebServer01PublicIp -ResourceGroupName CloudMakers -Location NorthEurope -DomainNameLabel cloudmakerswebserver01 -AllocationMethod Static
$WebServer01PublicIp = Get-AzureRmPublicIpAddress -Name WebServer01PublicIp -ResourceGroupName CloudMakers

#Create a public Network Interface for Web Server
$WebServer01NI = New-AzureRmNetworkInterface -ResourceGroupName CloudMakers -Subnet $webServerSN -Location NorthEurope -Name webserver01
$WebServer01NI = Get-AzureRmNetworkInterface -ResourceGroupName CloudMakers

#create storage
$cloudmakersvms = New-AzureRmStorageAccount -ResourceGroupName CloudMakers -Name cloudmakersvms -Type Standard_GRS -Location NorthEurope
$cloudmakersvms = Get-AzureRmStorageAccount -ResourceGroupName CloudMakers -Name cloudmakersvms

#Queries to 
Get-AzureRmVMSize -Location NorthEurope | Select Name, NumberOfCores, MemoryInMb | Format-Table
Get-AzureRmVMImageSku -Location NorthEurope -Publisher MicrosoftWindowsServer -Offer WindowsServer | Select Skus #2012-R2-Datacenter

#get administrator credential from the UI: ANNOTATE ON PAPER AND TAKE CARE OF THEM!!!!
$adminCredential = Get-Credential

#Steps to prepare VM Configuration
$vmConfig = New-AzureRmVMConfig -VMName CMWebServer01 -VMSize Standard_A1
$vmConfigOs = $vmConfig | Set-AzureRmVMOperatingSystem -Windows -ComputerName CMWebServer01 -ProvisionVMAgent -EnableAutoUpdate -Credential $adminCredential
$vmSource = $vmConfigOs |  Set-AzureRmVMSourceImage  -PublisherName MicrosoftWindowsServer  -Offer WindowsServer  -Skus 2012-R2-Datacenter -Version "latest"
$vmSourceOS = $vmSource | Set-AzureRmVMOSDisk -Name "OS" -VhdUri ($cloudmakersvms.PrimaryEndpoints.Blob.ToString() + "vhds/webServer01-os.vhd") -CreateOption fromImage
$vmSourceOSData = $vmSourceOS | Add-AzureRmVMDataDisk -Name "Data" -DiskSizeInGB 20 -VhdUri ($cloudmakersvms.PrimaryEndpoints.Blob.ToString() + "vhds/webServer01-data.vhd") -CreateOption empty
$vmSourceOSDataNic = $vmSourceOSData | Add-AzureRmVMNetworkInterface -Id (Get-AzureRmNetworkInterface -ResourceGroupName CloudMakers -Name WebServer01).Id -Primary

#Deploy VM
$vm = New-AzureRmVM  -VM $vmSourceOSDataNic -ResourceGroupName CloudMakers -Location NorthEurope
