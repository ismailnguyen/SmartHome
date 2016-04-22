using OxyPlot;
using System.Collections.Generic;

namespace SmartHome.ViewModels
{
    public abstract class BaseCapteur
    {
        protected string Title { get; set; }
        protected IList<DataPoint> Points { get; set; }
    }
}
