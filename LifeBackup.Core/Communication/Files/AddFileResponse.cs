using System.Collections.Generic;

namespace LifeBackup.Core.Communication.Files
{
    public class AddFileResponse
    {
        public IList<string> PreSignedUrl { get; set; }
    }
}
