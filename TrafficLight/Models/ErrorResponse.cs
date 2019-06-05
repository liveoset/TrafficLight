namespace TestTrafficLight.Models
{
    public class ErrorResponse : BaseResponse
    {
        public string msg { get; set; }

        public ErrorResponse(string message) : base("error")
        {
            msg = message;
        }
    }
}