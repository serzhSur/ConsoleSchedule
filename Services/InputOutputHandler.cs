using System.Globalization;
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
        private string connectionString;
        private string? Output;
        int _userId { get; set; } = 4;
        int _masterId { get; set; } = 1;
        public InputOutputHandler(ConfigurationService config)//string connectionString)
        {
            connectionString = config.GetConnectionString("myDbConnection");
            _userRepository = new UserRepository(connectionString);
            _masterRepository = new MasterRepository(connectionString);
            _serviceRepository = new ServiceRepository(connectionString);
            _appointmentService = new AppointmentService(connectionString);
            _scheduleService = new ScheduleService(connectionString);
        }
        public async Task Start()
        {
            User user = await _userRepository.GetUserById(_userId);
            Master master = await _masterRepository.GetMasterById(_masterId);
            List<Service> services = new List<Service>(await _serviceRepository.GetMasterServices(master));
            List<Master> masters = new List<Master>(await _masterRepository.GetAllMasters());

            bool exit = false;
            while (exit == false)
            {
                await DisplayData(master, masters, services);
                string input = InputCommands(master, user);
                exit = (input == "ex");
                if (exit == false)
                {
                    await ExecuteCommands(input, master, services, user);
                }
                Console.Clear();
            }
        }
        private async Task DisplayData(Master master, List<Master> masters, List<Service> services)
        {
            foreach (var m in masters)
            {
                Console.WriteLine($"Master id: {m.Id}\tName: {m.Name}\tSpeciality: {m.Speciality}" +
                         $"\tdayInterval: {m.Day_interval}");
            }

            Console.WriteLine($"\nMaster {master.Name} Has {services.Count} Services: ");

            foreach (var s in services)
            {
                Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
            }

            _scheduleService.ShowScheduleForUser(await _scheduleService.CreateScheduleForUser(master));

            _scheduleService.ShowScheduleDatail(await _scheduleService.CreateScheduleForMaster(master));

            Console.WriteLine($"\nResult: {Output}");
        }
        private string InputCommands(Master master, User user)
        {
            Console.WriteLine($"\nCommands for appointment User: {user.Name} to master: {master.Name}");
            Console.WriteLine("1 - to make appointment, enter time and service id. Example: add 08:15 4");
            Console.WriteLine("2 - to cancell appointment, enter time. Example: can 09:30");
            Console.WriteLine("3 - to exit the application, enter:  ex");
            Console.Write("Enter command: ");
            return Console.ReadLine();
        }
        private async Task<Master> ExecuteCommands(string input, Master master, List<Service> services, User user)
        {
            string[] commandParts = input.Split(' ');

            switch (commandParts[0].ToLower())
            {
                case "add":
                    await AddAppointment(commandParts, master, services, user);
                    break;
                case "can":
                    await CancelAppointment(commandParts, master, user);
                    break;
                default:
                    Output = "Unknown Command";
                    break;
            }
            return master;
        }
        private async Task AddAppointment(string[] command, Master master, List<Service> masterServices, User user)
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
                    Appointment appointment = new Appointment(date, master, selectedService, user);
                    await _appointmentService.MakeAppointment(appointment);
                    Output = _appointmentService.Message;
                }
            }
            catch (Exception ex)
            {
                Output = "Incorrect Command" + ex.Message;
            }
        }
        private async Task CancelAppointment(string[] command, Master master, User user)
        {
            try
            {
                string[] time = command[1].Split(":");
                int timeH = int.Parse(time[0]);
                int timeM = int.Parse(time[1]);
                DateTime date = new DateTime(2024, 10, 14, timeH, timeM, 0);
                await _appointmentService.CancelAppointment(date, master, user);
                Output = $"Appointment on {date.ToString("hh:mm")} Canceled";
            }
            catch (Exception ex)
            {
                Output = "Incorrect Command " + ex.Message;
            }
        }
    }
}
