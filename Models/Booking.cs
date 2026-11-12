public class Booking
{
    public int Id { get; }
    public string AccommodationName { get; }
    public string RoomNumber { get; }
    public DateOnly CheckInDate { get; }
    public DateOnly CheckOutDate { get; }
    public int NumberOfPeople { get; }
    public decimal TotalPrice { get; }

    public Booking(int id, string accommodationName, string roomNumber, DateOnly checkInDate, DateOnly checkOutDate, int numberOfPeople, decimal totalPrice)
    {
        Id = id;
        AccommodationName = accommodationName;
        RoomNumber = roomNumber;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        NumberOfPeople = numberOfPeople;
        TotalPrice = totalPrice;
    }
}