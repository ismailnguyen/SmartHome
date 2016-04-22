
using SmartHome.Converters;
using SmartHome.Models;
using System.Linq;
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

            //TO-DO:
            //Read .dt files
            //Get lines according to parameter 'id'
            //Add to datas

            return datas;
        }
    }
}
