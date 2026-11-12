public class AvailableRoom
{
    public int RoomId { get; }
    public string AccommodationName { get; }
    public string Location { get; }
    public string RoomNumber { get; }
    public int MaximumPeople { get; }
    public decimal PricePerNight { get; }

    public AvailableRoom(int roomId, string accommodationName, string location, string roomNumber, int maximumPeople, decimal pricePerNight)
    {
        RoomId = roomId;
        AccommodationName = accommodationName;
        Location = location;
        RoomNumber = roomNumber;
        MaximumPeople = maximumPeople;
        PricePerNight = pricePerNight;
    }
}
