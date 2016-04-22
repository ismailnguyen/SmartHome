using SmartHome.Models;
using System.Collections.Generic;

namespace SmartHome
{
    public class MainApplication
    {
        private DataReader _dataReader;
        public List<Capteur> Capteurs { get; set; }

        public MainApplication()
        {
            _dataReader = new DataReader();
            Capteurs = _dataReader.read();
        }
    }
}
