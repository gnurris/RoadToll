using System;

namespace TollFeeCalculator
{
    public class Car : Vehicle
    {
        // Map Car to the domain's CivillianCar vehicle type (the only non-toll-free type).
        public VehicleType Type => VehicleType.CivilianCar;

        public String GetVehicleType()
        {
            return Type.ToString();
        }
    }
}