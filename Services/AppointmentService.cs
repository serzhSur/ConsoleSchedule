using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleSchedule.Models;
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

        public async Task<List<(TimeSpan start, TimeSpan end, string status)>> GetBusyTime(int masterId)
        {
            List<Appointment> appointments = await _repository.GetAllAppointments(masterId);

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

        public async Task MakeAppointment(Appointment appointment)
        {
            if (appointment == null || appointment.User_id <= 0)
            {
                throw new ArgumentException("Error AppointmentService, MakeAppointment: appointment must be appointment.User_id> 0 && appointment != null");
            }
            else
            {
                int masterId = appointment.Master_id;
                var busyTime = new List<(TimeSpan start, TimeSpan end, string status)>(await GetBusyTime(masterId));

                var newTimeStart = appointment.Date.TimeOfDay;
                var newTimeEnd = appointment.Date.TimeOfDay + appointment.Duration;

                bool timeOccupited = busyTime.Any(busy => newTimeStart < busy.end && newTimeEnd > busy.start);
                
                if (timeOccupited)
                {
                    Console.WriteLine($"Appointment {appointment.Date} can't be made: Time interval is busy ");
                }
                else 
                {
                    await _repository.InsertAppointment(appointment);
                    Console.WriteLine($"Appointment Date: {appointment.Date} is Created");
                }

            }

        }
        public async Task CancelAppointmentById(int id)
        {
            Appointment appointment = await _repository.GetAppointmentById(id);
            appointment.Cancellation = true;
            appointment.Duration = new TimeSpan(0, 0, 0);

            await _repository.UpdateAppointment(appointment);
        }


    }

}
