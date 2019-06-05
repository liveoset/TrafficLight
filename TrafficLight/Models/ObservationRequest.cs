using System;

namespace TestTrafficLight.Models
{
    public class ObservationRequest
    {
        public Observation observation { get; set; }
        public Guid sequence { get; set; }
    }
}