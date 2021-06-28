using Amazon.S3;
using Amazon.S3.Model;
using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeBackup.Infrastructure.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<CreateBucketResponse> CreateBucketAsync(string bucketName)
        {
            PutBucketRequest request = new()
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            PutBucketResponse response = await _s3Client.PutBucketAsync(request);

            return new CreateBucketResponse
            {
                BucketName = bucketName,
                RequestId = response?.ResponseMetadata?.RequestId
            };
        }

        public async Task<bool> DoesS3BucketExistAsync(string bucketName)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketName);
        }

        public async Task<IEnumerable<ListS3BucketResponse>> ListBucketsAsync()
        {
            ListBucketsResponse response = await _s3Client.ListBucketsAsync();

            return response.Buckets.Select(b => new ListS3BucketResponse
            {
                BucketName = b.BucketName,
                CreationDate = b.CreationDate
            });
        }
    }
}
