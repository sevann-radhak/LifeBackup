using LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeBackup.Core.Interfaces
{
    public interface IFileRepository
    {
        public AddFileResponse UploadFiles(string bucketName, IList<IFormFile> formFiles);
        public Task<DeleteFileResposne> DeleteFileAsync(string bucketName, string fileName);
        public Task DownloadFileAsync(string bucketName, string fileName);
        public Task<IEnumerable<ListFileResponse>> ListFilesAsync(string bucketName);
    }
}
