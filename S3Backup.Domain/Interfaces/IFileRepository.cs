using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using S3Backup.Domain.Communication.File;
using S3Backup.Domain.Communication.JsonObject;

namespace S3Backup.Domain.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<ListFilesResponse>> ListObjects(string bucketName);
        Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles);
        Task DownloadFile(string bucketName, string fileName);
        Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName);
        Task AddJsonObject(string bucketName, AddJsonObjectRequest request);
        Task<GetJsonObjectResponse> GetJsonObject(string bucketName, string key);
    }
}