using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleSchedule.models;
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

        public async Task MakeAppointment(Appointment appointment)
        {
            List<Appointment> appointments = await _repository.GetAllAppointments();
            foreach (var a in appointments)
            {
                a.EndTime = a.Date + a.Duration;
            }


            List<(TimeSpan start, TimeSpan end)> busyTime = new List<(TimeSpan, TimeSpan)>();
            foreach (var a in appointments)
            {
                busyTime.Add((a.Date.TimeOfDay, a.EndTime.TimeOfDay));
            }

            var newTimeStart = appointment.Date.TimeOfDay;
            var newTimeEnd = appointment.EndTime.TimeOfDay;

            bool timeOccupied = false;
            foreach (var t in busyTime)
            {
                if (newTimeStart > t.start && newTimeStart >= t.end || newTimeStart < t.start && newTimeEnd <= t.start)
                {
                    //timeOccupied = false;
                }
                else
                {
                    timeOccupied = true;//time is buisy
                    break;
                }
            }

            if (timeOccupied)
            {
                Console.WriteLine($"Appointment {appointment.Date} can't be made: Time interval is busy ");
            }
            else
            {
                await _repository.InsertAppointment(appointment);
                Console.WriteLine($"Appointment Date: {appointment.Date} is Created");
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
