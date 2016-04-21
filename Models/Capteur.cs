using SmartHome.Enums;

namespace SmartHome.Models
{
    public class Capteur
    {
        public TypeCapteur Type { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public GrandeurCapteur Grandeur { get; set; }
        public string Box { get; set; }
        public string Lieu { get; set; }
    }
}
