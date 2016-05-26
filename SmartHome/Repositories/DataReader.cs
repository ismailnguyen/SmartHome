using SmartHome.Converters;
using SmartHome.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System;
using System.Linq;

namespace SmartHome
{
    public class DataReader
    {
        private XDocument _sensors;
        private List<string> _datas;
        private readonly string _basePath;

        public DataReader()
        {
            _basePath = "../../../";

            _sensors = XDocument.Load(FileSearch("*.xtim").First());
            _datas = FileSearch("*.dt").ToList();
        }

        private IEnumerable<string> FileSearch(string filename)
        {
            return Directory.GetFiles(
                _basePath,
                filename,
                SearchOption.AllDirectories
            );
        }

        public IEnumerable<Sensor> read()
        {
            foreach (var sensor in readSensors())
            {
                sensor.Datas = readDatas(sensor.Id);
                yield return sensor;
            }
        }

        private List<Sensor> readSensors()
        {
            var capteurs = new List<Sensor>();

            if (_sensors.Descendants("capteurs") != null)
            {
                foreach (XElement node in _sensors.Descendants("capteurs").Nodes())
                {
                    var capteur = new Sensor();

                    if (node.Attribute("type") != null)
                    {
                        capteur.Type = SensorTypeConverter.convert(node.Attribute("type").Value);
                    }

                    if (node.Element("id") != null)
                    {
                        capteur.Id = node.Element("id").Value;
                    }

                    if (node.Element("description") != null)
                    {
                        capteur.Description = node.Element("description").Value;
                    }

                    if (node.Element("grandeur") != null)
                    {
                        capteur.Measure = new SensorMeasure();

                        if (node.Element("grandeur").Attribute("nom") != null)
                        {
                            capteur.Measure.Name = node.Element("grandeur").Attribute("nom").Value;
                        }

                        if (node.Element("grandeur").Attribute("unite") != null)
                        {
                            capteur.Measure.Unit = node.Element("grandeur").Attribute("unite").Value;
                        }

                        if (node.Element("grandeur").Attribute("abreviation") != null)
                        {
                            capteur.Measure.Abbreviation = node.Element("grandeur").Attribute("abreviation").Value;
                        }
                    }

                    if (node.Element("valeur") != null)
                    {
                        capteur.Value = new SensorValue();

                        if (node.Element("valeur").Attribute("type") != null)
                        {
                            capteur.Value.Type = node.Element("valeur").Attribute("type").Value;
                        }

                        if (node.Element("valeur").Attribute("min") != null)
                        {
                            capteur.Value.Min = double.Parse(node.Element("valeur").Attribute("min").Value);
                        }

                        if (node.Element("valeur").Attribute("max") != null)
                        {
                            capteur.Value.Max = double.Parse(node.Element("valeur").Attribute("max").Value);
                        }
                    }

                    if (node.Element("box") != null)
                    {
                        capteur.Box = node.Element("box").Value;
                    }

                    if (node.Element("lieu") != null)
                    {
                        capteur.Place = node.Element("lieu").Value;
                    }

                    if (node.Descendants("seuils") != null)
                    {
                        foreach (XElement nodeSeuil in node.Descendants("seuils").Nodes())
                        {
                            if (nodeSeuil.Document.Element("seuil") != null)
                            {
                                var seuil = new SensorTreshold();

                                if (nodeSeuil.Document.Element("seuil").Attribute("description") != null)
                                {
                                    seuil.Description = nodeSeuil.Document.Element("seuil").Attribute("description").Value;
                                }

                                if (nodeSeuil.Document.Element("seuil").Attribute("valeur") != null)
                                {
                                    seuil.Value = double.Parse(nodeSeuil.Document.Element("seuil").Attribute("valeur").Value);
                                }

                                capteur.Tresholds.Add(seuil);
                            }
                        }
                    }

                    capteurs.Add(capteur);
                }
            }
            
            return capteurs;
        }

        private IEnumerable<SmartData> readDatas(string id)
        {
            foreach (var filePath in _datas)
            {
                foreach (var line in File.ReadLines("@" + Path.Combine(Directory.GetCurrentDirectory(), "\\../" + filePath)))
                {
                    var elements = line.Split(' ');

                    if (elements[2].Equals(id))
                    {
                        yield return new SmartData()
                        {
                            Value = double.Parse(elements[3]),
                            Date = DateTime.Parse(
                                elements[0].Substring(1)
                                + " "
                                + elements[1].Substring(0, elements[1].Length - 1)
                            )
                        };
                    }
                }
            }
        }
    }
}
