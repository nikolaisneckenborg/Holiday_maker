
public class Console_ui(CustomerRepository customers, RoomRepository rooms, BookingRepository bookings)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Holiday Maker Test");

        // Test för att få ut data från databasen
        var allCustomers = await customers.GetAllAsync();
        foreach (var c in allCustomers)
            Console.WriteLine($"{c.Id}. {c.FirstName} {c.LastName}");
    }
    
}

