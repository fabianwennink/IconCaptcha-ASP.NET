using System;

namespace IconCaptcha
{
    public class SubmissionException : Exception
    {
        public int Code { get; set; }
        
        public SubmissionException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
