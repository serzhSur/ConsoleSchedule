using VizitConsole.Services;

try
{
    var appconfig = new ConfigurationService();

    var dataBase = new DatabaseSetup(appconfig);
    await dataBase.InitializeDatabase(new DateTime(2024, 10, 14, 10, 0, 0));

    var handler = new InputOutputHandler(appconfig);
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








