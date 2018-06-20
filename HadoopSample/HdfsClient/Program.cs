using Microsoft.Hadoop.WebHDFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HdfsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string srcFileName = @"c:\temp\ufo_awesome.tsv";
            string folderName = @"/input";
            string destFileName = "??";

            // Connect to hadoop cluster
            Uri myUri = new Uri("http://localhost:50070");
            string userName = "root";
            WebHDFSClient hdfsClient = new WebHDFSClient(myUri, userName);

            hdfsClient.CreateDirectory(folderName).Wait();

            // Uploading File from local file share to hadoop 
            //var res = hdfsClient.CreateFile(@"testfile.txt", $"{folderName}/testfile.txt").Result;
            //Console.WriteLine("File Created At {0}", res);
                        
            hdfsClient.GetDirectoryStatus(folderName).ContinueWith(
             ds => ds.Result.Files.ToList().ForEach((f) =>
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("t" + f.PathSuffix);
                   // var hdfsFile = hdfsClient.OpenFile($"{folderName}/{f.PathSuffix}").Result;
                    //var fileContent = hdfsFile.Content.ReadAsStringAsync().Result;

                }
             )).Wait();
        }
    }
}
