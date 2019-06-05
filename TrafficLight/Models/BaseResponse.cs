namespace TestTrafficLight.Models
{
    public class BaseResponse
    {
        public string status { get;set; }

        protected BaseResponse(string stat)
        {
            status = stat;
        }
    }
}