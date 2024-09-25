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

    var repository = new MasterRepository(connectionString);
    Master master = await repository.GetMasterById(1);

    var service1 = new Service
    {
        Name = "test-Массаж верх1",
        Duration = new TimeSpan(1, 0, 0),
        Master_id = 1
    };
    var service2 = new Service
    {
        Name = "test-Массаж Полностью2",
        Duration = new TimeSpan(2, 0, 0),
        Master_id = 1
    };
    //await service1.AddServiceAsync(service1);
    //await service2.AddServiceAsync(service2);


    var appointment1 = new Appointment(new DateTime(2024, 9, 23, 9, 0, 0), master, service1, user1);

    List<Appointment> appointments = new List<Appointment>() { appointment1 };

    List<DateTime> busyTime = new List<DateTime>();
    busyTime = appointment1.BusyTime;

    //добавление 2 записи
    var appointment2 = new Appointment(new DateTime(2024, 9, 23, 16, 0, 0), master, service2, user2);
    appointment2.CreateAppointment(appointment2, appointments, busyTime);

    //добавление 3й записи
    var appointment3 = new Appointment(new DateTime(2024, 9, 23, 13, 0, 0), master, service1, user17);
    appointment3.CreateAppointment(appointment3, appointments, busyTime);


    // вывод для user
    DateTime startTime = new DateTime(2024, 9, 23, 9, 0, 0);
    DateTime finishTime = new DateTime(2024, 9, 23, 19, 0, 0);
    TimeSpan interval = master.Day_interval;

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
        Console.WriteLine($"{i.DateTime}\t{i.MasterId}\t{i.ServiceId}\t{i.Duration}\t{i.UserId}\t{i.Cancellation}");
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
