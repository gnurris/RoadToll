namespace TollFeeCalculator
{
    public class Tank : Vehicle
    {
        public VehicleType Type => VehicleType.Military;

        public string GetVehicleType()
        {
            return Type.ToString();
        }
    }
}