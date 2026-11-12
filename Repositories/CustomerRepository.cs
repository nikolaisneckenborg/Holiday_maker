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
    public async Task<int> CreateAsync(string firstName, string lastName, string mail, string phoneNumber)
    {
        await using var command = new MySqlCommand(
            """
            INSERT INTO customers (first_name, last_name, mail, phone_number)
            VALUES (@firstName, @lastName, @mail, @phoneNumber);
            """, connection);
        
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@mail", mail);
        command.Parameters.AddWithValue("@phoneNumber", phoneNumber);

        await command.ExecuteNonQueryAsync();
        return (int)command.LastInsertedId;
    }
    
}