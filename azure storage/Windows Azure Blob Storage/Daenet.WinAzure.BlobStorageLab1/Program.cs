using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Daenet.WinAzure.BlobStorageLab
{
    class Program
    {
        #region Credentials
        private static string m_AccKey = "jB1BTkwhLqpphGB6c1rjAHmYzUQfUv28s6D0GGN/EyfEGSkG6RRyKp8wEWk/2sE4yn5+vyxlzBKcydZsVk037w==";
        private static string m_AccName = "beststudents";
        #endregion

        private static string cVideoFileName = "IntegratingOnpremiseswithCloudDevicesandServices_mid.mp4";

        private const string cDevelopmentUriFormat = "http://devstorage1:10000/{0}/{1}/{2}";

        private const string cProductiveBlobUriFormat = "https://{0}.blob.core.windows.net/{1}/{2}";
        private const string cProductiveContainerUriFormat = "https://{0}.blob.core.windows.net/{1}";

        private static CloudStorageAccount getAccount()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            //string connStr = "DefaultEndpointsProtocol=https;AccountName=beststudents;AccountKey=jB1BTkwhLqpphGB6c1rjAHmYzUQfUv28s6D0GGN/EyfEGSkG6RRyKp8wEWk/2sE4yn5+vyxlzBKcydZsVk037w==";
            //var storageAccount = CloudStorageAccount.Parse(connStr);

            return storageAccount;
        }

        static void Main(string[] args)
        {
            uploadDownloadBlocks();

            //PageBlobUploader uploader = new PageBlobUploader(getAccount());
            //uploader.UploadFile("vhds", "mydisk2.vhd", @"c:\temp\mydisk.vhd");

            //downloadBlocksParallel();
            blobPagingSample();
            // uploadBlocksParallel();
            //uploadBlocks();
            manageBlobs();
            manageContainers();
        }

        private static void manageContainers()
        {
            CloudStorageAccount storageAccount = getAccount();

            CloudBlobContainer container1 =
                new CloudBlobContainer(
                    new StorageUri(new Uri(String.Format(cProductiveContainerUriFormat, "daenet01",
                        "container2"))), storageAccount.Credentials);

            container1.CreateIfNotExists();
            container1.Delete();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents");
            container.CreateIfNotExists();
            container.Delete();
        }


        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void manageBlobs()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents");
            container.CreateIfNotExists();

            var blobs = blobClient.ListBlobs("documents/file.txt");
            if (blobs.Count() == 0)
            {
                ICloudBlob blockBlobRef = blobClient.GetBlobReferenceFromServer(
                 new StorageUri(new Uri(String.Format(cProductiveBlobUriFormat, m_AccName,
                     "documents", "file.txt"))));

                foreach (var file in Directory.GetFiles("*.png"))
                {
                    blockBlobRef.UploadFromFile(file, FileMode.Create);
                }
            }
            else
            {
                foreach (var blob in blobs)
                {
                    var blobRef = blobClient.GetBlobReferenceFromServer(new StorageUri(new Uri(blob.Uri.AbsoluteUri)));
                    blobRef.Delete();
                }
            }
        }


        /// <summary>
        /// Demonstrates how to manage PageBlobs.
        /// </summary>
        private static void managePageBlobs()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("drives");
            container.CreateIfNotExists();

            var blobs = blobClient.ListBlobs("drives/");
            if (blobs.Count() == 0)
            {
                ICloudBlob blobRef =
                    blobClient.GetBlobReferenceFromServer(
                    new StorageUri(new Uri(String.Format(cProductiveBlobUriFormat, m_AccName,
                        "drives", "drivex.vhd"))));

                blobRef.UploadFromFile(@"C:\Temp\drivex.vhd", FileMode.Create);

            }
            else
            {
                foreach (var blob in blobs)
                {
                    Console.WriteLine(blob.Uri.AbsoluteUri);
                }
            }
        }


        private static string getId(int id)
        {
            return String.Format("{0,-6:100000}", id);
        }

        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void uploadDownloadBlocks()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("videos");
            container.CreateIfNotExists();

            var block = container.GetBlockBlobReference("IntegratingOnpremiseswithCloudDevicesandServices_mid.mp4");

            block.StreamWriteSizeInBytes = 65535 * 4;

            block.UploadFromFile(cVideoFileName, FileMode.Open);

            block.StreamMinimumReadSizeInBytes = 65535 * 4;

            block.DownloadToFile("Downloaded.mp4", FileMode.Create);
            // OLD CODE...
            //var blockIdList = new List<string>();

            //const int blockSize = 32768 * 2;
            //var fileStream = File.OpenRead(@"c:\temp\ProjectDb.bak");
            //var blocks = fileStream.Length / blockSize;

            //blocks++;

            //for (int b = 0; b < blocks - 1; b++)
            //{
            //    //Create a block ID. All block ID strings must have same length.
            //    string blockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId(b)));

            //    byte[] buff = new byte[blockSize];
            //    fileStream.Seek(b * blockSize, SeekOrigin.Begin);
            //    int cbRead = fileStream.Read(buff, 0, blockSize);

            //    using (var stream = new System.IO.MemoryStream(buff))
            //    {
            //        block.PutBlock(blockID64, stream, null);
            //        blockIdList.Add(blockID64);
            //    }
            //}

            //long remainingBytes = fileStream.Length - ((blocks - 1) * blockSize);

            //byte[] lastBuffer = new byte[remainingBytes];
            //fileStream.Seek(((blocks - 1) * blockSize), SeekOrigin.Begin);
            //fileStream.Read(lastBuffer, 0, (int)remainingBytes);

            //string lastBlockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)(blocks - 1))));

            //using (var stream = new System.IO.MemoryStream(lastBuffer))
            //{
            //    block.PutBlock(lastBlockID64, stream, null);
            //    blockIdList.Add(lastBlockID64);
            //}

            ////Commit the block list.
            //block.PutBlockList(blockIdList);

            //foreach (var myBlock in block.DownloadBlockList())
            //{
            //    Console.WriteLine("Name:{0}, Size: {1}", myBlock.Name, myBlock.Size);
            //}

            //block.DownloadToFile(@"c:\temp\blockFile.pptx");

        }



        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void uploadBlocksParallel()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("videos");
            container.CreateIfNotExists();

            var block = container.GetBlockBlobReference("A" + cVideoFileName);

            var blockIdList = new List<string>();

            const int blockSize = 65535 * 1;
            var fileStream = File.OpenRead(cVideoFileName);
            var blocks = fileStream.Length / blockSize;

            blocks++;

            List<Task> tasks = new List<Task>();

            Parallel.For(0, blocks - 1,
            b =>
            {

                var fStream = File.OpenRead(cVideoFileName);

                //Create a block ID. All block ID strings must have same length.
                string blockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)b)));

                Console.WriteLine(b);

                byte[] buff = new byte[blockSize];
                fStream.Seek(b * blockSize, SeekOrigin.Begin);
                int cbRead = fStream.Read(buff, 0, blockSize);

                using (var stream = new System.IO.MemoryStream(buff))
                {
                    block.PutBlock(blockID64, stream, null);
                }

                fStream.Close();
            });

            long remainingBytes = fileStream.Length - ((blocks - 1) * blockSize);

            byte[] lastBuffer = new byte[remainingBytes];
            fileStream.Seek(((blocks - 1) * blockSize), SeekOrigin.Begin);
            fileStream.Read(lastBuffer, 0, (int)remainingBytes);

            string lastBlockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)(blocks - 1))));

            using (var stream = new System.IO.MemoryStream(lastBuffer))
            {
                block.PutBlock(lastBlockID64, stream, null);
            }

            for (int i = 0; i < blocks; i++)
            {
                blockIdList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(getId(i))));
            }
            //Commit the block list.
            block.PutBlockList(blockIdList);

            foreach (var myBlock in block.DownloadBlockList())
            {
                Console.WriteLine("Name:{0}, Size: {1}", myBlock.Name, myBlock.Length);
            }

            block.StreamWriteSizeInBytes = 65535 * 4;
            block.DownloadToFile("DownloadedParellel.mp4", FileMode.Create);

        }


        private static void downloadBlocksParallel()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("videos");

            var block = container.GetBlockBlobReference(cVideoFileName);

            var numBlock = block.DownloadBlockList().Count();

            List<ListBlockItem> blockList = new List<ListBlockItem>();

            foreach (var myBlock in block.DownloadBlockList())
            {
                blockList.Add(myBlock);
            }

            int currBlock = 0;

            Parallel.For(0, numBlock,
             b =>
             {
                 var fStream = block.OpenRead();

                 if (currBlock > 0)
                     fStream.Seek(blockList[currBlock - 1].Length, SeekOrigin.Begin);

                 //Create a block ID. All block ID strings must have same length.
                 string blockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)b)));

                 Console.WriteLine(b);

                 long blockSize = blockList[currBlock].Length;

                 byte[] buff = new byte[blockList[currBlock].Length];

                 fStream.Seek(b * blockSize, SeekOrigin.Begin);

                 int cbRead = fStream.Read(buff, 0, (int)blockSize);

                 fStream.Close();
             });


            Console.ReadLine();
        }


        private static void blobPagingSample()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wad-control-container");
            container.CreateIfNotExists();

            BlobRequestOptions opt = new BlobRequestOptions()
            {
                UseTransactionalMD5 = true,
                LocationMode = LocationMode.PrimaryOnly,
                RetryPolicy = new LinearRetry(),
                ParallelOperationThreadCount = 5
            };

            BlobContinuationToken continuationToken = new BlobContinuationToken();

            BlobResultSegment resultSegment = resultSegment = container.ListBlobsSegmented(null, true,
                     BlobListingDetails.All, 10, continuationToken, opt, null);

            foreach (var blobItem in resultSegment.Results)
            {
                Console.WriteLine(blobItem.Uri.Segments[2]);
            }
            
            continuationToken = resultSegment.ContinuationToken;

            while (continuationToken != null)
            {
                resultSegment = container.ListBlobsSegmented(null, true,
                     BlobListingDetails.All, 10, continuationToken, opt, null);

                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine(blobItem.Uri.Segments[2]);
                }

                Console.WriteLine(resultSegment.Results.Count());

                continuationToken = resultSegment.ContinuationToken;

                //resultSegment.ContinuationToken.n;
            }
        }

        private static string GetContainerSasUri(CloudBlobContainer container)
        {
            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken;
        }
    }
}
