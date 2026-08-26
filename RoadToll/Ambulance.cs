namespace TollFeeCalculator
{
    public class Ambulance : Vehicle
    {
        public VehicleType Type => VehicleType.Emergency;

        public string GetVehicleType()
        {
            return Type.ToString();
        }
    }
}