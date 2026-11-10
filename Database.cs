using MySqlConnector;
public class Database
{
   private const string ConnectionString =
        "Server=localhost;Database=holiday_maker;User ID=root;Password=12321;UseAffectedRows=false;";

    public async Task<MySqlConnection> OpenConnectionAsync()
    {
        var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}
