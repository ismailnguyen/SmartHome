using System.Collections.Generic;
using OxyPlot;

namespace SmartHome.ViewModels
{
    public class Humidité : BaseCapteur
    {
        public Humidité()
        {
            Title = "Humidité";
            Points = new List<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };
        }
    }
}
