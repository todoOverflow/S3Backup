using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3Backup.Domain.Communication.Bucket;
using S3Backup.Domain.Interfaces;
using System.Collections.Generic;

namespace S3Backup.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;
        public BucketController(IBucketRepository bucketRepository)
        {
            this._bucketRepository = bucketRepository;
        }

        [HttpPost]
        [Route("create/{bucketname}")]
        public async Task<ActionResult<CreateBucketResponse>> createS3Bucket([FromRoute]string bucketname)
        {
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketname);
            if (bucketExists)
            {
                return BadRequest("S3 bucket already exists");
            }
            var result = await _bucketRepository.PutBucket(bucketname);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<ListS3BucketResponse>>> ListS3Bucket()
        {
            var result = await _bucketRepository.ListBuckets();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteS3Bucket([FromRoute]string bucketName)
        {
            var result = await _bucketRepository.DeleteEmptyBucket(bucketName);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest($"Failed to delete the bucket {bucketName}");
            }
        }
    }
}