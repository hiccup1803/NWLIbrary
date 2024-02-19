using NHibernate;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Excel;
using System.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;
using NW.Security;

namespace NW.Service
{
    public class FileService : BaseService, IFileService
    {
        private CloudBlobClient BlobClient;
        public FileService(
            IUnitOfWork _unitOfWork,
            ISession _session)
            : base(_unitOfWork, _session)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            BlobClient = storageAccount.CreateCloudBlobClient();
        }
        public CloudBlobContainer GetContainer(string containerName)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            return container;
        }
        public IList<CloudBlobContainer> GetContainers()
        {
            return BlobClient.ListContainers().ToList();
        }
        public bool ContainerExists(string containerName)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            return container.Exists();

        }
        public void CreateContainer(string containerName, BlobContainerPublicAccessType permission = BlobContainerPublicAccessType.Blob)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = permission });
            container.Create();
        }
        public bool CreateContainerIfNotExists(string containerName, BlobContainerPublicAccessType permission = BlobContainerPublicAccessType.Blob)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = permission });
            return container.CreateIfNotExists();
        }
        public bool DeleteContainer(string containerName)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            return container.DeleteIfExists();

        }
        //public IList<CloudBlob> ListBlobsInContainer(CloudBlobContainer container, int pageIndex = 1, int pageSize = 40)
        //{
        //    return container.ListBlobs().OfType<CloudBlob>().OrderByDescending(b=>b.Properties.LastModified).Skip(pageSize*(pageIndex-1)).Take(pageSize).ToList();
        //}
        //public IList<CloudBlob> ListBlobsInContainer(string containerName, int pageIndex = 1, int pageSize = 40)
        //{
        //    CloudBlobContainer container = GetContainer(containerName);
        //    return ListBlobsInContainer(container, pageIndex, pageSize);
        //}
        public bool BlobExists(CloudBlobContainer container, string fileName)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            return blockBlob.Exists();
        }
        public bool BlobExists(string containerName, string fileName)
        {
            CloudBlobContainer container = GetContainer(containerName);
            return BlobExists(container, fileName);
        }
        public CloudBlockBlob UploadFile(CloudBlobContainer container, string fileName, FileStream fileStream)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromStream(fileStream);
            return blockBlob;
        }
        public CloudBlockBlob UploadFile(CloudBlobContainer container, string fileName, MemoryStream fileStream)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromStream(fileStream);
            return blockBlob;
        }
        public CloudBlockBlob UploadFile(string containerName, string fileName, FileStream fileStream)
        {
            CreateContainerIfNotExists(containerName);
            CloudBlobContainer container = GetContainer(containerName);
            return UploadFile(container, fileName, fileStream);
        }
        public CloudBlockBlob UploadFile(string containerName, string fileName, byte[] fileByteArray)
        {
            CreateContainerIfNotExists(containerName);
            CloudBlobContainer container = GetContainer(containerName);
            using (var fileStream = new MemoryStream(fileByteArray, writable: false))
            {
                return UploadFile(container, fileName, fileStream);
            }
        }
        public string UploadFileReturnUri(string containerName, string fileName, byte[] fileByteArray)
        {
            CreateContainerIfNotExists(containerName);
            CloudBlobContainer container = GetContainer(containerName);
            using (var fileStream = new MemoryStream(fileByteArray, writable: false))
            {
                return UploadFile(container, fileName, fileStream).Uri.ToString();
            }
        }
        public FileStream DownloadFile(CloudBlobContainer container, string fileName, string path)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            using (var fileStream = System.IO.File.OpenWrite(String.Format(@"{0}/{1}", path, fileName)))
            {
                blockBlob.DownloadToStream(fileStream);
                return fileStream;
            }
        }
        public FileStream DownloadFile(string containerName, string fileName, string path)
        {
            CloudBlobContainer container = GetContainer(containerName);
            return DownloadFile(container, fileName, path);
        }
        public bool DeleteFile(CloudBlobContainer container, string fileName)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            return blockBlob.DeleteIfExists();
        }
        public bool DeleteFile(string containerName, string fileName)
        {
            CloudBlobContainer container = GetContainer(containerName);
            return DeleteFile(container, fileName);
        }
        public void RenameFile(CloudBlobContainer container, string oldFileName, string newFileName)
        {
            CloudBlockBlob oldBlob = container.GetBlockBlobReference(oldFileName);
            CloudBlockBlob newBlob = container.GetBlockBlobReference(newFileName);

            using (var stream = new MemoryStream())
            {
                oldBlob.DownloadToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                newBlob.UploadFromStream(stream);

                //copy metadata here if you need it too

                oldBlob.Delete();
            }
        }
        public void RenameFile(string containerName, string oldFileName, string newFileName)
        {
            CloudBlobContainer container = GetContainer(containerName);
            RenameFile(container, oldFileName, newFileName);
        }

        public bool ContainerNameIsValid(string containerName)
        {
            // Container names must be valid DNS names, and must conform to these rules:
            // * Container names must start with a letter or number, and can contain only letters, numbers, and the dash (-) character.
            // * Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.
            // * All letters in a container name must be lowercase.
            // * Container names must be from 3 through 63 characters long.

            // $root is a special container that can exist at the root level and is always valid.
            if (containerName.Equals("$root"))
                return false;

            if (!Regex.IsMatch(containerName, @"^[a-z0-9](([a-z0-9\-[^\-])){1,61}[a-z0-9]$"))
            {
                return false;
            }
            return true;
        }


        #region Excel
        #endregion
        #region cloudinary
        public string UploadImageToCloudinary(string folderName, string fileName, byte[] fileByteArray)
        {

            var cloudinary = new Cloudinary(
               new Account(
                 ConfigurationManager.AppSettings["Cloudinary_CloudName"],
                 ConfigurationManager.AppSettings["Cloudinary_APIKey"],
                 ConfigurationManager.AppSettings["Cloudinary_APISecret"]));

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, new MemoryStream(fileByteArray, writable: false)),
                PublicId = String.Format("{0}/{1}",folderName,fileName)
            };

            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult.Uri.ToString();

        }
        public string DownloadEncryptedImageFromCloudinary(string url)
        {
            WebRequest ftpRequest = WebRequest.Create(url);
            WebResponse response = (WebResponse)ftpRequest.GetResponse();

            string text = string.Empty;
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                text = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return SecurityHelper.Decrypt(text);

        }
        #endregion

    }
}
