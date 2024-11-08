using VizitConsole.Services;

try
{
    var appconfig = new ConfigurationService();
    string connectionString = appconfig.GetConnectionString("dbConString");

    var dataBase = new DatabaseSetup(connectionString);
    await dataBase.InitializeDatabase(new DateTime(2024, 10, 14, 10, 0, 0));

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








