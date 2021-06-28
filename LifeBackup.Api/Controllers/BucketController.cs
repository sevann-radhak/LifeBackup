using LifeBackup.Core.Communication.Bucket;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeBackup.Api.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : Controller
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<BucketResponse>> CreateS3BucketAsync([FromRoute] string bucketName)
        {
            bool bucketExists = await _bucketRepository.DoesS3BucketExistAsync(bucketName);

            if (bucketExists)
            {
                return BadRequest($"Error: S3 Bucket {bucketName} already exists.");
            }

            BucketResponse response = await _bucketRepository.CreateBucketAsync(bucketName);

            return response == null
                ? BadRequest()
                : Ok(response);
        }

        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            BucketResponse response = await _bucketRepository.DeleteS3BucketAsync(bucketName);
            return Ok(response);
        }

        [HttpGet]
        [Route("ListBuckets")]
        public async Task<ActionResult<IEnumerable<ListS3BucketResponse>>> ListBucketsAsync()
        {
            IEnumerable<ListS3BucketResponse> response = await _bucketRepository.ListBucketsAsync();
            return Ok(response);
        }
    }
}
