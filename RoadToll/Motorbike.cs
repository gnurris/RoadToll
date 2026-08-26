using System;

namespace TollFeeCalculator
{
    public class Motorbike : Vehicle
    {
        public VehicleType Type => VehicleType.Motorbike;

        public string GetVehicleType()
        {
            return Type.ToString();
        }
    }
}