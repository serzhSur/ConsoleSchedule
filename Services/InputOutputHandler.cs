using VizitConsole.Models;
using VizitConsole.Repositories;

namespace VizitConsole.Services
{
    internal class InputOutputHandler
    {
        string connectionString { get; set; }
        private string Output;
        public InputOutputHandler(string connectionString)
        {
            this.connectionString = connectionString;
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
            Service service = services.FirstOrDefault(s => s.Name == "hair-coloring");

            //отмена записи к мастеру и создание другой записи
            var appointmentRepo = new AppointmentRepository(connectionString);
            var appointmentService = new AppointmentService(appointmentRepo);
            await appointmentService.CancelAppointmentById(6);
            if (service == null)
            {
                throw new ArgumentException("Appointment's parametrs NOT Found", nameof(service));
            }
            Appointment appointment = new Appointment(new DateTime(2024, 10, 14, 12, 30, 0), master, service, user4);
            await appointmentService.MakeAppointment(appointment);

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
                var scheduleService = new ScheduleService(appointmentService, appDetailRepo);
                scheduleService.ShowScheduleForUser(await scheduleService.CreateScheduleForUser(master));

                //Вывод расписания для Master
                scheduleService.ShowScheduleDatail(await scheduleService.CreateScheduleForMaster(master));


                Console.WriteLine($"\nRezalt: {Output}");
                Console.Write($"\nTo make an appointment {user4.Name} with {master.Name} Enter command: (hh+mm+serviceId)\nto escape enter: (ex) ");

                string input = Console.ReadLine();
                exit = (input == "ex");
                if (exit == false)
                {
                    // обработка ввода и выполнение команды
                    //18+0+5
                    try
                    {
                        string[] companents = input.Split('+');

                        int timeH = int.Parse(companents[0]);
                        int timeM = int.Parse(companents[1]);
                        int serviceId = int.Parse(companents[2]);

                        var date = new DateTime(2024, 10, 14, timeH, timeM, 0);
                        Service selectedService = services.FirstOrDefault(s => s.Id == serviceId);


                        if (selectedService != null)
                        {
                            Appointment appointment2 = new Appointment(date, master, selectedService, user4);
                            await appointmentService.MakeAppointment(appointment2);
                            Output = $"Done {input}";
                        }

                    }
                    catch (Exception ex)
                    {
                        Output = "Incorrect Command"+ex.Message;
                    }
                }
                Console.Clear();
            }

        }

    }
}
