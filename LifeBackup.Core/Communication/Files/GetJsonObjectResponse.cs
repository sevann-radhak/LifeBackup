using System;

namespace LifeBackup.Core.Communication.Files
{
    public class GetJsonObjectResponse
    {
        public Guid Id { get; set; }
        public string Data { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
