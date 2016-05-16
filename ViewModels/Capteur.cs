using OxyPlot;
using OxyPlot.Axes;

namespace SmartHome.ViewModels
{
    public class Capteur : PlotModel
    {
        public Capteur()
        {
            Title = "Netatmo";

            Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 100,
                Title = "Valeurs",
                PositionAtZeroCrossing = true
            });

            Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Temps",
                StringFormat = "HH:mm",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80
            });

        }
    }
}
