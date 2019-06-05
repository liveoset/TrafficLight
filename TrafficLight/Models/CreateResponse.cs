using System;

namespace TestTrafficLight.Models
{
    public class CreateResponse : BaseResponse
    {
        public CreateResponse(Guid seq) : base("ok")
        {
            response = new SequenceResponse() { sequence = seq };
        }

        public SequenceResponse response { get; set; }
    }
}