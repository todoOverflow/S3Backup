using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3Backup.Domain.Communication.File;
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
    }
}