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
            InitializeCalendars();
        }

        private void InitializeFields()
        {
            _repository.Sensors
                .ToList()
                .ForEach(
                capteur =>
                {
                    if (!choiceBox.Items.Contains(capteur.Box))
                    {
                        choiceBox.Items.Add(capteur.Box);
                    }
                }
            );

            choicePlace.IsEnabled = false;
            choiceMeasure.IsEnabled = false;
        }

        private void InitializeCalendars()
        {
            var dates = new List<DateTime>();

            _repository.Sensors
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

            SetupCalendars(dates);
        }
        
        private void SetupCalendars(List<DateTime> dates)
        {
            if (dates.Count > 0)
            {
                var firstDate = dates.First();
                var lastDate = dates.First().AddDays(1);
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
        }

        private void choiceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choicePlace.Items.Clear();

            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Sensors
                    .Where(capteur => capteur.Box == selectedItem)
                    .ToList()
                    .ForEach(
                        capteur =>
                        {
                            if (!choicePlace.Items.Contains(capteur.Place))
                            {
                                choicePlace.Items.Add(capteur.Place);
                            }
                        }
                );

                choicePlace.IsEnabled = true;
            }
            else
            {   
                choicePlace.IsEnabled = false;
            }
        }

        private void choiceLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choiceMeasure.Items.Clear();

            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;
            string box = choiceBox.SelectedItem as string;

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Sensors
                    .Where(capteur => capteur.Box == box && capteur.Place == selectedItem)
                    .ToList()
                    .ForEach(
                        capteur =>
                        {
                            if (!choiceMeasure.Items.Contains(capteur.Measure.Name))
                            {
                                choiceMeasure.Items.Add(capteur.Measure.Name);
                            }
                        }
                );

                choiceMeasure.IsEnabled = true;
            }
            else
            {
                choiceMeasure.IsEnabled = false;
            }
        }

        private void choiceGrandeur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = sender as ComboBox;
            var selectedItem = choice.SelectedItem as string;
            string box = choiceBox.SelectedItem as string;
            string lieu = choicePlace.SelectedItem as string;
            var dates = new List<DateTime>();

            if (selectedItem != null && selectedItem.Length > 0)
            {
                _repository.Sensors
                    .Where(capteur => capteur.Box == box 
                            && capteur.Place == lieu 
                            && capteur.Measure.Name == selectedItem)
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

                SetupCalendars(dates);
                drawGraphs();
            }
        }

        private void calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drawGraphs();
        }

        private void drawGraphs()
        {
            if (calendarStart != null
                && calendarStart.SelectedDate != null
                && calendarEnd != null
                && calendarEnd.SelectedDate != null)
            {
                var startDate = calendarStart.SelectedDate.Value;
                var endDate = calendarEnd.SelectedDate.Value;
            

                Plotter.Sensor.Series.Clear();
                Plotter.Sensor.Axes.Clear();

                var box = choiceBox.SelectedItem as string;
                var place = choicePlace.SelectedItem as string;
                var measure = choiceMeasure.SelectedItem as string;
                var min = 0.0;
                var max = 0.0;

                var capteurs = _repository.Sensors
                    .Where(capteur =>
                        (box != null && capteur.Box == box)
                        && (place != null && capteur.Place == place)
                        && (measure != null && capteur.Measure.Name == measure)
                );

                foreach (var capteur in capteurs)
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
                    var seuil = capteur.Tresholds != null && capteur.Tresholds.Count() > 0
                        ? capteur.Tresholds.Average(x => x.Value) 
                        : 0;

                    foreach (var data in capteur.Datas)
                    {
                        if (data.Date >= startDate
                                && data.Date <= endDate)
                        {
                            lineSerie.Points.Add(
                                new DataPoint(
                                    Axis.ToDouble(data.Date),
                                    data.Value
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

                            if (data.Value < min)
                            {
                                min = data.Value;
                            }

                            if (data.Value > max)
                            {
                                max = data.Value;
                            }
                        }
                    }

                    Plotter.Sensor.Title = capteur.Place + " (" + capteur.Box  + ")";

                    Plotter.Sensor.Axes.Add(new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = (capteur.Value != null ? capteur.Value.Min : min) - 10,
                        Maximum = (capteur.Value != null ? capteur.Value.Max : max) + 10,
                        Title = capteur.Measure.Unit + "(" + capteur.Measure.Abbreviation + ")",
                        PositionAtZeroCrossing = true
                    });

                    Plotter.Sensor.Axes.Add(new DateTimeAxis()
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

                    Plotter.Sensor.Series.Add(lineSerie);

                    if (seuilSerie.Points.Count > 0)
                    {
                        Plotter.Sensor.Series.Add(seuilSerie);
                    }

                    Plotter.Sensor.InvalidatePlot(true);
                }
            }
        }
    }
}
