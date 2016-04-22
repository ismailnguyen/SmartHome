
using SmartHome.Converters;
using SmartHome.Models;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System;

namespace SmartHome
{
    public class DataReader
    {
        private XDocument _doc;
        private string[] _netatmoDatas;

        public DataReader()
        {
            _doc = XDocument.Load("../../../datas/capteurs.xtim");
            _netatmoDatas = Directory.GetFiles(@"../../../datas/netatmo/", "*.dt", SearchOption.AllDirectories);
        }

        public List<Capteur> read()
        {
            var capteurs = readCapteurs();

            capteurs.ForEach(d =>
                d.Datas = readDatas(d.Id)
            );

            return capteurs;
        }

        private List<Capteur> readCapteurs()
        {
            var capteurs = new List<Capteur>();

            foreach (XElement node in _doc.Descendants("capteurs").Nodes())
            {
                var capteur = new Capteur()
                {
                    Type = TypeCapteurConverter.convert(node.Attribute("type").Value),
                    Id = node.Element("id").Value,
                    Description = node.Element("description").Value,
                    Grandeur = new GrandeurCapteur()
                    {
                        Nom = node.Element("grandeur").Attribute("nom").Value,
                        Unite = node.Element("grandeur").Attribute("unite").Value,
                        Abreviation = node.Element("grandeur").Attribute("abreviation").Value
                    },
                    Valeur = new ValeurCapteur()
                    {
                        Type = node.Element("valeur").Attribute("type").Value,
                        Min = double.Parse(node.Element("valeur").Attribute("min").Value),
                        Max = double.Parse(node.Element("valeur").Attribute("max").Value)
                    },
                    Box = node.Element("box").Value,
                    Lieu = node.Element("lieu").Value
                };

                foreach (XElement nodeSeuil in node.Descendants("seuils").Nodes())
                {
                    var seuil = new SeuilCapteur()
                    {
                        Description = nodeSeuil.Document.Element("seuil").Attribute("description").Value,
                        Valeur = double.Parse(nodeSeuil.Document.Element("seuil").Attribute("valeur").Value)
                    };

                    capteur.Seuils.Add(seuil);
                }

                capteurs.Add(capteur);
            }

            return capteurs;
        }

        private List<SmartData> readDatas(string id)
        {
            var datas = new List<SmartData>();

            foreach (string filePath in _netatmoDatas)
            {
                foreach (string line in File.ReadLines("@" + filePath))
                {              
                    var elements = line.Split(' ');

                    if (elements[2].Equals(id))
                    {
                        var data = new SmartData()
                        {
                            Date = DateTime.Parse(
                                elements[0].Substring(1)
                                + elements[1].Substring(0, elements[1].Length - 1)
                            ),
                            Valeur = double.Parse(elements[3])
                        };

                        datas.Add(data);
                    }
                }
            }

            return datas;
        }
    }
}
