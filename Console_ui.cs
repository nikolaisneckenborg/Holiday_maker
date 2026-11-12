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
                case "4": await ShowBookingsAsync(); break;
                case "5": await ChangePeopleAsync(); break;
                case "6": await CancelBookingAsync(); break;
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
        if(_currentCustomer == null)
        {
            Console.WriteLine("Du måste välja en kund först.");
            return;
        }
        string location = ReadRequired("Välj ort: ");
        DateOnly checkinDate = ReadDate("Välj check-in datum: ");
        DateOnly checkoutDate = ReadDate("Välj check-ut datum: ");
        if(checkoutDate <= checkinDate)
        {
            Console.WriteLine("Din check-out måste vara efter check-in");
            return;
        }
        int people = ReadInt("Antal personer: ");
        bool? pool = ReadOptionalBool("Pool? (j/n, tomt = spelar ingen roll): ");
        bool? restaurant = ReadOptionalBool("Restaurang? (j/n, tomt = spelar ingen roll): ");
        int? maxBeach = ReadOptionalInt("Max avstånd till stranden i meter (tomt = spelar ingen roll): ");
        int? maxCenter = ReadOptionalInt("Max avstånd till centrum i meter (tomt = spelar ingen roll): ");
        Console.Write("Sortera på 1 = pris (lägst först), 2 = betyg (högst först): ");
        string? sortInput = Console.ReadLine()?.Trim();
        string sortBy = sortInput == "2" ? "rating" : "price";
        var availableRooms = await rooms.GetAvailableRoomsAsync(location, checkinDate, checkoutDate, people, pool, restaurant, maxBeach, maxCenter, sortBy);
        if(availableRooms.Count == 0)
        {
            Console.WriteLine("Inga lediga rum hittades");
            return;
        }
        else
        {
            foreach(var r in availableRooms)
            {
                Console.WriteLine($"{r.RoomId}. {r.AccommodationName}, {r.Location} - rum {r.RoomNumber}, max {r.MaximumPeople} personer, {r.PricePerNight} kr/natt, betyg {r.Rating}/5");
            }
        }
        int roomId = ReadInt("Ange rums-id du vill boka: ");

        AvailableRoom? chosenRoom = availableRooms.FirstOrDefault(r => r.RoomId == roomId);
        if (chosenRoom is null)
        {
            Console.WriteLine("Det rummet finns inte i sökresultatet.");
            return;
        }

        int? bookingId = await bookings.CreateAsync(_currentCustomer.Id, roomId, checkinDate, checkoutDate, people);
        if (bookingId is null)
        {
            Console.WriteLine("Bokningen kunde inte genomföras. Rummet är inte längre ledigt eller för litet.");
            return;
        }

        int nights = checkoutDate.DayNumber - checkinDate.DayNumber;
        decimal totalPrice = nights * chosenRoom.PricePerNight;

        Console.WriteLine($"Bokning klar! Bokningsnummer: {bookingId}");
        Console.WriteLine($"{chosenRoom.AccommodationName}, rum {chosenRoom.RoomNumber}");
        Console.WriteLine($"{checkinDate} – {checkoutDate}, {nights} nätter, {people} personer");
        Console.WriteLine($"Totalpris: {totalPrice} kr");

    }
    private async Task ShowBookingsAsync()
    {
        if(_currentCustomer == null)
        {
            Console.WriteLine("Ingen kund är vald");
            return;
        }
        List<Booking> bookinglist = await bookings.GetByCustomerAsync(_currentCustomer.Id);
        if(bookinglist.Count == 0)
        {
            Console.WriteLine("Inga bokningar registrerade.");
        }
        else
        {
            foreach (var b in bookinglist)
            {
                int nights = b.CheckOutDate.DayNumber - b.CheckInDate.DayNumber;
                Console.WriteLine($"Bokning {b.Id}: {b.AccommodationName}, rum {b.RoomNumber}");
                Console.WriteLine($"  {b.CheckInDate} – {b.CheckOutDate} ({nights} nätter), {b.NumberOfPeople} personer, totalt {b.TotalPrice} kr");
            }
        }

    }
    private async Task CancelBookingAsync()
    {
        if(_currentCustomer == null)
        {
            Console.WriteLine("Ingen kund är vald");
            return;
        }
        await ShowBookingsAsync();
        int bookingId = ReadInt("Ange bokningsnummer att avboka: ");
        bool cancel = await bookings.DeleteAsync(bookingId, _currentCustomer.Id);
        if (cancel)
        {
            Console.WriteLine($"Din bokning med id: {bookingId} har avbokats.");
        }
        else
        {
            Console.WriteLine("Det finns ingen bokning med det id:t.");
        }
    }
    private async Task ChangePeopleAsync()
    {
        if(_currentCustomer == null)
        {
            Console.WriteLine("Ingen kund är vald");
            return;
        }
        await ShowBookingsAsync();
        int bookingId = ReadInt("Ange bokningsnummer att ändra: ");
        int people = ReadInt("Ange antal personer: ");
        bool update = await bookings.UpdatePeopleAsync(bookingId, _currentCustomer.Id, people);
        if (update)
        {
            Console.WriteLine($"Din bokning med id: {bookingId} har ändrats till {people} personer.");
        }
        else
        {
            Console.WriteLine("Kunde inte ändra: bokningen finns inte för kunden eller rummet har inte plats för så många.");
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

    private static bool? ReadOptionalBool(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
                return null;
            if (input == "j")
                return true;
            if (input == "n")
                return false;

            Console.WriteLine("Svara j, n eller lämna tomt.");
        }
    }

    private static int? ReadOptionalInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                return null;
            if (int.TryParse(input, out int value) && value >= 0)
                return value;

            Console.WriteLine("Ange ett heltal (0 eller mer) eller lämna tomt.");
        }
    }
}

