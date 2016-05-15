using System;
using OxyPlot;
using OxyPlot.Series;
using SmartHome.ViewModels;

namespace SmartHome
{
    public class MainViewModel
    {
        public Humidité Humidité { get; set; }

        public MainViewModel()
        {

            Humidité = new Humidité();
   
        }
    }
}
