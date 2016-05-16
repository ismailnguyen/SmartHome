using SmartHome.Models;
using System.Collections.Generic;

namespace SmartHome.Repositories
{
    public class BaseRepository
    {
        private DataReader _dataReader;
        public List<Capteur> Capteurs { get; set; }

        public BaseRepository()
        {
            _dataReader = new DataReader();
            Capteurs = _dataReader.read();
        }
    }
}
