using SmartHome.Enums;
using System.Collections.Generic;

namespace SmartHome.Models
{
    public class Sensor
    {
        public SensorType Type { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public SensorMeasure Measure { get; set; }
        public SensorValue Value { get; set; }
        public string Box { get; set; }
        public string Place { get; set; }
        public List<SensorTreshold> Tresholds { get; set; }
        public IEnumerable<SmartData> Datas { get; set; }
    }
}
