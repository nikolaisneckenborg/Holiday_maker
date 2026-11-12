using MySqlConnector;
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
                case "1": await CreateCustomerAsync(); break;
                case "2": await SelectCustomerAsync(); break;
                case "3": await SearchAvailableRoomAsync(); break;
                case "0": return;
                default: Console.WriteLine("Ogiltigt val."); break;
            }
        } 
    }

    private async Task CreateCustomerAsync()
    {
        string firstName = ReadRequired("Förnamn: ");
        string lastName = ReadRequired("Efternamn: ");
        string mail = ReadRequired("E-post: ");
        string phone = ReadRequired("Telefon: ");

        try
        {
            int id = await customers.CreateAsync(firstName, lastName, mail, phone);
            Console.WriteLine($"Kund registrerad med id {id}.");
            _currentCustomer = new Customer(id, firstName, lastName, mail, phone); 
        }
        catch(MySqlException ex)
        {
            Console.WriteLine($"Kunden kunde inte sparas: {ex.Message}");
        }

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
    private async Task SearchAvailableRoomAsync()
    {
        string location = ReadRequired("Välj ort: ");
        DateOnly checkinDate = ReadDate("Välj check-in datum: ");
        DateOnly checkoutDate = ReadDate("Välj check-ut datum: ");
        if(checkoutDate <= checkinDate)
        {
            Console.WriteLine("Din check-out måste vara efter check-in");
            return;
        }
        int people = ReadInt("Antal personer: ");
        var availableRooms = await rooms.GetAvailableRoomsAsync(location, checkinDate, checkoutDate, people);
        if(availableRooms.Count == 0)
        {
            Console.WriteLine("Inga lediga rum hittades");
        }
        else
        {
            foreach(var r in availableRooms)
            {
                Console.WriteLine($"{r.RoomId}. {r.AccommodationName}, {r.Location} - rum {r.RoomNumber}, max {r.MaximumPeople} personer, {r.PricePerNight} kr/natt");
            }
        }
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
    private static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if(DateOnly.TryParse(input, out DateOnly date))
            {
                return date;
            }
            else
            {
                Console.WriteLine("Ogiltigt datum, skriv formatet: 2000-10-15");
            }
            
        }
    }
    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if(int.TryParse(input, out int people) && people > 0)
            {
                return people;
            }
            else
            {
                Console.WriteLine("Ange ett heltal större än 0");
            }
            
        }
    }
    
}

