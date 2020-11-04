using System;

namespace Code.Network
{
    public sealed class RequestResult
    {
        public byte[] Data { get; set; }
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
    }
}