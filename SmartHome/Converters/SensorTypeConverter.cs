using SmartHome.Enums;
using System;

namespace SmartHome.Converters
{
    public class SensorTypeConverter
    {
        public static SensorType convert(string type)
        {
            switch (type)
            {
                case "mesure":
                    return SensorType.Measure;

                case "objectif":
                    return SensorType.Goal;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
