using System.Collections.Generic;

namespace S3Backup.Domain.Communication.File
{
    public class AddFileResponse
    {
        public IList<string> PreSignedURLs { get; set; }
    }
}