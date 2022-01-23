using System;
using System.Runtime.Serialization;

namespace CyberdropDownloader.Core.Exceptions
{
    public class NullAlbumTitleException : Exception
    {
        public NullAlbumTitleException()
        {
        }

        public NullAlbumTitleException(string message) : base(message)
        {
        }

        public NullAlbumTitleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullAlbumTitleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
