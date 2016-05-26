using SmartHome.Models;
using System.Collections.Generic;

namespace SmartHome.Repositories
{
    public class BaseRepository
    {
        private DataReader _dataReader;
        public IEnumerable<Sensor> Sensors { get; set; }

        public BaseRepository()
        {
            _dataReader = new DataReader();
            Sensors = _dataReader.read();
        }
    }
}
