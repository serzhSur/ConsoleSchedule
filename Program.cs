
using ConsoleSchedule.Models;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;
using ConsoleSchedule;
using System.Diagnostics.Metrics;


string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=master_schedule";
Console.WriteLine("Start program...");
try 
{
    var dataBase = new DatabasePostgreSQL(connectionString);
    await dataBase.CreateDataBase("master_schedule");
    await dataBase.CreateTables();
    await dataBase.CreateTestRecords();

    var masterRepository = new MasterRepository(connectionString);
    Master master = await masterRepository.GetMasterById(2);

    var serviceRepository = new ServiceRepository(connectionString);
    var services = new List<Service>( await serviceRepository.GetMasterServices(master));

    var appointmentRepository = new AppointmentRepository(connectionString);
    var appointmentService = new AppointmentService(appointmentRepository);

    // вывод услуг
    Console.WriteLine($"Master id: {master.Id}\tName: {master.Name}\tSpeciality: {master.Speciality}" +
                      $"\tdayInterval: {master.Day_interval}");
    foreach (var s in services)
    {
        Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
    }
    
    // вывод для пользователей-клиентов
    Console.WriteLine("View for User");

    var startDayTime = new TimeSpan(9,0, 0);
    var endDayTime = new TimeSpan(19, 00, 0);
    var intervalDay = master.Day_interval;
    List<(TimeSpan start, TimeSpan end, string status)>schedule = new List<(TimeSpan, TimeSpan, string)>();
    for (var i=startDayTime; i < endDayTime; i += intervalDay) 
    {
        schedule.Add((i, i + intervalDay, "free"));
    }

    var occupitedTime = await appointmentService.GetBusyTime(master.Id);
    schedule = schedule.Where(s =>
            !occupitedTime.Any(o =>
                s.start < o.end && s.end > o.start)).ToList();

    schedule.AddRange(occupitedTime);
    schedule.Sort((x, y) => x.start.CompareTo(y.start));
    foreach (var i in schedule) 
    {
        Console.WriteLine($"{i.start}--{i.end}\t{i.status}");
    }



    //Вывод для Master
    var appointmentDetailsRepository = new AppointmentDetailsRepository(connectionString);
    var appointmentDetails = new List<AppointmentDetails>(appointmentDetailsRepository.GetAll(master));
    
    var filteredAppointments = appointmentDetails.Where(a=> a.Cancellation==false)
        .OrderBy(a=> a.Date)
        .ToList();
    Console.WriteLine($"Appointments to Master: {filteredAppointments.Count}");
    Console.WriteLine($"{"Id",-5}{"Date",-25}{"End Time",-10}{"Duration",-10}{"Service Name",-20}{"Master Name",-20}{"User Name",-20}{"User Phone",-15}{"Cancellation",-10}");
    Console.WriteLine(new string('-', 120)+"\n"); // Разделитель
    foreach (var a in filteredAppointments)
    {
        a.ShowConsole(a);
    }
    //вывод отмененных записей
    var cancelAppointments = appointmentDetails.Where(a=> a.Cancellation==true)
        .OrderBy(a=> a.Date)
        .ToList();
    Console.WriteLine($"Cancel Appointments: {cancelAppointments.Count}");
    foreach (var a in cancelAppointments)
    {
        a.ShowConsole(a);
    }
    
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







