using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using S3Backup.Domain.Communication.File;
using S3Backup.Domain.Communication.JsonObject;
using S3Backup.Domain.Interfaces;

namespace S3Backup.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        public FileController(IFileRepository fileRepository)
        {
            this._fileRepository = fileRepository;
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName)
        {
            var response = await _fileRepository.ListObjects(bucketName);
            return Ok(response);
        }

        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles == null)
            {
                return BadRequest("The request 0 file to be uploaded");
            }

            var response = await _fileRepository.UploadFiles(bucketName, formFiles);

            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<ActionResult> DownloadFile(string bucketName, string fileName)
        {
            await _fileRepository.DownloadFile(bucketName, fileName);
            return Ok();
        }

        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            var response = await _fileRepository.DeleteFile(bucketName, fileName);
            return Ok(response);
        }

        [HttpPost]
        [Route("{bucketName}/addjsonobject")]
        public async Task<ActionResult> AddJsonObject(string bucketName, [FromBody]AddJsonObjectRequest request)
        {
            await _fileRepository.AddJsonObject(bucketName, request);
            return Ok();
        }

        [HttpGet]
        [Route("{bucketName}/getjsonobject")]
        public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObject(string bucketName, [FromQuery]string key)
        {
            var response = await _fileRepository.GetJsonObject(bucketName, key);
            return Ok(response);
        }
    }
}