using OxyPlot.Axes;
using OxyPlot.Series;
using SmartHome.Repositories;
using SmartHome.ViewModels;
using System;

namespace SmartHome
{
    public class Plotter
    {
        public static Capteur Capteur { get; set; }

        public Plotter()
        {
            Capteur = new Capteur();
        }
    }
}
