using ConsoleSchedule.Models;
using ConsoleSchedule.Repositories;

namespace ConsoleSchedule.Services
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
                // вывод услуг master
                Console.WriteLine($"Master id: {master.Id}\tName: {master.Name}\tSpeciality: {master.Speciality}" +
                              $"\tdayInterval: {master.Day_interval}");
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


               
                Console.WriteLine($"Rezalt: {Output}");
                Console.Write($"To make an appointment {user4.Name} with {master.Name} Enter command: (hh:mm serviceId)\nto escape enter: (ex) ");
                string input = Console.ReadLine();
                Output = input;//это тест, здесь вывод результата команды в консоль
                exit = (input == "ex");
                if (exit == false)
                {
                    //выполняем команду
                    //int timeH = Convert.ToInt16("9");
                    //int timeM = Convert.ToInt16("15");
                    //var date = new DateTime(2024, 10, 14, timeH, timeM, 0);
                    //int serviceId = 5;
                    //Appointment appointment2 = new Appointment(date, master, service, user4);
                }
                Console.Clear();
            }

        }

    }
}
