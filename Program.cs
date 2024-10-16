
using ConsoleSchedule.Models;
using ConsoleSchedule.Services;
using ConsoleSchedule.Repositories;

string connectionString = "Host=localhost;Username=postgres;Password=Sur999; Database=master_schedule";
Console.WriteLine("Start program...");
try 
{ 
    var dataBase = new DatabasePostgreService(connectionString);
    await dataBase.CreateDataBase("Host=localhost;Username=postgres;Password=Sur999", "master_schedule");
    await dataBase.CreateTables();
    await dataBase.CreateTestRecords();

    var masterRepository = new MasterRepository(connectionString);
    Master master = await masterRepository.GetMasterById(2);

    var serviceRepository = new ServiceRepository(connectionString);
    var services = new List<Service>( await serviceRepository.GetMasterServices(master));
    // вывод услуг master
    Console.WriteLine($"Master id: {master.Id}\tName: {master.Name}\tSpeciality: {master.Speciality}" +
                      $"\tdayInterval: {master.Day_interval}");
    foreach (var s in services)
    {
        Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
    }

    var appointmentRepository = new AppointmentRepository(connectionString);
    var appointmentService = new AppointmentService(appointmentRepository);
    await appointmentService.CancelAppointmentById(6);
    // вывод расписания для user
    var appDetailRepo = new AppointmentDetailsRepository(connectionString);
    var scheduleService = new ScheduleService(appointmentService, appDetailRepo);
    scheduleService.ShowScheduleForUser(await scheduleService.CreateScheduleForUser(master));
    //Вывод расписания для Master
    scheduleService.ShowScheduleDatail(await scheduleService.CreateScheduleForMaster(master));
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







