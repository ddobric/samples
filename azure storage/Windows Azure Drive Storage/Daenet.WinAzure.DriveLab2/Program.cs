using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Daenet.WinAzure.DriveLab2
{
    class Program
    {
        #region Credentials
        private const string m_AccKey = "bje87vDTkmlJcvX95P1CXUcCWEwm0Se4fY7RsLJG9BHvO2fGTPb5mGdhcMSTccgmXTB2bShSYnMHxkPFtw2F+Q==";
        private const string m_AccName = "daenet01";
        #endregion

        private const string cDevelopmentUriFormat = "http://devstorage1:10000/{0}/{1}/{2}";

        private const string cProductiveBlobUriFormat = "https://{0}.blob.core.windows.net/{1}/{2}";
        private const string cProductiveContainerUriFormat = "https://{0}.blob.core.windows.net/{1}";

        private static CloudStorageAccount getAccount()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentialsAccountAndKey(m_AccName, m_AccKey), false);
         
            return storageAccount;
        }

        static void Main(string[] args)
        {

            var mountedDrives = CloudDrive.GetMountedDrives();
          
            if (mountedDrives.Count == 0)
            {
                CloudStorageAccount account = getAccount();

                string driveUri = "drives/drivex.vhd";
                
                try
                {
                    CloudDrive.InitializeCache("C:\\temp", 10);

                    CloudDrive drive = account.CreateCloudDrive(driveUri);

                    // bool isCreated = Drive.CreateIfNotExist(64 /* sizeInMB */);

                    var mountedDrive = drive.Mount(1, DriveMountOptions.None);

                    mountedDrives = CloudDrive.GetMountedDrives();
                }
                catch (Exception ex)
                {
                    //TODO...
                }
            }
        }
    }
}
