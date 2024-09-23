using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation
{
    internal class Appointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int MasterId { get; set; }
        public int ServiceId { get; set; }  
        public TimeSpan Duration { get; set; }
        public int UserId { get; set; }
        public bool Cancellation { get; set; } = false;
        
        private string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
        public Appointment() { }
        public Appointment(DateTime dateTime, Master master, Service service, User user) 
        {
            DateTime = dateTime;
            Duration = service.Duration;
            MasterId = master.Id;
            ServiceId = service.Id;
            UserId = user.Id;
        }

        public async Task InserAppointment(DateTime dateTime, Master master, Service service, User user) 
        {
            var appointment = new Appointment()
            {
                DateTime = dateTime,
                Duration = service.Duration,
                MasterId = master.Id,
                ServiceId = service.Id,
                UserId = user.Id,
            };
            using (var con = new NpgsqlConnection(connString))
            {
                await con.OpenAsync();
                string query = @"INSERT INTO appointments (date, duration, master_id, service_id, user_id, cancellation) VALUES
(@DateTime, @Duration, @MasterId, @ServiceId, @UserId, @Cancellation)";
                try
                {
                    await con.ExecuteAsync(query, appointment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: InserAppointment - "+ex.Message);
                }
            }
                
        }
        public void Show(List<Appointment> appointments) 
        {
            foreach (var i in appointments)
            {
                Console.WriteLine($"{i.DateTime}\t{i.MasterId}\t{i.ServiceId}\t{i.Duration}\t{i.UserId}\t{i.Cancellation}");
            }
        }
    }

}
