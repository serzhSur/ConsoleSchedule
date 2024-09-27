using ConsoleSchedule.models;
using Npgsql;
using Dapper;

namespace ConsoleSchedule
{
    internal class AppointmentRepository
    {
        private string _connectionString;

        public AppointmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task InsertAppointment(Appointment appointment)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO appointments (date, duration, master_id, service_id, user_id, cancellation) 
                    await con.OpenAsync();
VALUES (@Date, @Duration, @Master_id, @Service_id, @User_id, @Cancellation)";
                try
                {
                    await con.ExecuteAsync(query, appointment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR class AppointmentRepository, InserAppointment: " + ex.Message);
                }

            }
        }
        public async Task<List<Appointment>> GetAllAppointments()
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM public.appointments";
                try
                {
                    await con.OpenAsync();
                    var appointments = await con.QueryAsync<Appointment>(query);
                    return appointments.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR class AppointmentRepository, GetAllAppointments: " + ex.Message);
                    throw;
                }
            }
        }

        public async Task MakeAppointment(Appointment appointment)//, AppointmentRepository repository)
        {
            List<Appointment> appointments = await GetAllAppointments();

            List<DateTime> busyTime = new List<DateTime>();
            TimeSpan dayInterval = appointment.DayInterval;
            foreach (var a in appointments)
            {
                for (var i = a.Date; i < a.Date + a.Duration; i += dayInterval)
                {
                    busyTime.Add(i);
                }
            }

            var plannedTime = new List<DateTime>();
            for (var t = appointment.Date; t < appointment.Date + appointment.Duration; t += appointment.DayInterval)
            {
                plannedTime.Add(t);
            }
            bool timeIsBusy = false;
            foreach (var t in plannedTime)
            {
                timeIsBusy = busyTime.Contains(t);
                if (timeIsBusy == true)
                {
                    Console.WriteLine("Time interval is busy");
                    break;
                }
            }
            if (timeIsBusy == false)
            {
                await InsertAppointment(appointment);
                //busyTime.AddRange(plannedTime);
                //appointments.Add(appointment);
                Console.WriteLine($"Appointment id: {appointment.Id} is Created");
            }
        }

    }
}
