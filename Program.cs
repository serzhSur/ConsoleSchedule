using Dapper;
using DbCreation;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using ConsoleSchedule.models;
using ConsoleSchedule;


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


    // вывод для user
    DateTime startTime = new DateTime(2024, 9, 23, 9, 0, 0);
    DateTime finishTime = new DateTime(2024, 9, 23, 19, 0, 0);
    TimeSpan interval = master.Day_interval;

    List<Appointment> appointments = await appointmentRepository.GetAllAppointments();

    List<DateTime> busyTime = new List<DateTime>();
    TimeSpan dayInterval = master.Day_interval;
    foreach (var a in appointments)
    {
        for (var i = a.Date; i < a.Date + a.Duration; i += dayInterval)
        {
            busyTime.Add(i);
        }
    }

    Console.WriteLine($"Master: {master.Name}");
    Console.WriteLine("Services: ");
    int numb = 0;   
    foreach (var s in services) 
    {
        Console.WriteLine($"namber: {numb}\tservice: {s.Name}\tduration: {s.Duration}\tprice: 00");
        numb++;
    }

    for (DateTime i = startTime; i < finishTime; i += interval)
    {
        bool status = false;
        foreach (var t in busyTime) //listBusyTime)
        {
            if (i == t)
            {
                status = true;
            }
        }
        Console.WriteLine($"{i}\t{status}");
    }

    //Вывод для Master
    Console.WriteLine("{a.Id}\t{a.Date}\t{a.Master_id}\t{a.Service_id}\t{a.Duration}\t{a.User_id}\t{a.Cancellation}");
    foreach (var a in appointments)
    {
        Console.WriteLine($"{a.Id}\t{a.Date}\t{a.Master_id}\t{a.Service_id}\t{a.Duration}\t{a.User_id}\t{a.Cancellation}");
    }
    // вывод2 для user
    var startDayTime = new TimeSpan(9, 0, 0);
    var endDayTime = new TimeSpan(19, 0, 0);
    var intervalDay = new TimeSpan(0, 30, 0);
    List<(TimeSpan start, TimeSpan end)>schedule = new List<(TimeSpan, TimeSpan)>();
    for (var i=startDayTime; i < endDayTime; i += intervalDay) 
    {
        schedule.Add((i, i + interval));
        
    }

    List<(TimeSpan start, TimeSpan end, string status)> occupitedTime = new List<(TimeSpan, TimeSpan,string)> 
    { 
        (new TimeSpan(10,0,0), new TimeSpan(11,0,0), "reserved")
    };
    
    
    foreach (var t in schedule) 
    {
        if ((t.start < occupitedTime[0].start && t.end <= occupitedTime[0].start)||(t.start >= occupitedTime[0].end))// && t.start > occupitedTime[0].end))
        {
            occupitedTime.Add((t.start, t.end,"free"));
        }
    }
    var sortedDaySchedule = occupitedTime.OrderBy(item=>item.start).ToList();
    foreach (var i in sortedDaySchedule) 
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







