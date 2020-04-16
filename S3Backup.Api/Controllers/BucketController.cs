using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3Backup.Domain.Communication.Bucket;
using S3Backup.Domain.Interfaces;

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
    }
}