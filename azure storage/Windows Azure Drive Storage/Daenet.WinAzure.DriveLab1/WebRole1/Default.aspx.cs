using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;

namespace WebRole1
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("...");
        }

        /// <summary>
        /// Demonstrates how to enlist mounted drives.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {          
            
            var mountedDrives = CloudDrive.GetMountedDrives();
            if (mountedDrives.Count == 0)
            {
                Response.Write("Mounting drive.");
                Global.MountDrive();
            }
            else
            {
                CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("CloudStorageAccount");
                string driveUri = RoleEnvironment.GetConfigurationSettingValue("DriveUri");

                Global.MountedDrive = mountedDrives.First().Key;

                foreach (var keyVal in mountedDrives)
                {
                    Response.Write(String.Format("{0} - {1}", keyVal.Key, keyVal.Value));
                }
            }          
        }


        /// <summary>
        /// Demonstrates how to write to drive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button2_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(Global.MountedDrive + DateTime.Now.Ticks.ToString() + ".txt");
            sw.Write(DateTime.Now.ToString());
            sw.Flush();
            sw.Close();
        }


        /// <summary>
        /// Denomstrates how to read from drive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button4_Click(object sender, EventArgs e)
        {
            Response.Write("<BR/>");
            Response.Write("Files on drive:<BR/>");
            Response.Write("-----------<BR/>");

            foreach (var file in Directory.GetFiles(Global.MountedDrive, "*.*"))
            {   
                Response.Write(file);
                Response.Write("<BR/>");                
            }
        }


        /// <summary>
        /// Demonstrates snapshot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button3_Click(object sender, EventArgs e)
        {
            Global.Drive.Unmount();
            //var uri = Global.Drive.Snapshot();
                        
            //Global.Drive.CopyTo(new Uri(RoleEnvironment.GetConfigurationSettingValue("DriveUri").Replace("drive", "snapshot")));
        }


       
     
    }
}
