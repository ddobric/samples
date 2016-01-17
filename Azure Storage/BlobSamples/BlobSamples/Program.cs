using Daenet.Azure.BlobStorageLab;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobSamples
{
    class Program
    {
        private static string cFile = "SampleDoc.docx";

        private static string m_AccKey = "***";
        private static string m_AccName = "***";

        private const string cDevelopmentUriFormat = "http://devstorage1:10000/{0}/{1}/{2}";

        private const string cProductiveBlobUriFormat = "https://{0}.blob.core.windows.net/{1}/{2}";

        private const string cProductiveContainerUriFormat = "https://{0}.blob.core.windows.net/{1}";

        static void Main(string[] args)
        {
            m_AccKey = ConfigurationManager.AppSettings["AccountKey"];
            m_AccName = ConfigurationManager.AppSettings["AccountName"];

            manageContainers();

            manageBlobs();

            uploadBlocks();

            downloadBlocksParallel();

            uploadBlocksParallel();

            blobPagingSample();

            blobSasDemo();
        }

        private static void blobSasDemo()
        {
            var uri = getBlobSasUri();

            CloudBlobClient client = new CloudBlobClient(new Uri(uri));

            var blobRef = client.GetBlobReferenceFromServer(new Uri(uri));

            blobRef.DownloadToFile("fileWithSAS.docx", FileMode.Open);
        }

        private static CloudStorageAccount getAccount()
        {
            StorageCredentials cred = new StorageCredentials(m_AccName, m_AccKey);

            CloudStorageAccount storageAccount = new CloudStorageAccount(cred, true);
            //storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            //var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            return storageAccount;
        }

        private static void manageContainers()
        {
            CloudStorageAccount storageAccount = getAccount();

            var storageUri = new StorageUri(new Uri(String.Format(cProductiveContainerUriFormat, m_AccName, "container1")));

            CloudBlobContainer container1 = new CloudBlobContainer(storageUri, storageAccount.Credentials);

            container1.CreateIfNotExists();

            container1.Delete();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);
            container.Delete();
        }


        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void manageBlobs()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents2");
            container.CreateIfNotExists();

            var blobs = container.ListBlobs();
            if (blobs.Count() == 0)
            {
                ICloudBlob blockBlobRef = container.GetBlockBlobReference(cFile);

                blockBlobRef.UploadFromFile(cFile, FileMode.Open);

                blockBlobRef.DownloadToFile("mydoc.tmp.docx", FileMode.Create);
            }
            else
            {
                foreach (var blob in blobs)
                {
                    var blobRef = blobClient.GetBlobReferenceFromServer(blob.Uri);
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

            var blobs = blobClient.ListBlobs("/");
            if (blobs.Count() == 0)
            {
                CloudPageBlob blobRef = container.GetPageBlobReference(String.Format(cProductiveBlobUriFormat, m_AccName,
                    "documents",
                    cFile));

                using (StreamReader cr = new StreamReader(cFile))
                {
                    blobRef.WritePages(cr.BaseStream, 0);
                }
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
            return String.Format("{0,-4:1000}", id);
        }

        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void uploadBlocks()
        {
            string fileName = "daenet.mp4";

            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents");
            container.CreateIfNotExists();

            var blockBlob = container.GetBlockBlobReference(fileName);

            var blockIdList = new List<string>();

            const int blockSize = 32768 * 2;
            using (var fileStream = File.OpenRead(fileName))
            {
                var blocks = fileStream.Length / blockSize;

                blocks++;

                for (int b = 0; b < blocks - 1; b++)
                {
                    //Create a block ID. All block ID strings must have same length.
                    string blockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId(b)));

                    byte[] buff = new byte[blockSize];
                    fileStream.Seek(b * blockSize, SeekOrigin.Begin);
                    int cbRead = fileStream.Read(buff, 0, blockSize);

                    using (var stream = new System.IO.MemoryStream(buff))
                    {
                        blockBlob.PutBlock(blockID64, stream, null);
                        blockIdList.Add(blockID64);
                    }
                }

                long remainingBytes = fileStream.Length - ((blocks - 1) * blockSize);

                byte[] lastBuffer = new byte[remainingBytes];
                fileStream.Seek(((blocks - 1) * blockSize), SeekOrigin.Begin);
                fileStream.Read(lastBuffer, 0, (int)remainingBytes);

                string lastBlockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)(blocks - 1))));

                using (var stream = new System.IO.MemoryStream(lastBuffer))
                {
                    blockBlob.PutBlock(lastBlockID64, stream, null);
                    blockIdList.Add(lastBlockID64);
                }

                //Commit the block list.
                blockBlob.PutBlockList(blockIdList);

                foreach (var myBlock in blockBlob.DownloadBlockList())
                {
                    Console.WriteLine("Name:{0}, Size: {1}", myBlock.Name, myBlock.Length);
                }
            }

            blockBlob.DownloadToFile("d_" + fileName, FileMode.Create);

        }



        /// <summary>
        /// Demonstrates how to manage BlockBlobs.
        /// </summary>
        private static void uploadBlocksParallel()
        {
            string fName = "daenet.mp4";

            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("kuhn-documents");
            container.CreateIfNotExists();

            var block = container.GetBlockBlobReference(fName);

            var blockIdList = new SortedList<long, string>();

            const int blockSize = 32768 * 5;
            var fileStream = File.OpenRead(fName);
            var blocks = fileStream.Length / blockSize;

            blocks++;

            List<Task> tasks = new List<Task>();

            Parallel.For(0, blocks - 1, b =>
            {
                //lock ("aa")
                {
                    //Create a block ID. All block ID strings must have same length.
                    string blockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)b)));

                    byte[] buff = new byte[blockSize];
                    fileStream.Seek(b * blockSize, SeekOrigin.Begin);
                    int cbRead = fileStream.Read(buff, 0, blockSize);

                    using (var stream = new System.IO.MemoryStream(buff))
                    {
                        block.PutBlock(blockID64, stream, null);
                        blockIdList.Add(b, blockID64);
                    }
                }
            });

            long remainingBytes = fileStream.Length - ((blocks - 1) * blockSize);

            byte[] lastBuffer = new byte[remainingBytes];
            fileStream.Seek(((blocks - 1) * blockSize), SeekOrigin.Begin);
            fileStream.Read(lastBuffer, 0, (int)remainingBytes);

            string lastBlockID64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(getId((int)(blocks - 1))));

            using (var stream = new System.IO.MemoryStream(lastBuffer))
            {
                block.PutBlock(lastBlockID64, stream, null);
                blockIdList.Add(blocks, lastBlockID64);
            }


            var blockIdList2 = new List<string>();
            foreach (var item in blockIdList)
            {
                blockIdList2.Add(item.Value);
            }
            //Commit the block list.
            block.PutBlockList(blockIdList2);

            foreach (var myBlock in block.DownloadBlockList())
            {
                Console.WriteLine("Name:{0}, Size: {1}", myBlock.Name, myBlock.Length);
            }

            block.DownloadToFile("media.mp4", FileMode.Create);

        }

        private static void downloadBlocksParallel()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("documents");

            var block = container.GetBlockBlobReference("daenet.mp4");

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

            // BlobRequestOptions opt = new BlobRequestOptions() { UseTransactionalMD5 = true };

            BlobContinuationToken continuationToken = new BlobContinuationToken();

            BlobResultSegment resultSegment = container.ListBlobsSegmented(continuationToken);

            foreach (var blobItem in resultSegment.Results)
            {
                Console.WriteLine(blobItem.Uri.Segments[2]);
            }

            int pageCount = 0;

            continuationToken = resultSegment.ContinuationToken;
            while (continuationToken != null)
            {
                resultSegment = container.ListBlobsSegmented(continuationToken);

                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine(blobItem.Uri.Segments[2]);
                }

                Console.WriteLine(resultSegment.Results.Count());

                continuationToken = resultSegment.ContinuationToken;

                //resultSegment.ContinuationToken.n;
            }
        }

        private static string getContainerSasUri(CloudBlobContainer container)
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


        private static ICloudBlob getBlob()
        {
            CloudStorageAccount storageAccount = getAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("documents");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Off);

            ICloudBlob blobRef = container.GetBlockBlobReference(cFile);

            var isExist = blobRef.Exists();
            if (isExist == false)
                blobRef.UploadFromFile(cFile, FileMode.Open);

            return blobRef;
        }


        private static string getBlobSasUri()
        {
            var cloudBlobRef = getBlob();

            //Set the expiry time and permissions for the container.
            //In this case no start time is specified, so the shared access signature becomes valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            //Generate the shared access signature on the container, setting the constraints directly on the signature.
            string sasContainerToken = cloudBlobRef.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return cloudBlobRef.Uri + sasContainerToken;
        }
    }
}
