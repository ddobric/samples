#Backup.ps1 
# 
# Parameters 
# $ServiceName : DNS Name for target Virtual Machine 
# $VMName : Host Name for target Virtual Machine 
# 
Param($ServiceName, $VMName) 
 
#Modify here 
# $PublishSettingsFile : Windows Azure Publish setting file 
$PublishSettingsFile = "C:\Azure\SubscriptionName-12-25-2012-credentials.publishsettings" 
# $SubscriptionName : Windows Azure subscription name 
$SubscriptionName = "Subscription Name" 
# $BackupContainerName : Backup file to be placed into this container 
$BackupContainerName = "vhdbackups" 
# $PsdPath : Windows Azure PowerShell Cmdlets file 
$PsdPath = "C:\Program Files (x86)\Microsoft SDKs\Windows Azure\PowerShell\Azure\Azure.psd1" 
# $StorageClientPath : Windows Azure Storage Client DLL file 
$StorageClientPath = "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\2012-10\bin\Microsoft.WindowsAzure.StorageClient.dll" 
#Modify end 
 
Import-Module $PsdPath 
Import-AzurePublishSettingsFile $PublishSettingsFile 
Set-AzureSubscription -DefaultSubscription $SubscriptionName 
Select-AzureSubscription -SubscriptionName $SubscriptionName 
 
# Get target Azure Disk attached to $VMName 
$Disk = Get-AzureDisk | Where {($_.AttachedTo.RoleName -eq $VMName) -and ($_.OS -eq "Windows") -and ($_.AttachedTo.HostedServiceName -eq $ServiceName)} 
# Get storage account and storage key for the $Disk 
$StorageAccount = Get-AzureStorageAccount | Where {$_.Endpoints -contains ("http://" + $Disk.MediaLink.Host + "/")} 
$StorageKey = Get-AzureStorageKey -StorageAccountName $StorageAccount.StorageAccountName 
 
Set-AzureSubscription -SubscriptionName $SubscriptionName -CurrentStorageAccount $StorageAccount.StorageAccountName 
 
# Stop VM before backup system disk 
Stop-AzureVM -ServiceName $ServiceName -Name $VMName 
 
Add-Type -path $StorageClientPath 
# Get credential for accessing storage account 
$Credential = New-Object Microsoft.WindowsAzure.Storage.Auth.StorageCredentials($StorageAccount.StorageAccountName, $StorageKey.Primary) 
# $BlobClient : BlobClient object for target storage account 
$BlobClient = New-Object Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient($StorageAccount.Endpoints[0], $Credential) 
 
# Create container for backup if not exist. 
$Container = $BlobClient.GetContainerReference($BackupContainerName) 
$Container.CreateIfNotExists() 
$Permission = $Container.GetPermissions() 
$Permission.PublicAccess = "Off" 
$Container.SetPermissions($Permission) 
 
# $SourceBlob : System disk of Virtual Machine 
$SourceBlob = $BlobClient.GetBlobReferenceFromServer($Disk.MediaLink.AbsoluteUri) 
$SourceBlob.FetchAttributes() 
 
# $TargetBlob : Target Blob file for backup 
$TargetBlob = New-Object Microsoft.WindowsAzure.Storage.Blob.CloudPageBlob($Disk.MediaLink.AbsoluteUri.Replace("vhds/", $BackupContainerName + "/"),$Credential) 
$TargetBlob.StartCopyFromBlob($SourceBlob.Uri.AbsoluteUri) 
 
# Start Virtual Machine 
Start-AzureVM -ServiceName $ServiceName -Name $VMName 
 
#End of Script