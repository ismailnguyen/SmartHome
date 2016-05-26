using OxyPlot;

namespace SmartHome
{
    public class Plotter
    {
        public static PlotModel Sensor { get; set; }

        public Plotter()
        {
            Sensor = new PlotModel();
        }
    }
}
