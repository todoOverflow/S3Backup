using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using S3Backup.Domain.Communication.Bucket;
using S3Backup.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;

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

        public async Task<IEnumerable<ListS3BucketResponse>> ListBuckets()
        {
            var response = await _s3Client.ListBucketsAsync();
            return response.Buckets.Select(b => new ListS3BucketResponse
            {
                BucketName = b.BucketName,
                CreationDate = b.CreationDate
            });
        }

        public async Task<bool> DeleteEmptyBucket(string bucketName)
        {
            if (!await _s3Client.DoesS3BucketExistAsync(bucketName))
            {
                return false;
            }
            try
            {
                await _s3Client.DeleteBucketAsync(bucketName);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}