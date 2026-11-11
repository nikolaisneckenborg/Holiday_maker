using MySqlConnector;
public class CustomerRepository(MySqlConnection connection)
{
    public async Task<List<Customer>> GetAllAsync()
    {
        var result = new List<Customer>();

        await using var command = new MySqlCommand(
            """
            SELECT id, first_name, last_name, mail, phone_number
            FROM customers
            ORDER BY id;
            """, connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Customer(
                reader.GetInt32("id"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetString("mail"),
                reader.GetString("phone_number")));
        }

        return result;
    }
}