using System;
using System.Runtime.Serialization;

namespace CyberdropDownloader.Core.Exceptions
{
    public class AllServersAreUnreachable : Exception
    {
        public AllServersAreUnreachable()
        {
        }

        public AllServersAreUnreachable(string message) : base(message)
        {
        }

        public AllServersAreUnreachable(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AllServersAreUnreachable(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
