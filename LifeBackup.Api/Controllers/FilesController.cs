using LifeBackup.Core.Communication.Files;
using LifeBackup.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeBackup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IFileRepository _fileRepository;

        public FilesController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles is null)
            {
                return BadRequest("The request does not contain any files to be uploaded.");
            }

            AddFileResponse response = _fileRepository.UploadFiles(bucketName, formFiles);

            if(response is null)
            {
                return BadRequest();
            }

            return Ok(response);
        }
    }
}
