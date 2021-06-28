using LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace LifeBackup.Core.Interfaces
{
    public interface IFileRepository
    {
        public AddFileResponse UploadFiles(string bucketName, IList<IFormFile> formFiles);
    }
}
