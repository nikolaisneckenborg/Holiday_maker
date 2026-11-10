using MySqlConnector;

var database = new Database();
await using var connection = await database.OpenConnectionAsync();

// REPOSITORY (klass med databas-queries)

/*await using var command = new MySqlCommand(
            """
            UPDATE products
            SET name = @name, price = @price
            WHERE id = @id;
            """, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@id", id);
        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
*/

await using var command = new MySqlCommand(
            "SELECT id, first_name FROM customers WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", 5);
        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();


if (await reader.ReadAsync())
{
    Console.WriteLine($"{reader.GetInt32("id")}: {reader.GetString("first_name")}");
}
else
{
    Console.WriteLine("No customer found with that id.");
}
