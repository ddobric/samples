using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

//http://blogs.msdn.com/b/windowsazurestorage/archive/2010/04/11/using-windows-azure-page-blobs-and-how-to-efficiently-upload-and-download-page-blobs.aspx

namespace Daenet.Azure.BlobStorageLab
{
    public class PageBlobUploader
    {
        private CloudStorageAccount m_Account;

        public PageBlobUploader(CloudStorageAccount account)
        {
            m_Account = account;
        }

    
        private static bool IsAllZero(byte[] range, long rangeOffset, long size)
        {
            for (long offset = 0; offset < size; offset++)
            {
                if (range[rangeOffset + offset] != 0)
                {
                    return false;
                }
            }
            return true;
        }


        public void UploadFile(string containerName, string blobName, string fileName)
        {
            long fileLength = new FileInfo(fileName).Length;

            CloudBlobClient blobStorage = this.m_Account.CreateCloudBlobClient();

            CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
            container.CreateIfNotExists();

            string blobUri = String.Format("http://{0}.blob.core.windows.net/{1}/{2}", m_Account.Credentials.AccountName, containerName, blobName);

            CloudPageBlob pageBlob = container.GetPageBlobReference(blobUri);

            long blobSize = roundPageBloSize(fileLength);
            pageBlob.Create(blobSize);

            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            long totalUploaded = 0;
            long vhdOffset = 0;
            int offsetToTransfer = -1;

            while (vhdOffset < fileLength)
            {
                byte[] range = reader.ReadBytes(FourMegabytesAsBytes);

                int offsetInRange = 0;

                if ((range.Length % cPageBlobPageSize) > 0)
                {
                    int grow = (int)(cPageBlobPageSize - (range.Length % cPageBlobPageSize));
                    Array.Resize(ref range, range.Length + grow);
                }

                // Upload groups of contiguous non-zero page blob pages.  
                while (offsetInRange <= range.Length)
                {
                    if ((offsetInRange == range.Length) ||
                        IsAllZero(range, offsetInRange, cPageBlobPageSize))
                    {
                        if (offsetToTransfer != -1)
                        {
                            // Transfer up to this point
                            int sizeToTransfer = offsetInRange - offsetToTransfer;
                            MemoryStream memoryStream = new MemoryStream(range, offsetToTransfer, sizeToTransfer, false, false);
                            pageBlob.WritePages(memoryStream, vhdOffset + offsetToTransfer);
                            Console.WriteLine("Range ~" + Megabytes(offsetToTransfer + vhdOffset) + " + " + PrintSize(sizeToTransfer));
                            totalUploaded += sizeToTransfer;
                            offsetToTransfer = -1;
                        }
                    }
                    else
                    {
                        if (offsetToTransfer == -1)
                        {
                            offsetToTransfer = offsetInRange;
                        }
                    }
                    offsetInRange += cPageBlobPageSize;
                }
                vhdOffset += range.Length;
            }
            Console.WriteLine("Uploaded " + Megabytes(totalUploaded) + " of " + Megabytes(blobSize));
        }

        private const int cPageBlobPageSize = 512;
        private static int OneMegabyteAsBytes = 1024 * 1024;
        private static int FourMegabytesAsBytes = 4 * OneMegabyteAsBytes;
        private static string PrintSize(long bytes)
        {
            if (bytes >= 1024 * 1024) return (bytes / 1024 / 1024).ToString() + " MB";
            if (bytes >= 1024) return (bytes / 1024).ToString() + " kb";
            return (bytes).ToString() + " bytes";
        }
        private static string Megabytes(long bytes)
        {
            return (bytes / OneMegabyteAsBytes).ToString() + " MB";
        }

        private static long roundPageBloSize(long size)
        {
            return (size + cPageBlobPageSize - 1) & ~(cPageBlobPageSize - 1);
        }

    }
  
}
