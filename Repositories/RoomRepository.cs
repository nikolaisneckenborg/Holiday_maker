using MySqlConnector;
public class RoomRepository(MySqlConnection connection)
{
    public async Task<List<AvailableRoom>> GetAvailableRoomsAsync(string location, DateOnly checkIn, DateOnly checkOut, int numPeople)
    {
        var result = new List<AvailableRoom>();

        await using var command = new MySqlCommand(
            """
            SELECT r.id, a.name, a.location, r.room_number, r.maximum_people, r.price_per_night
            FROM rooms r
            JOIN accommodations a ON a.id = r.accommodation_id
            WHERE a.location = @location
            AND r.maximum_people >= @people
            AND NOT EXISTS (
            SELECT 1 FROM bookings b
            WHERE b.room_id = r.id
            AND b.check_in_date < @checkOut
            AND b.check_out_date > @checkIn);
            """, connection);
        
        command.Parameters.AddWithValue("@location", location);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@people", numPeople);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new AvailableRoom(
                reader.GetInt32("id"),
                reader.GetString("name"),
                reader.GetString("location"),
                reader.GetString("room_number"),
                reader.GetInt32("maximum_people"),
                reader.GetDecimal("price_per_night")));
        }
        
        return result;
    }
}