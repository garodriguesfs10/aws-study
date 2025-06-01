using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace demo.aws.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;

        public BucketsController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBucketAsync(string name) 
        {
            var bucketsExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, name);
            if (bucketsExists) return BadRequest("Bucket with this name already exists");
            await _s3Client.PutBucketAsync(name);
            return Created("buckets", $"Bucket {name} created");
        }

        [HttpGet]
        public async Task<IActionResult> ListBucketsAsync()
        {
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBucketAsync(string name)
        {
            var bucketsExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, name);
            if (!bucketsExists) return BadRequest("Bucket with this name doesn't exists");
            await _s3Client.DeleteBucketAsync(name);
            return Ok();
        }

    }
}
