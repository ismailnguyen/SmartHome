using OxyPlot;

namespace SmartHome
{
    public class Plotter
    {
        public static PlotModel Capteur { get; set; }

        public Plotter()
        {
            Capteur = new PlotModel();
        }
    }
}
