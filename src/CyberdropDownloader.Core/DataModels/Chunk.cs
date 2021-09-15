namespace CyberdropDownloader.Core.DataModels
{
    public class Chunk
    {
        public long Start { get; set; }
        public byte[] Data { get; set; }
        public long End { get; set; }
    }
}
