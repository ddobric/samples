using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {

        public static string MountedDrive;

        public static CloudDrive m_Drive;


        public override bool OnStart()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher(
           (configName, configSettingPublisher) =>
           {
               var connectionString =
                   RoleEnvironment.GetConfigurationSettingValue(configName);
               configSettingPublisher(connectionString);
           }
       );
            

            initDiagnostics();

           // mountDrive();

            System.Diagnostics.Trace.WriteLine("WebRole started");
            
            return base.OnStart();
        }

        private void initDiagnostics()
        {
            DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            // Transfer logs to storage every 10 seconds
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromSeconds(10);

            // Transfer verbose, critical, etc. logs
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

            Microsoft.WindowsAzure.Diagnostics.CrashDumps.EnableCollection(true);

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);
        }


        private static void mountDrive()
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

                    m_Drive = account.CreateCloudDrive(driveUri);

                    // bool isCreated = Drive.CreateIfNotExist(64 /* sizeInMB */);

                    MountedDrive = m_Drive.Mount(1, DriveMountOptions.None);

                    mountedDrives = CloudDrive.GetMountedDrives();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetType().Name);
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        public override void OnStop()
        {
            m_Drive.Unmount();
            System.Diagnostics.Trace.WriteLine("WebRole stopped");
            base.OnStop();
        }
    }
}
