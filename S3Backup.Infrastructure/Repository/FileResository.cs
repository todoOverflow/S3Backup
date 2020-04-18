using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using S3Backup.Domain.Communication.File;
using S3Backup.Domain.Communication.JsonObject;
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

        public async Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles)
        {
            var preSignedURLs = new List<string>();

            using (var transferUtility = new TransferUtility(_s3Client))
            {
                foreach (var file in formFiles)
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = file.OpenReadStream(),
                        Key = file.FileName,
                        BucketName = bucketName,
                        // Owner gets FULL_CONTROL. No one else has access rights (default).
                        CannedACL = S3CannedACL.NoACL
                    };
                    await transferUtility.UploadAsync(uploadRequest);

                    var getPreSignedUrlRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = bucketName,
                        Key = file.FileName,
                        Expires = DateTime.Now.AddDays(3),
                    };
                    var url = _s3Client.GetPreSignedURL(getPreSignedUrlRequest);
                    preSignedURLs.Add(url);
                }
            }

            return new AddFileResponse
            {
                PreSignedURLs = preSignedURLs
            };
        }


        public async Task DownloadFile(string bucketName, string fileName)
        {
            var saveFilePath = $"C:\\temp\\{fileName}";

            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = saveFilePath,
            };

            using (var transferUtility = new TransferUtility(_s3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }

        public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
        {
            var deleteObjectsReqeust = new DeleteObjectsRequest
            {
                BucketName = bucketName
            };
            deleteObjectsReqeust.AddKey(fileName);
            var response = await _s3Client.DeleteObjectsAsync(deleteObjectsReqeust);

            return new DeleteFileResponse
            {
                NumberOfDeleteObjects = response.DeletedObjects.Count,
            };
        }

        public async Task AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            var createdOnUtc = DateTime.UtcNow;
            var s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:dd}/{request.Id}";
            var body = JsonConvert.SerializeObject(request);

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = body,
            };

            await _s3Client.PutObjectAsync(putObjectRequest);

        }

        public async Task<GetJsonObjectResponse> GetJsonObject(string bucketName, string key)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(getObjectRequest);

            using (var sr = new System.IO.StreamReader(response.ResponseStream))
            {
                var contents = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<GetJsonObjectResponse>(contents);
            }
        }
    }
}