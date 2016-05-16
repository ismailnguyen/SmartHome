using SmartHome.Repositories;
using System.Windows;
using OxyPlot.Axes;
using OxyPlot.Series;
using SmartHome.ViewModels;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BaseRepository _repository;
        
        public MainWindow()
        {
            InitializeComponent();

            _repository = new BaseRepository();

            initCombos();
        }

        private void initCombos()
        {
            foreach (var capteur in _repository.Capteurs)
            {
                if (!choiceBox.Items.Contains(capteur.Box))
                {
                    choiceBox.Items.Add(capteur.Box);
                }
            }
            
            choiceLieu.IsEnabled = false;
            calendar.IsEnabled = false;
        }

        private void choiceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choiceLieu.Items.Clear();

            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;

            if (selectedItem != null && selectedItem.Length > 0)
            {
                foreach (var capteur in _repository.Capteurs)
                {
                    if (capteur.Box == selectedItem)
                    {
                        if (!choiceLieu.Items.Contains(capteur.Lieu))
                        {
                            choiceLieu.Items.Add(capteur.Lieu);
                        }
                    }
                }

                choiceLieu.IsEnabled = true;
            }
            else
            {   
                choiceLieu.IsEnabled = false;
            }
        }

        private void choiceLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;
            string box = choiceBox.SelectedItem as string;
            var dates = new List<DateTime>();

            if (selectedItem != null && selectedItem.Length > 0)
            {
                foreach (var capteur in _repository.Capteurs)
                {
                    if (capteur.Box == box
                        && capteur.Lieu == selectedItem)
                    {
                        foreach (var data in capteur.Datas)
                        {
                            var date = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day);

                            if (!dates.Contains(date))
                            {
                                dates.Add(date);
                            }
                        }
                    }
                }

                var firstDate = dates.First();
                var lastDate = dates.Last();
                var dateCounter = firstDate;


                foreach (var d in dates.Skip(1))
                {
                    if (d.AddDays(-1).Date != dateCounter.Date)
                    {
                        calendar.BlackoutDates.Add(
                            new CalendarDateRange(dateCounter.AddDays(1), d.AddDays(-1)));
                    }

                    dateCounter = d;
                }

                calendar.DisplayDateStart = firstDate;
                calendar.DisplayDateEnd = lastDate;

                calendar.IsEnabled = true;
            }
            else
            {
                calendar.IsEnabled = false;
            }
        }

        private void calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            
            if (calendar.SelectedDate != null)
            {
                var selectedDate = calendar.SelectedDate.Value;

                drawGraphs(selectedDate);
            }
        }

        private void drawGraphs(DateTime date)
        {
            Plotter.Capteur.Series.Clear();

            string box = choiceBox.SelectedItem as string;
            string lieu = choiceLieu.SelectedItem as string;

            foreach (var capteur in _repository.Capteurs)
            {
                if (capteur.Box == box
                    && capteur.Lieu == lieu)
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
                        if (data.Date.Year == date.Year
                            && data.Date.Month == date.Month
                            && data.Date.Day == date.Day)
                        {
                            lineSerie.Points.Add(
                                new OxyPlot.DataPoint(
                                    DateTimeAxis.ToDouble(data.Date.TimeOfDay),
                                    data.Valeur
                                )
                            );
                        }
                    }

                    Plotter.Capteur.Series.Add(lineSerie);
                    Plotter.Capteur.InvalidatePlot(true);
                }
            }
        }
    }
}
