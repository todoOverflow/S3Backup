using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using S3Backup.Domain.Communication.File;
using S3Backup.Domain.Interfaces;

namespace S3Backup.Infrastructure.Repository
{
    public class FileResository : IFileRepository
    {
        private readonly IAmazonS3 _s3Client;
        public FileResository(IAmazonS3 s3Client)
        {
            this._s3Client = s3Client;
        }

        public async Task<IEnumerable<ListFilesResponse>> ListObjects(string bucketName)
        {
            var response = await _s3Client.ListObjectsAsync(bucketName);

            return response.S3Objects.Select(file => new ListFilesResponse
            {
                BucketName = file.BucketName,
                Key = file.Key,
                OwnerDisplayName = file.Owner.DisplayName,
                Size = file.Size
            });
        }
    }
}