using MySqlConnector;
public class BookingRepository(MySqlConnection connection)
{
    public async Task<int> CreateAsync(int customerId, int roomId, DateOnly checkIn, DateOnly checkOut, int numPeople)
    {
        await using var command = new MySqlCommand(
            """
            INSERT INTO bookings (customer_id, room_id, check_in_date, check_out_date, number_of_people)
            VALUES (@customerId, @roomId, @checkInDate, @checkOutDate, @numPeople);
            """, connection);
        
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkInDate", checkIn);
        command.Parameters.AddWithValue("@checkOutDate", checkOut);
        command.Parameters.AddWithValue("@numPeople", numPeople);

        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }   
}