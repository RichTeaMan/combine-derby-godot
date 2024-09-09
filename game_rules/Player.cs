public record Player
{
    public int Id { get; init; }

    public string VehicleName { get; init; }

    public Player(int id, string vehicleName)
    {
        Id = id;
        VehicleName = vehicleName;
    }
}
