namespace ApplicationCore.Exceptions
{
    public class ErrorResponseModel
    {
        // For use in development environments.
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string InnerExceptionMessage { get; set; }


    }
}
