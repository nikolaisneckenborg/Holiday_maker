using MySqlConnector;
public class Database
{
    private const string ServerName = "localhost";
    private const string DatabaseName = "holiday_maker";
    private const string UserId = "root";
    private const string Password = "12321";

    private const string ConnectionString =
        $"Server={ServerName};Database={DatabaseName};User ID={UserId};Password={Password};UseAffectedRows=false;";

    public async Task<MySqlConnection> OpenConnectionAsync()
    {
        var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}
