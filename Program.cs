
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
    await dataBase.CreateDataBase("Host=localhost;Username=postgres;Password=Sur999", "master_schedule");
    await dataBase.CreateTables();
    await dataBase.CreateTestRecords();

    var masterRepository = new MasterRepository(connectionString);
    Master master = await masterRepository.GetMasterById(1);

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
    
    // вывод расписания для user
    var scheduleService = new ScheduleService(appointmentService);
    var schedule= await scheduleService.CreateScheduleForUser(master);
    scheduleService.ShowScheduleForUser(schedule);



    //Вывод расписания для Master
    var appointmentDetailRepository = new AppointmentDetailsRepository(connectionString);
    scheduleService.ShowScheduleDatail(await scheduleService.CreateScheduleForMaster(appointmentDetailRepository, master));
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







