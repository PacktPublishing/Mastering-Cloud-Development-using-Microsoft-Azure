##############################################################
#
# Create Azure Storage Account
#
##############################################################

New-AzureRmRedisCache -ResourceGroupName CloudMakers -Name AdventureWorks -Location "West Europe" -Size C0 -sku Basic

$redisCache = Get-AzureRmRedisCache -ResourceGroupName CloudMakers -Name AdventureWorks
$redisCache.HostName

$redisCacheKey = $redisCache | Get-AzureRmRedisCacheKey 
$redisCacheKey.PrimaryKey

$redisCache.HostName + ":6380,ssl=true,password=" + $redisCacheKey.PrimaryKey
                                