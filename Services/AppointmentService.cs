using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleSchedule.Repositories;

namespace ConsoleSchedule.Services
{
    internal class AppointmentService
    {
        private AppointmentRepository _repository;
        
        public AppointmentService(AppointmentRepository appointmentRepository)
        {
            _repository = appointmentRepository;
        }

        public async Task <List<(TimeSpan start, TimeSpan end, string status)>> GetBusyTime()
        {
            List<Appointment> appointments = await _repository.GetAllAppointments();
            List<(TimeSpan start, TimeSpan end, string status)> busyTime =
                appointments.Select(a =>
                {
                    TimeSpan start = a.Date.TimeOfDay;
                    TimeSpan end = start + a.Duration;
                    string status = "Busy";
                    return (start, end, status);
                }).ToList();
            return busyTime;
        }



    }

}
