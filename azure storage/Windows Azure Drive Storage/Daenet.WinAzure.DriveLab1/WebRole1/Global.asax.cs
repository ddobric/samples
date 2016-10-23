using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics;

namespace WebRole1
{
    public class Global : System.Web.HttpApplication
    {
        public static string MountedDrive;

        public static CloudDrive Drive;

        void Application_Start(object sender, EventArgs e)
        {
            Trace.WriteLine("Application_Start");

            CloudStorageAccount.SetConfigurationSettingPublisher(
              (configName, configSettingPublisher) =>
              {
                  var connectionString =
                      RoleEnvironment.GetConfigurationSettingValue(configName);
                  configSettingPublisher(connectionString);
              }
          );
            
            //MountDrive();         
        }

     
       
        public static void MountDrive()
        {
            var mountedDrives = CloudDrive.GetMountedDrives();
            if (mountedDrives.Count == 0)
            {
                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("CloudStorageAccount");

                string driveUri = RoleEnvironment.GetConfigurationSettingValue("DriveUri");
               
                LocalResource localCache = RoleEnvironment.GetLocalResource("MyAzureDriveCache");

                try
                {
                    CloudDrive.InitializeCache(localCache.RootPath, localCache.MaximumSizeInMegabytes);

                    Drive = account.CreateCloudDrive(driveUri);
                    Trace.WriteLine("CreateCloudDrive: " + Drive.Uri.AbsoluteUri);
                    
                    bool isCreated = Drive.CreateIfNotExist(20 /* sizeInMB */);
                    Trace.WriteLine("CreateIfNotExist: " + isCreated);

                    MountedDrive = Drive.Mount(20, DriveMountOptions.None);
                    Trace.WriteLine("Mounted to: " + MountedDrive);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetType().Name);
                    Trace.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            Drive.Unmount();

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
  

            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
