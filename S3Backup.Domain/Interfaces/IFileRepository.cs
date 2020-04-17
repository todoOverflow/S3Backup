using System.Collections.Generic;
using System.Threading.Tasks;
using S3Backup.Domain.Communication.File;

namespace S3Backup.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<ListFilesResponse>> ListObjects(string bucketName);
    }
}