namespace S3Backup.Domain.Communication.File
{
    public class ListFilesResponse
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string OwnerDisplayName { get; set; }
        public long Size { get; set; }

    }
}