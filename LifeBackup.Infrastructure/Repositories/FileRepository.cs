using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeBackup.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IAmazonS3 _s3Client;

        public FileRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public AddFileResponse UploadFiles(string bucketName, IList<IFormFile> formFiles)
        {
            List<string> response = new();

            _ = formFiles.Select(f =>
            {
                TransferUtilityUploadRequest uploadRequest = new()
                {
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.NoACL,
                    InputStream = f.OpenReadStream(),
                    Key = f.FileName
                };

                using (TransferUtility fileTransferUtility = new(_s3Client))
                {
                    fileTransferUtility.UploadAsync(uploadRequest).Wait();
                };

                GetPreSignedUrlRequest expiryUrlRequest = new()
                {
                    BucketName = bucketName,
                    Key = f.FileName,
                    Expires = DateTime.Now.AddDays(1)
                };

                string url = _s3Client.GetPreSignedURL(expiryUrlRequest);
                response.Add(url);

                return f;
            })
            .ToArray();

            return new AddFileResponse
            {
                PreSignedUrl = response
            };
        }

        public async Task DownloadFileAsync(string bucketName, string fileName)
        {
            string pathAndFileName = $"C:\\S3Temp\\{fileName}"; //TODO: improve -> contact directories

            TransferUtilityDownloadRequest downloadRequest = new()
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = pathAndFileName
            };

            using TransferUtility transferUtility = new(_s3Client);

            await transferUtility.DownloadAsync(downloadRequest);
        }

        public async Task<IEnumerable<ListFileResponse>> ListFilesAsync(string bucketName)
        {
            ListObjectsResponse response = await _s3Client.ListObjectsAsync(bucketName);

            return response.S3Objects.Select(b => new ListFileResponse
            {
                BubketName = b.BucketName,
                Key = b.Key,
                Owner = b.Owner.DisplayName,
                Size = b.Size
            });
        }
    }
}
