
using ConsoleSchedule.models;
using ConsoleSchedule;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;


string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
Console.WriteLine("Start program...");
try 
{
    //var db = new DabasePostgreSQL();
    //await db.CreateDataBase();
    //await db.CreateTables();
    var userRepository = new UserRepository(connectionString);
    User user1 = await userRepository.GetUserById(1);
    User user2 = await userRepository.GetUserById(2);
    User user17 = await userRepository.GetUserById(17);

    var masterRepository = new MasterRepository(connectionString);
    Master master = await masterRepository.GetMasterById(1);

    var serviceRepository = new ServiceRepository(connectionString);
    var masterId = master.Id;
    List<Service> services = await serviceRepository.GetMasterServices(masterId);


    var appointmentRepository = new AppointmentRepository(connectionString);
    //var appointment4 = new Appointment(new DateTime(2024, 9, 23, 10, 15, 0), master, services[0], user17);
    //await appointmentRepository.MakeAppointment(appointment4);
    //await appointmentRepository.DeleteAppointmentByDate(appointment4);

    var appointment5 = new Appointment(new DateTime(2024, 9, 23, 10, 30, 0), master, services[0], user17);
    await appointmentRepository.MakeAppointment2(appointment5);

    var appointmentService = new AppointmentService(appointmentRepository);


    // вывод услуг
    Console.WriteLine($"Master: {master.Name}");
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
    Console.WriteLine("View for Master");

    var db = new AppointmentDetailsRepository(connectionString);
    var appointmentDetails = new List<AppointmentDetails>(db.GetAll());
    foreach (var a in appointmentDetails)
    {
        Console.WriteLine($"{a.Id}\t{a.Date}\t{a.ServiceDuration}" +
            $"\t{a.Date.Add( a.Duration).ToString("HH:mm")}" +
            $"\t{a.ServiceName}\t{a.MasterName}\t{a.UserName}\t{a.UserPhone}\t{a.Cancellation}");
    }

}
catch (Exception ex) 
{ 
    Console.WriteLine("APPLICATION ERROR: "+ ex.Message);
}
finally 
{
    Console.WriteLine("finish.");
    Console.ReadKey();
}







