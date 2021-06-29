using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request)
        {
            DateTime createdOnUTC = DateTime.UtcNow;
            request.Id = Guid.NewGuid();
            request.TimeSent = createdOnUTC;
            string s3Key = $"{createdOnUTC:yyyy}/{createdOnUTC:MM}/{createdOnUTC:dd}/{request.Id}";

            PutObjectRequest putObjectRequest = new()
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = JsonConvert.SerializeObject(request)
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<DeleteFileResposne> DeleteFileAsync(string bucketName, string fileName)
        {
            DeleteObjectsRequest multiObjectDeleteRequest = new() { BucketName = bucketName };
            multiObjectDeleteRequest.AddKey(fileName);

            DeleteObjectsResponse response = await _s3Client.DeleteObjectsAsync(multiObjectDeleteRequest);

            return new DeleteFileResposne { NumberOfDeletedObjects = response.DeletedObjects.Count };
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

        public async Task<GetJsonObjectResponse> GetJsonObjectAsync(string bucketName, string fileName)
        {
            GetObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = fileName
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request);

            using StreamReader reader = new(response.ResponseStream);
            string contents = reader.ReadToEnd();

            try
            {
                return JsonConvert.DeserializeObject<GetJsonObjectResponse>(contents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error InnerException Message: {ex.InnerException?.Message}");
                Console.WriteLine($"Error InnerException InnerException Message: {ex.InnerException?.InnerException?.Message}");

                throw new Exception("Error while reading file from bucket. Check console log.");
            }
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
    }
}
