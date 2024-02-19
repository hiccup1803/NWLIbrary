using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IFileService
    {
        CloudBlobContainer GetContainer(string containerName);
        IList<CloudBlobContainer> GetContainers();
        bool ContainerExists(string containerName);
        void CreateContainer(string containerName, BlobContainerPublicAccessType permission = BlobContainerPublicAccessType.Blob);
        bool CreateContainerIfNotExists(string containerName, BlobContainerPublicAccessType permission = BlobContainerPublicAccessType.Blob);
        bool DeleteContainer(string containerName);
        //IList<CloudBlob> ListBlobsInContainer(CloudBlobContainer container, int pageIndex = 1, int pageSize = 40);
        //IList<CloudBlob> ListBlobsInContainer(string containerName, int pageIndex = 1, int pageSize = 40);
        bool BlobExists(CloudBlobContainer container, string fileName);
        bool BlobExists(string containerName, string fileName);
        CloudBlockBlob UploadFile(CloudBlobContainer container, string fileName, FileStream fileStream);
        CloudBlockBlob UploadFile(CloudBlobContainer container, string fileName, MemoryStream memoryStream);
        CloudBlockBlob UploadFile(string containerName, string fileName, FileStream fileStream);
        CloudBlockBlob UploadFile(string containerName, string fileName, byte[] fileByteArray);
        string UploadFileReturnUri(string containerName, string fileName, byte[] fileByteArray);
        FileStream DownloadFile(CloudBlobContainer container, string fileName, string path);
        FileStream DownloadFile(string containerName, string fileName, string path);
        bool DeleteFile(CloudBlobContainer container, string fileName);
        bool DeleteFile(string containerName, string fileName);
        void RenameFile(CloudBlobContainer container, string oldFileName, string newFileName);
        void RenameFile(string containerName, string oldFileName, string newFileName);
        bool ContainerNameIsValid(string containerName);

        #region Excel
        #endregion
        #region cloudinary
        string UploadImageToCloudinary(string folderName, string fileName, byte[] fileByteArray);
        string DownloadEncryptedImageFromCloudinary(string url);
        #endregion

    }
}
