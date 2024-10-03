using Dapper;
using DbCreation;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using ConsoleSchedule.models;
using ConsoleSchedule;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;


Console.WriteLine("Start program...");
string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
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




    List<Appointment> appointments = await appointmentRepository.GetAllAppointments();



    Console.WriteLine($"Master: {master.Name}");
    Console.WriteLine("Services: ");
    int numb = 0;   
    foreach (var s in services) 
    {
        Console.WriteLine($"namber: {numb}\tservice: {s.Name}\tduration: {s.Duration}\tprice: 00");
        numb++;
    }


    //Вывод для Master
    Console.WriteLine("{a.Id}\t{a.Date}\t{a.Master_id}\t{a.Service_id}\t{a.Duration}\t{a.User_id}\t{a.Cancellation}");
    foreach (var a in appointments)
    {
        Console.WriteLine($"{a.Id}\t{a.Date}\t{a.Master_id}\t{a.Service_id}\t{a.Duration}\t{a.User_id}\t{a.Cancellation}");
    }
    // вывод для user
    var startDayTime = new TimeSpan(9,0, 0);
    var endDayTime = new TimeSpan(19, 00, 0);
    var intervalDay = new TimeSpan(0, 30, 0);
    List<(TimeSpan start, TimeSpan end, string status)>schedule = new List<(TimeSpan, TimeSpan, string)>();
    for (var i=startDayTime; i < endDayTime; i += intervalDay) 
    {
        schedule.Add((i, i + intervalDay, "free"));
    }
    /*
    List<(TimeSpan start, TimeSpan end, string status)> occupitedTime = new List<(TimeSpan, TimeSpan,string)> 
    { 
        (new TimeSpan(9,0,0), new TimeSpan(10,0,0), "reserved"),
        (new TimeSpan(12,0,0), new TimeSpan(12,30,0), "reserved"),
        (new TimeSpan(10,15,0), new TimeSpan(10,45,0), "reserved")
    };
    */
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







