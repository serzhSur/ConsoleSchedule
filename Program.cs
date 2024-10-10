
using ConsoleSchedule.Models;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;
using ConsoleSchedule;


string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=master_schedule";
Console.WriteLine("Start program...");
try 
{
    var dataBase = new DatabasePostgreSQL(connectionString);
    await dataBase.CreateDataBase("master_schedule");
    await dataBase.CreateTables();
    await dataBase.CreateTestRecords();
    /*
    var userRepository = new UserRepository(connectionString);
    User user1 = await userRepository.GetUserById(1);
    User user2 = await userRepository.GetUserById(2);
    User user17 = await userRepository.GetUserById(17);
    User user18 = await userRepository.GetUserById(18);
    //User userW = new User() {Name="Wolverine", PhoneNumber="900900900" };
    //await userRepository.AddUser(userW);
    var userW=await userRepository.GetUserById(20);

    var masterRepository = new MasterRepository(connectionString);
    Master master = await masterRepository.GetMasterById(1);

    var serviceRepository = new ServiceRepository(connectionString);
    List<Service> services = await serviceRepository.GetMasterServices(master);
    var service30 = services.FirstOrDefault<Service>(s => s.Duration == new TimeSpan(0, 30, 0));
    var service60 = services.FirstOrDefault(s=> s.Duration==new TimeSpan(1, 00, 0));
    if (service30 == null || service60 == null) 
    {
        throw new KeyNotFoundException("Service 30/60 Not Found");
    }

    var appointmentRepository = new AppointmentRepository(connectionString);
    var appointmentService = new AppointmentService(appointmentRepository);

    var appointment6 = new Appointment(new DateTime(2024, 9, 23, 11, 0, 0), master, service30, user1);
    //await appointmentService.MakeAppointment(appointment6);
    //await appointmentService.CancelAppointmentById(22);


    // вывод услуг
    Console.WriteLine("Services: ");
    int numb = 0;
    foreach (var s in services)
    {
        Console.WriteLine($"namber: {numb}\tservice: {s.Name}\tduration: {s.Duration}\tprice: 00");
        numb++;
    }


    // вывод для пользователей-клиентов
    Console.WriteLine("View for User");

    var startDayTime = new TimeSpan(9,0, 0);
    var endDayTime = new TimeSpan(19, 00, 0);
    var intervalDay = new TimeSpan(0, 30, 0);
    List<(TimeSpan start, TimeSpan end, string status)>schedule = new List<(TimeSpan, TimeSpan, string)>();
    for (var i=startDayTime; i < endDayTime; i += intervalDay) 
    {
        schedule.Add((i, i + intervalDay, "free"));
    }

    var occupitedTime = await appointmentService.GetBusyTime();
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
    var appointmentDetails = new List<AppointmentDetails>(appointmentDetailsRepository.GetAll());
    
    var filteredAppointments = appointmentDetails.Where(a=> a.Cancellation==false)
        .OrderBy(a=> a.Date)
        .ToList();
    Console.WriteLine($"Appointments to Master: {filteredAppointments.Count}");
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
    */
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







