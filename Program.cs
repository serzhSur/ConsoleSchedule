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
    var appointment4 = new Appointment(new DateTime(2024, 9, 23, 9, 0, 0), master, services[0], user17);
    await appointmentRepository.MakeAppointment(appointment4);


     /*
     busyTime = appointment1.BusyTime;

     //добавление 2 записи
     var appointment2 = new Appointment(new Date(2024, 9, 23, 16, 0, 0), master, services[1], user2);
     //appointmentRepository.InsertAppointment(appointment2);
     appointment2.CreateAppointment(appointment2, appointments, busyTime);

     //добавление 3й записи
     var appointment3 = new Appointment(new Date(2024, 9, 23, 13, 0, 0), master, services[2], user17);
     //appointmentRepository.InsertAppointment(appointment3);
     appointment3.CreateAppointment(appointment3, appointments, busyTime);
     */

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
    foreach (var i in appointments)
    {
        Console.WriteLine($"{i.Date}\t{i.Master_id}\t{i.Service_id}\t{i.Duration}\t{i.User_id}\t{i.Cancellation}");
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






void CanselAppointment(Appointment appointment, IEnumerable<DaySchedule> daySchedule)
{
    var canselIntervals = daySchedule.Where(i => i.Busy == true && i.AppointmentId == appointment.Id);
    if (canselIntervals != null)
    {
        foreach (var i in canselIntervals)
        {
            i.Busy = false;
            i.AppointmentId = null;
        }
    }
}
