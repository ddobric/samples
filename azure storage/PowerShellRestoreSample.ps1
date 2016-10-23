#Restore.ps1 
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
# $VMStateFile : tempolary file for save the status of Virtual Machine 
$VMStateFile = "C:\temp\" + $VMName + "_VMstate.xml" 
#Modify end 
 
Import-Module $PsdPath 
Import-AzurePublishSettingsFile $PublishSettingsFile 
Select-AzureSubscription -SubscriptionName $SubscriptionName 
 
# Get target Azure Disk attached to $VMName 
$Disk = Get-AzureDisk | Where {($_.AttachedTo.RoleName -eq $VMName) -and ($_.OS -eq "Windows") -and ($_.AttachedTo.HostedServiceName -eq $ServiceName)} 
# Get storage account and storage key for the $Disk 
$StorageAccount = Get-AzureStorageAccount | Where {$_.Endpoints -contains ("http://" + $Disk.MediaLink.Host + "/")} 
$StorageKey = Get-AzureStorageKey -StorageAccountName $StorageAccount.StorageAccountName 
 
Set-AzureSubscription -SubscriptionName $SubscriptionName -CurrentStorageAccount $StorageAccount.STorageAccountName 
 
# Stop, Export setting, Remove VM before restore system disk 
Stop-AzureVM -ServiceName $ServiceName -Name $VMName 
$Deployment = Get-AzureDeployment $ServiceName 
$VNetName = $Deployment.VNetName 
Export-AzureVM -ServiceName $ServiceName -Name $VMName -Path $VMStateFile 
Remove-AzureVM -ServiceName $ServiceName -Name $VMName 
 
# Wait for disk is released 
$DiskName = $Disk.DiskName 
$MediaLinkAbsoluteUri = $Disk.MediaLink.AbsoluteUri 
do 
{ 
    Start-Sleep -s 60 
    $Disk = Get-AzureDisk | Where {($_.AttachedTo.RoleName -eq $VMName) -and ($_.OS -eq "Windows")} 
}while ($Disk.DiskName -eq $DiskName) 
 
# Remove disk 
Remove-AzureDisk -DiskName $Disk.DiskName -DeleteVHD 
 
Add-Type -path $StorageClientPath 
# Get credential for accessing storage account 
$Credential = New-Object Microsoft.WindowsAzure.Storage.Auth.StorageCredentials($StorageAccount.StorageAccountName, $StorageKey.Primary) 
$BlobClient = New-Object Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient($StorageAccount.Endpoints[0], $Credential) 
 
# $SourceBlob : Backed up System disk of Virtual Machine 
$SourceBlob =  $BlobClient.GetBlobReferenceFromServer($MediaLinkAbsoluteUri.Replace("vhds/", $BackupContainerName + "/")) 
$SourceBlob.FetchAttributes() 
 
# $TargetBlob : Target Blob file for restore 
$TargetBlob = New-Object Microsoft.WindowsAzure.Storage.Blob.CloudPageBlob($MediaLinkAbsoluteUri,$Credential) 
$TargetBlob.StartCopyFromBlob($SourceBlob.Uri.AbsoluteUri) 
 
# Add target blob as AzureDisk 
Add-AzureDisk -DiskName $DiskName -MediaLocation ($TargetBlob.Uri) -OS "Windows" 
 
# Import virtual machine from saved configuration and create virtual machine 
Import-AzureVM -Path $VMStateFile | New-AzureVM -ServiceName $ServiceName -VNetName $VNetName 
 
# Start Virtual Machine 
Start-AzureVM -ServiceName $ServiceName -Name $VMName 
 
#End of Script