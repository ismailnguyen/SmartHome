using OxyPlot;
using System.Collections.Generic;

namespace SmartHome.ViewModels
{
    public abstract class BaseCapteur
    {
        public string Title { get; set; }
        public IList<DataPoint> Points { get; set; }
    }
}
