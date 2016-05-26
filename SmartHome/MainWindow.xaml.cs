using SmartHome.Repositories;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Linq;
using OxyPlot;
using System.Threading;

namespace SmartHome
{
    public partial class MainWindow
    {
        private BaseRepository _repository;
        
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr");

            InitializeComponent();

            _repository = new BaseRepository();

            InitializeFields();
        }

        private void InitializeFields()
        {
            _repository.Capteurs.ForEach(
                capteur =>
                {
                    if (!choiceBox.Items.Contains(capteur.Box))
                    {
                        choiceBox.Items.Add(capteur.Box);
                    }
                }
            );

            choiceLieu.IsEnabled = false;
            choiceGrandeur.IsEnabled = false;
            calendarStart.IsEnabled = false;
            calendarEnd.IsEnabled = false;
        }

        private void choiceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choiceLieu.Items.Clear();

            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Capteurs
                    .Where(capteur => capteur.Box == selectedItem)
                    .ToList()
                    .ForEach(
                        capteur =>
                        {
                            if (!choiceLieu.Items.Contains(capteur.Lieu))
                            {
                                choiceLieu.Items.Add(capteur.Lieu);
                            }
                        }
                );

                choiceLieu.IsEnabled = true;
            }
            else
            {   
                choiceLieu.IsEnabled = false;
            }
        }

        private void choiceLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choiceGrandeur.Items.Clear();

            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;
            string box = choiceBox.SelectedItem as string;

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Capteurs
                    .Where(capteur => capteur.Box == box && capteur.Lieu == selectedItem)
                    .ToList()
                    .ForEach(
                        capteur =>
                        {
                            if (!choiceGrandeur.Items.Contains(capteur.Grandeur.Nom))
                            {
                                choiceGrandeur.Items.Add(capteur.Grandeur.Nom);
                            }
                        }
                );

                choiceGrandeur.IsEnabled = true;
            }
            else
            {
                choiceGrandeur.IsEnabled = false;
            }
        }

        private void choiceGrandeur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;
            string box = choiceBox.SelectedItem as string;
            string lieu = choiceLieu.SelectedItem as string;
            var dates = new List<DateTime>();

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Capteurs
                    .Where(capteur => capteur.Box == box 
                            && capteur.Lieu == lieu 
                            && capteur.Grandeur.Nom == selectedItem)
                    .ToList()
                    .ForEach(
                        capteur =>
                        {
                            capteur.Datas
                            .ToList()
                            .ForEach(
                                data =>
                                {
                                    var date = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day);

                                    if (!dates.Contains(date))
                                    {
                                        dates.Add(date);
                                    }
                                }
                            );
                        }    
                );

                if (dates.Count > 0)
                {
                    var firstDate = dates.First();
                    var lastDate = dates.Last();
                    var dateCounter = firstDate;
                    
                    foreach (var d in dates.Skip(1))
                    {
                        if (d.AddDays(-1).Date != dateCounter.Date)
                        {
                            calendarStart.BlackoutDates.Add(
                                new CalendarDateRange(dateCounter.AddDays(1), d.AddDays(-1)));

                            calendarEnd.BlackoutDates.Add(
                                new CalendarDateRange(dateCounter.AddDays(1), d.AddDays(-1)));
                        }

                        dateCounter = d;
                    }
                    
                    // Set calendars default date from start and end of existing datas dates
                    calendarStart.SelectedDate = firstDate;
                    calendarStart.DisplayDate = firstDate;

                    calendarEnd.SelectedDate = lastDate;
                    calendarEnd.DisplayDate = lastDate;
                }

                calendarStart.IsEnabled = true;
                calendarEnd.IsEnabled = true;
            }   
            else
            {
                calendarStart.IsEnabled = false;
                calendarEnd.IsEnabled = false;
            }
        }

        private void calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendarStart != null
                && calendarStart.SelectedDate != null
                && calendarEnd != null
                && calendarEnd.SelectedDate != null)
            {
                drawGraphs(
                    calendarStart.SelectedDate.Value,
                    calendarEnd.SelectedDate.Value
                );
            }
        }

        private void drawGraphs(DateTime startDate, DateTime endDate)
        {
            Plotter.Capteur.Series.Clear();
            Plotter.Capteur.Axes.Clear();

            var box = choiceBox.SelectedItem as string;
            var lieu = choiceLieu.SelectedItem as string;
            var grandeur = choiceGrandeur.SelectedItem as string;
            var min = 0.0;
            var max = 0.0;

            foreach (var capteur in _repository.Capteurs)
            {
                if (capteur.Box == box
                    && capteur.Lieu == lieu
                    && capteur.Grandeur.Nom == grandeur)
                {
                    var lineSerie = new LineSeries()
                    {
                        StrokeThickness = 2,
                        MarkerSize = 3,
                        CanTrackerInterpolatePoints = false,
                        Title = capteur.Description,
                        Smooth = false,
                    };

                    var seuilSerie = new LineSeries();
                    var seuil = capteur.Seuils != null && capteur.Seuils.Count() > 0
                        ? capteur.Seuils.Average(x => x.Valeur) 
                        : 0;

                    foreach (var data in capteur.Datas)
                    {
                        if (data.Date >= startDate
                                && data.Date <= endDate)
                        {
                            lineSerie.Points.Add(
                                new DataPoint(
                                    Axis.ToDouble(data.Date),
                                    data.Valeur
                                )
                            );

                            if (seuil != 0)
                            {
                                seuilSerie.Points.Add(
                                    new DataPoint(
                                        Axis.ToDouble(data.Date),
                                        seuil
                                        )
                                    );
                            }

                            if (data.Valeur < min)
                            {
                                min = data.Valeur;
                            }

                            if (data.Valeur > max)
                            {
                                max = data.Valeur;
                            }
                        }
                    }

                    Plotter.Capteur.Title = capteur.Lieu + " (" + capteur.Box  + ")";

                    Plotter.Capteur.Axes.Add(new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = (capteur.Valeur != null ? capteur.Valeur.Min : min) - 10,
                        Maximum = (capteur.Valeur != null ? capteur.Valeur.Max : max) + 10,
                        Title = capteur.Grandeur.Unite + "(" + capteur.Grandeur.Abreviation + ")",
                        PositionAtZeroCrossing = true
                    });

                    Plotter.Capteur.Axes.Add(new DateTimeAxis()
                    {
                        Position = AxisPosition.Bottom,
                        Title = "Date",
                        StringFormat = "dddd MM-dd-yyyy HH:mm",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        MinorIntervalType = DateTimeIntervalType.Hours,
                        IntervalType = DateTimeIntervalType.Hours,
                        IntervalLength = 80
                    });

                    Plotter.Capteur.Series.Add(lineSerie);

                    if (seuilSerie.Points.Count > 0)
                    {
                        Plotter.Capteur.Series.Add(seuilSerie);
                    }

                    Plotter.Capteur.InvalidatePlot(true);
                }
            }
        }
    }
}
