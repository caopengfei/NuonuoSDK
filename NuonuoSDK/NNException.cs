using System;

namespace NuonuoSDK
{
    public sealed class NNException : ApplicationException
    {
        private readonly string error;
        private readonly Exception innerException;

        public NNException(string msg)
            : base(msg)
        {
            this.error = msg;
        }

        public NNException(string msg, Exception innerException)
            : base(msg)
        {
            this.innerException = innerException;
            this.error = msg;

        }
    }
}
