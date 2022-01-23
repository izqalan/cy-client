using System;
using System.Runtime.Serialization;

namespace CyberdropDownloader.Core.Exceptions
{
    public class NullAlbumSizeException : Exception
    {
        public NullAlbumSizeException()
        {
        }

        public NullAlbumSizeException(string message) : base(message)
        {
        }

        public NullAlbumSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullAlbumSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
