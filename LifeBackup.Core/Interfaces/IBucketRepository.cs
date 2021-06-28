using LifeBackup.Core.Communication.Bucket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeBackup.Core.Interfaces
{
    public interface IBucketRepository
    {
        public Task<BucketResponse> CreateBucketAsync(string bucketName);
        public Task<bool> DoesS3BucketExistAsync(string bucketName);
        public Task<BucketResponse> DeleteS3BucketAsync(string bucketName);
        public Task<IEnumerable<ListS3BucketResponse>> ListBucketsAsync();
    }
}
