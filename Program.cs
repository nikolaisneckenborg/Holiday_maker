using MySqlConnector;

var database = new Database();
await using var connection = await database.OpenConnectionAsync();

var ui = new Console_ui(
    new CustomerRepository(connection),
    new RoomRepository(connection),
    new BookingRepository(connection));


await ui.RunAsync();
