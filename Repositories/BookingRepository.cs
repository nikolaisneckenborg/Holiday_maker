using MySqlConnector;
public class BookingRepository(MySqlConnection connection)
{
    public async Task<int?> CreateAsync(int customerId, int roomId, DateOnly checkIn, DateOnly checkOut, int numPeople)
    {
        await using var selectCommand = new MySqlCommand(
            """
            SELECT id FROM bookings
            WHERE room_id = @roomId
            AND check_in_date < @checkOutDate
            AND check_out_date > @checkInDate;
            """, connection);

        selectCommand.Parameters.AddWithValue("@roomId", roomId);
        selectCommand.Parameters.AddWithValue("@checkInDate", checkIn);
        selectCommand.Parameters.AddWithValue("@checkOutDate", checkOut);
        
        await using (var reader = await selectCommand.ExecuteReaderAsync())
        {
            if (reader.HasRows)
            {
                return null;
            }
        }

        await using var selectMaxPeople = new MySqlCommand(
            """
            SELECT maximum_people FROM rooms 
            WHERE id = @roomId
            """, connection);

        selectMaxPeople.Parameters.AddWithValue("@roomId", roomId);

        await using (var readerMaxPeople = await selectMaxPeople.ExecuteReaderAsync())
        {
            if (!await readerMaxPeople.ReadAsync())
            {
                return null; // rummet finns inte
            }

            int maxPeople = readerMaxPeople.GetInt32("maximum_people");

            if (numPeople > maxPeople)
            {
                return null; // för många för att få plats i rummet
            }
        }
        
        
        await using var insertCommand = new MySqlCommand(
            """
            INSERT INTO bookings (customer_id, room_id, check_in_date, check_out_date, number_of_people)
            VALUES (@customerId, @roomId, @checkInDate, @checkOutDate, @numPeople);
            """, connection);
        
        insertCommand.Parameters.AddWithValue("@customerId", customerId);
        insertCommand.Parameters.AddWithValue("@roomId", roomId);
        insertCommand.Parameters.AddWithValue("@checkInDate", checkIn);
        insertCommand.Parameters.AddWithValue("@checkOutDate", checkOut);
        insertCommand.Parameters.AddWithValue("@numPeople", numPeople);

        await insertCommand.ExecuteNonQueryAsync();
        return (int)insertCommand.LastInsertedId;
    }
    public async Task<List<Booking>> GetByCustomerAsync(int customerId)
    {
        await using var selectCommand = new MySqlCommand(
            // Använder View booking_details som jag skapade i Sql filen
            """
            SELECT id, accommodation_name, room_number, check_in_date,
                check_out_date, number_of_people, total_price
            FROM booking_details
            WHERE customer_id = @customerId
            ORDER BY check_in_date;
            """, connection);

        selectCommand.Parameters.AddWithValue("@customerId", customerId);

        List<Booking> bookings = [];
        await using (var reader = await selectCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                bookings.Add(new Booking(
                    reader.GetInt32("id"),
                    reader.GetString("accommodation_name"),
                    reader.GetString("room_number"),
                    reader.GetDateOnly("check_in_date"),
                    reader.GetDateOnly("check_out_date"),
                    reader.GetInt32("number_of_people"),
                    reader.GetDecimal("total_price")));
            }
        }
        return bookings;
    }
    public async Task<bool> DeleteAsync(int bookingId, int customerId)
    {
        await using var deleteCommand = new MySqlCommand(
            """
            DELETE FROM bookings 
            WHERE id = @bookingId AND customer_id = @customerId;
            """, connection);

        deleteCommand.Parameters.AddWithValue("@bookingId", bookingId);
        deleteCommand.Parameters.AddWithValue("@customerId", customerId);

        int affectedRows = await deleteCommand.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }
}