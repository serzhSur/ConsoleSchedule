using Dapper;
using DbCreation;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using ConsoleSchedule.models;

Console.WriteLine("Start program...");

//var db = new DabasePostgreSQL();
//await db.CreateDataBase();
//await db.CreateTables();

var user1 = new User
{
    Name = "test-Оля1",
    PhoneNamber = "+79191801313"
};
var user2 = new User
{
    Name = "test-Саша2",
    PhoneNamber = "+79191801414"
};
//await user1.AddUserAsync(user1);
//await user2.AddUserAsync(user2);


var repository = new MasterRepository("Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata");
/*
{
    Name = "test-Master1",
    DayInterval = new TimeSpan(0, 30, 0)
};
*/
//await Master.AddMasterAsync(Master);

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
var appointment3 = new Appointment(new DateTime(2024, 9, 23, 13, 0, 0), master, service1, user1);
appointment3.CreateAppointment(appointment3, appointments, busyTime);


// вывод для user
DateTime startTime = new DateTime(2024, 9, 23, 9, 0, 0);
DateTime finishTime = new DateTime(2024, 9, 23, 19, 0, 0);
TimeSpan interval = master.DayInterval;

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


Console.WriteLine("finish.");
Console.ReadKey();


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
