using ConsoleSchedule.Services;

string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=schedule_test";
try 
{ 
    //создание базы данных, таблиц, тестовых записей
    var dataBase = new DatabasePostgreService(connectionString);
    await dataBase.CreateDataBase("Host=localhost;Username=postgres;Password=Sur999", "schedule_test");
    await dataBase.CreateTables();
    await dataBase.CreateTestRecords(new DateTime(2024, 10, 14, 10, 0, 0));
    
    var handler = new InputOutputHandler(connectionString);
    bool exit;
    do
    {
        await handler.Start();
        Console.WriteLine("\nДля выхода введите: 2");
        string input = Console.ReadLine();
        exit = (input == "2");
        Console.Clear();
    }
    while (!exit);
}

catch (Exception ex) 
{ 
    Console.WriteLine("APPLICATION ERROR: \n"+ex.Message+ ex.StackTrace);
}
finally 
{
    Console.WriteLine("finish.");
    Console.ReadKey();
}







