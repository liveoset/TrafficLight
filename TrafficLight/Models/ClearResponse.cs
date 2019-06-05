namespace TestTrafficLight.Models
{
    public class ClearResponse : BaseResponse
    {
        public string response { get; set; }

        public ClearResponse() : base("ok")
        {
            response = "ok";
        }
    }
}