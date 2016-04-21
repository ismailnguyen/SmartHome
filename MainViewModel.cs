using System;
using OxyPlot;
using OxyPlot.Series;

namespace SmartHome
{
    public class MainViewModel
    {
        public PlotModel Humidité { get; set; }

        public MainViewModel()
        {
            Humidité = new PlotModel
            {
                Title = "Humidité"
            };
        
        
            Humidité.Series.Add(
                new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)")
            );
        }
    }
}
