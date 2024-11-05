using System.Diagnostics.Metrics;
using VizitConsole.Models;
using VizitConsole.Repositories;

namespace VizitConsole.Services
{
    internal class InputOutputHandler
    {
        AppointmentRepository _appointmentRepo;
        AppointmentService _appointmentService;
        string connectionString { get; set; }
        private string Output;
        public InputOutputHandler(string connectionString)
        {
            this.connectionString = connectionString;
            _appointmentRepo = new AppointmentRepository(connectionString);
            _appointmentService = new AppointmentService(_appointmentRepo);
        }

        public async Task Start()
        {
            //получение user
            var userRopo = new UserRepository(connectionString);
            User user4 = await userRopo.GetUserById(4);
            //получение master
            var masterRepo = new MasterRepository(connectionString);
            Master master = await masterRepo.GetMasterById(2);

            //получение списка сервисов(services) мастера и конкретного сервиса(service)
            var serviceRepository = new ServiceRepository(connectionString);
            var services = new List<Service>(await serviceRepository.GetMasterServices(master));

            //запуск в цикле интерфейса с расписанием и выполнением команд из командной строки
            bool exit = false;
            while (exit == false)
            {
                // вывод master и его услуг 
                Console.WriteLine($"Master id: {master.Id}\tName: {master.Name}\tSpeciality: {master.Speciality}" +
                              $"\tdayInterval: {master.Day_interval}\n");
                foreach (var s in services)
                {
                    Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
                }

                // вывод расписания для user
                var appDetailRepo = new AppointmentDetailsRepository(connectionString);
                var scheduleService = new ScheduleService(_appointmentService, appDetailRepo);
                scheduleService.ShowScheduleForUser(await scheduleService.CreateScheduleForUser(master));

                //Вывод расписания для Master
                scheduleService.ShowScheduleDatail(await scheduleService.CreateScheduleForMaster(master));

                //вывод результата команды из командной строки 
                Console.WriteLine($"\nRezalt: {Output}");

                //ввод команд
                Console.Write($"\nTo make an appointment {user4.Name} with {master.Name} Enter command: (add hh:mm serviceId)\nto escape enter: (ex) ");
                string input = Console.ReadLine();
                exit = (input == "ex");
                if (exit == false)
                {
                    // обработка ввода и выполнение команды (add 09:30 4)
                    string[] commandParts = input.Split(' ');
                    switch (commandParts[0].ToLower())
                    {
                        case "add":
                            await AddCommand(commandParts, master, services, user4);
                            break;
                        case "can":
                            break;
                        default:
                            Output = "Unknown Command";
                            break;
                    }

                }
                Console.Clear();
            }
        }
        async Task AddCommand(string[] command, Master master, List<Service> masterServices, User user)
        {
            try
            {
                string[] time = command[1].Split(':');
                int timeH = int.Parse(time[0]);
                int timeM = int.Parse(time[1]);
                int serviceId = int.Parse(command[2]);

                var date = new DateTime(2024, 10, 14, timeH, timeM, 0);
                Service selectedService = masterServices.FirstOrDefault(s => s.Id == serviceId);

                if (selectedService != null)
                {
                    Appointment appointment2 = new Appointment(date, master, selectedService, user);
                    await _appointmentService.MakeAppointment(appointment2);
                    Output = $"Done {command}";
                }
            }
            catch (Exception ex)
            {
                Output = "Incorrect Command" + ex.Message;
            }
        }
    }
}
