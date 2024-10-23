using ConsoleSchedule.Models;
using ConsoleSchedule.Repositories;
using ConsoleSchedule.Services;
using ConsoleSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSchedule.Services
{
    internal class InputOutputHandler
    {
       string connectionString {  get; set; }
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
            // вывод услуг master
            Console.WriteLine($"Master id: {master.Id}\tName: {master.Name}\tSpeciality: {master.Speciality}" +
                          $"\tdayInterval: {master.Day_interval}");
            foreach (var s in services)
            {
                Console.WriteLine($"service id: {s.Id}\tname: {s.Name}\tduration: {s.Duration}\tprice: 00");
            }

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

            // вывод расписания для user
            var appDetailRepo = new AppointmentDetailsRepository(connectionString);
            var scheduleService = new ScheduleService(appointmentService, appDetailRepo);
            scheduleService.ShowScheduleForUser(await scheduleService.CreateScheduleForUser(master));

            //Вывод расписания для Master
            scheduleService.ShowScheduleDatail(await scheduleService.CreateScheduleForMaster(master));
        }
        
    }
}
