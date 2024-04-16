namespace API.Errors
{
    public class APIException
    {
        public APIException(int statuscode,string message,string details)
        {
            StatusCode=statuscode;
            Message=message;
            Details=details;

        }

        public int StatusCode{get;set;}
        public string Message{get;set;}

        public string Details{get;set;}
    }
}