using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using S3Backup.Domain.Communication.Bucket;
using S3Backup.Domain.Interfaces;
using System.Linq;

namespace S3Backup.Infrastructure.Repository
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;
        public BucketRepository(IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
        }

        public async Task<bool> DoesS3BucketExist(string bucketname)
        {
            return await _s3Client.DoesS3BucketExistAsync(bucketname);
        }

        public async Task<CreateBucketResponse> PutBucket(string bucketname)
        {
            //using Amazon.S3.Model;
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketname,
                UseClientRegion = true
            };

            var response = await _s3Client.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse
            {
                BucketName = bucketname,
                RequestId = response.ResponseMetadata.RequestId
            };
        }
    }
}