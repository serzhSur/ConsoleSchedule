using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleSchedule.models;
using System.Diagnostics.Metrics;

namespace ConsoleSchedule
{
    internal class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Master_id { get; set; }
        public int Service_id { get; set; }
        public TimeSpan Duration { get; set; }
        public int User_id { get; set; }
        public bool Cancellation { get; set; } = false;
        public List<DateTime> BusyTime { get; }//кастыль
        public TimeSpan DayInterval { get; set; }



        public Appointment() { }
        public Appointment(DateTime dateTime, Master master, Service service, User user)
        {
            Date = dateTime;
            Duration = service.Duration;
            Master_id = master.Id;
            Service_id = service.Id;
            User_id = user.Id;
            BusyTime = new List<DateTime>(GetBusyTime(master));
            DayInterval = master.Day_interval;

        }




        private List<DateTime> GetBusyTime(Master master)
        {
            List<DateTime> busyTime = new List<DateTime>();
            TimeSpan dayInterval = master.Day_interval;

            for (var i = Date; i < Date + Duration; i += dayInterval)
            {
                busyTime.Add(i);
            }
            return busyTime;
        }


        public void CreateAppointment(Appointment appointment, List<Appointment> appointments, List<DateTime> busyDayTime)
        {
            bool timeOccupied = false;
            foreach (var time in appointment.BusyTime)
            {
                if (busyDayTime.Contains(time))
                {
                    Console.WriteLine("interval is incorrect");
                    break;
                }
            }
            if (timeOccupied == false)
            {
                appointments.Add(appointment);
                busyDayTime.AddRange(appointment.BusyTime);
                //запись в базу данных
                Console.WriteLine("Appointment is Create");
            }
        }
        

    }

}
