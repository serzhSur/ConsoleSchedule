
using VizitConsole.Models;
using VizitConsole.Repositories;

namespace VizitConsole.Services
{
    internal class InputOutputHandler
    {
        private UserRepository _userRepository;
        private MasterRepository _masterRepository;
        private ServiceRepository _serviceRepository;
        private AppointmentService _appointmentService;
        private ScheduleService _scheduleService;
        private string? Output;
       
        public InputOutputHandler(string connectionString)
        {
            _userRepository = new UserRepository(connectionString);
            _masterRepository = new MasterRepository(connectionString);
            _serviceRepository = new ServiceRepository(connectionString);
            _appointmentService = new AppointmentService(connectionString);
            _scheduleService = new ScheduleService(connectionString);
        }

        public async Task Start(int userId, int masterId)
        {
            User user4 = await _userRepository.GetUserById(userId);

            Master master = await _masterRepository.GetMasterById(masterId);
            var services = new List<Service>(await _serviceRepository.GetMasterServices(master));
            List<Master> masters = new List<Master>(await _masterRepository.GetAllMasters());

            //запуск в цикле интерфейса с расписанием и выполнением команд из командной строки
            bool exit = false;
            while (exit == false)
            {
                // вывод master и его услуг 
                foreach (var m in masters)
                {
                    Console.WriteLine($"Master id: {m.Id}\tName: {m.Name}\tSpeciality: {m.Speciality}" +
                             $"\tdayInterval: {m.Day_interval}");
                }
                Console.WriteLine($"\nMaster {master.Name} Has {services.Count} services: ");
                foreach (var s in services)
                {
                    Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
                }

                // вывод расписания для user;
                _scheduleService.ShowScheduleForUser(await _scheduleService.CreateScheduleForUser(master));

                //Вывод расписания для Master
                _scheduleService.ShowScheduleDatail(await _scheduleService.CreateScheduleForMaster(master));

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
                            Output = "cancel appointment";
                            break;
                        case "master":
                            Output = "youre master is:";
                            break;
                        default:
                            Output = "Unknown Command";
                            break;
                    }
                }
                Console.Clear();
            }
        }
        private async Task AddCommand(string[] command, Master master, List<Service> masterServices, User user)
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
                    Output = _appointmentService.Message;
                }
            }
            catch (Exception ex)
            {
                Output = "Incorrect Command" + ex.Message;
            }
        }
    }
}
