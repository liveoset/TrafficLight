using System.Collections.Generic;

namespace TestTrafficLight.Models
{
    public class ObservationResponse : BaseResponse
    {
        public ObservationResponse(IList<byte> numbers, byte mis1, byte mis2) : base("ok")
        {
            this.numbers = numbers;
            this.missing = new string[2]
            {
                mis1.ConvertToBString(),
                mis2.ConvertToBString()
            };

        }

        public IList<byte> numbers { get; set; }
        public string[] missing { get; set; }
    }
}