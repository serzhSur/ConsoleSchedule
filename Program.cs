using VizitConsole.Services;

string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=schedule_test";
try
{
    var dataBase = new DatabaseSetup(connectionString);
    await dataBase.InitializeDatabase("Host=localhost;Username=postgres;Password=Sur999", "schedule_test", new DateTime(2024, 10, 14, 10, 0, 0));

    var handler = new InputOutputHandler(connectionString);
    await handler.Start();
}
catch (Exception ex)
{
    Console.WriteLine("Application Error: \n" + ex.Message + ex.StackTrace);
}
finally 
{
    Console.WriteLine("Application Finish.");
}








