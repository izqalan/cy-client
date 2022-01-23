using System;
using System.Runtime.Serialization;

namespace CyberdropDownloader.Core.Exceptions
{
    public class NullAlbumFilesException : Exception
    {
        public NullAlbumFilesException()
        {
        }

        public NullAlbumFilesException(string message) : base(message)
        {
        }

        public NullAlbumFilesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullAlbumFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
