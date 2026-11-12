public class AvailableRoom
{
    public int RoomId { get; }
    public string AccommodationName { get; }
    public string Location { get; }
    public string RoomNumber { get; }
    public int MaximumPeople { get; }
    public decimal PricePerNight { get; }
    public int Rating { get; }
    public AvailableRoom(int roomId, string accommodationName, string location, string roomNumber, int maximumPeople, decimal pricePerNight, int rating)
    {
        RoomId = roomId;
        AccommodationName = accommodationName;
        Location = location;
        RoomNumber = roomNumber;
        MaximumPeople = maximumPeople;
        PricePerNight = pricePerNight;
        Rating = rating;
    }
}
