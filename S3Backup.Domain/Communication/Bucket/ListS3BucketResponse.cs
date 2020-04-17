using System;
namespace S3Backup.Domain.Communication.Bucket
{
    public class ListS3BucketResponse
    {
        public string BucketName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}