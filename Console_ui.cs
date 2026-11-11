
public class Console_ui(CustomerRepository customers, RoomRepository rooms, BookingRepository bookings)
{
    public async Task RunAsync()
    {
        Console.WriteLine("Holiday Maker Test");
    
    }

    private async Task SelectCustomerAsync()
    {
        // Test för att få ut data från databasen
        var allCustomers = await customers.GetAllAsync();
        foreach (var c in allCustomers)
            Console.WriteLine($"{c.Id}. {c.FirstName} {c.LastName}");
        
        Console.Write("Välj kund-id: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Du måste skriva en siffra.");
            return;
        }

        Customer? selected = allCustomers.FirstOrDefault(c => c.Id == id);
        if (selected is null)
        {
            Console.WriteLine("Ingen kund med det id:t.");
            return;
        }

        Console.WriteLine($"Vald kund: {selected.FirstName} {selected.LastName}");
    }
    
}

