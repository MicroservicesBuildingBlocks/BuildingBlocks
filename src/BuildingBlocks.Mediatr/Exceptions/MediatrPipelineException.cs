using System;

namespace BuildingBlocks.Mediatr.Validators
{
    public class MediatrPipelineException : Exception
    {
        public MediatrPipelineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}