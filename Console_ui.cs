
public class Console_ui(CustomerRepository customers, RoomRepository rooms, BookingRepository bookings)
{
    private Customer? _currentCustomer;

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(_currentCustomer is null
                ? "Ingen kund vald"
                : $"Vald kund: {_currentCustomer.FirstName} {_currentCustomer.LastName}");
            Console.WriteLine("1. Registrera kund");
            Console.WriteLine("2. Välj kund");
            Console.WriteLine("3. Sök och boka rum");
            Console.WriteLine("4. Visa bokningar");
            Console.WriteLine("5. Ändra antal personer");
            Console.WriteLine("6. Avboka");
            Console.WriteLine("0. Avsluta");
            Console.Write("Val: ");

            switch (Console.ReadLine())
            {
                case "2": await SelectCustomerAsync(); break;
                case "0": return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        } 
    }

    private async Task CreateCustomerAsync()
    {
        Console.Write("Ange förnamn: ");
        string? input = Console.ReadLine();

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
        _currentCustomer = selected;
    }
    private static string ReadRequired(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(input))
            return input;
        Console.WriteLine("Fältet får inte vara tomt.");
    }
}
    
}

