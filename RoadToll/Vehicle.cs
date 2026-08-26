namespace TollFeeCalculator
{
    public interface Vehicle
    {
        VehicleType Type { get; }
        String GetVehicleType();
        public bool IsTollFree()
        {
            return Type != VehicleType.CivilianCar;
        }
    }
}