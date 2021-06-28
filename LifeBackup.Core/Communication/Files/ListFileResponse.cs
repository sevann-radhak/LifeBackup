namespace LifeBackup.Core.Communication.Files
{
    public class ListFileResponse
    {
        public string BubketName { get; set; }
        public string Key { get; set; }
        public string Owner { get; set; }
        public long Size { get; set; }
    }
}
