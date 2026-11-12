using MySqlConnector;
public class RoomRepository(MySqlConnection connection)
{
    public async Task<List<AvailableRoom>> GetAvailableRoomsAsync(string location, DateOnly checkIn, DateOnly checkOut, int numPeople, bool? pool, bool? restaurant, int? maxBeach, int? maxCenter, string sortBy)
    {
        var result = new List<AvailableRoom>();

        await using var command = new MySqlCommand(
            """
            SELECT r.id, a.name, a.rating, a.location, r.room_number, r.maximum_people, r.price_per_night
            FROM rooms r
            JOIN accommodations a ON a.id = r.accommodation_id
            WHERE a.location = @location
            AND r.maximum_people >= @people
            AND NOT EXISTS (
            SELECT 1 FROM bookings b
            WHERE b.room_id = r.id
            AND b.check_in_date < @checkOut
            AND b.check_out_date > @checkIn)
            AND (@pool IS NULL OR a.pool = @pool)
            AND (@restaurant IS NULL OR a.restaurant = @restaurant)
            AND (@maxBeach IS NULL OR a.distance_to_beach <= @maxBeach)
            AND (@maxCenter IS NULL OR a.distance_to_city_center <= @maxCenter)
            ORDER BY
            CASE WHEN @sortBy = 'rating' THEN a.rating END DESC,
            r.price_per_night ASC;
            """, connection);
        
        command.Parameters.AddWithValue("@location", location);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@people", numPeople);
        command.Parameters.AddWithValue("@pool", pool);
        command.Parameters.AddWithValue("@restaurant", restaurant);
        command.Parameters.AddWithValue("@maxBeach", maxBeach);
        command.Parameters.AddWithValue("@maxCenter", maxCenter);
        command.Parameters.AddWithValue("@sortBy", sortBy);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new AvailableRoom(
                reader.GetInt32("id"),
                reader.GetString("name"),
                reader.GetString("location"),
                reader.GetString("room_number"),
                reader.GetInt32("maximum_people"),
                reader.GetDecimal("price_per_night"),
                reader.GetInt32("rating")));
        }
        
        return result;
    }
}