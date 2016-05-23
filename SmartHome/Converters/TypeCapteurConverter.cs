using SmartHome.Enums;
using System;

namespace SmartHome.Converters
{
    public class TypeCapteurConverter
    {
        public static TypeCapteur convert(String type)
        {
            switch (type)
            {
                case "mesure":
                    return TypeCapteur.Mesure;

                case "objectif":
                    return TypeCapteur.Objectif;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
