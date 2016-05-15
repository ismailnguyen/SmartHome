using SmartHome.Repositories;
using System.Windows;
using OxyPlot.Axes;
using OxyPlot.Series;
using SmartHome.ViewModels;
using System.Collections.Generic;

namespace SmartHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BaseRepository _repository;
        //private Plotter _plotter;

        public MainWindow()
        {
            InitializeComponent();

            //_plotter = new Plotter();

            _repository = new BaseRepository();
            drawGraphs();
            initCombos();
        }

        private void drawGraphs()
        {
            foreach (var capteur in _repository.Capteurs)
            {
                if (capteur.Id == "temperaturearrieremaison")
                {
                    var lineSerie = new LineSeries()
                    {
                        StrokeThickness = 2,
                        MarkerSize = 3,
                        CanTrackerInterpolatePoints = false,
                        Title = capteur.Description,
                        Smooth = false,
                    };

                    foreach (var data in capteur.Datas)
                    {
                        lineSerie.Points.Add(
                            new OxyPlot.DataPoint(
                                DateTimeAxis.ToDouble(data.Date),
                                data.Valeur
                            )
                        );
                    }

                    Plotter.Capteur.Series.Add(lineSerie);
                }
            }
        }

        private void initCombos()
        {
            foreach (var capteur in _repository.Capteurs)
            {
                if (!choice_box.Items.Contains(capteur.Box))
                {
                    choice_box.Items.Add(capteur.Box);
                }
            }
            
            choice_lieu.IsEnabled = false;
            choice_date.IsEnabled = false;
        }
    }
}
