
using SmartHome.Converters;
using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SmartHome
{
    public class DataReader
    {
        private XDocument _doc;

        public DataReader()
        {
            _doc = XDocument.Load("../../../datas/capteurs.xtim");
        }

        public List<Capteur> read()
        {
            var capteurs = new List<Capteur>();

            foreach (XElement node in _doc.Descendants("capteurs"))
            {
                capteurs.Add(new Capteur()
                {
                    Type = TypeCapteurConverter.convert(node.Attribute("type").Value),
                    Id = node.Element("id").Value,
                    Description = node.Element("description").Value,
                    Grandeur = new GrandeurCapteur()
                    {
                        nom = node.Element("grandeur").Attribute("nom").Value,
                        unite = node.Element("grandeur").Attribute("unite").Value,
                        abreviation = node.Element("grandeur").Attribute("abreviation").Value
                    },
                    Box = node.Element("box").Value,
                    Lieu = node.Element("lieu").Value
                });
            }

            return capteurs;
        }
    }
}
