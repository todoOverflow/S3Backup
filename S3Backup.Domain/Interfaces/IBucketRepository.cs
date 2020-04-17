using System.Collections.Generic;
using System.Threading.Tasks;
using S3Backup.Domain.Communication.Bucket;

namespace S3Backup.Domain.Interfaces
{
    public interface IBucketRepository
    {
        Task<bool> DoesS3BucketExist(string bucketname);
        Task<CreateBucketResponse> PutBucket(string bucketname);
        Task<IEnumerable<ListS3BucketResponse>> ListBuckets();
        Task<bool> DeleteEmptyBucket(string bucketName);
    }
}