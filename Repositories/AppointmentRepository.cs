using ConsoleSchedule.models;
using Npgsql;
using Dapper;


namespace ConsoleSchedule.Repositories
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
                    Console.WriteLine($"Appointment {appointment.Date} can't be made: Time interval is busy ");
                    break;
                }
            }
            if (timeIsBusy == false)
            {
                await InsertAppointment(appointment);
                Console.WriteLine($"Appointment Date: {appointment.Date} is Created");
            }
        }

        public async Task DeleteAppointmentByDate(Appointment appointment)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                DateTime date = appointment.Date;
                int id = appointment.Master_id;
                string query = @"DELETE FROM appointments WHERE date = @Date AND master_id = @Id";
                try
                {
                    con.Open();
                    await con.ExecuteAsync(query, new { Date = date, Id = id });
                    Console.WriteLine($"Appointment date-{appointment.Date} Deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR class AppointmentRepository, DeleteAppointment " + ex.Message);
                }
            }
        }

        public bool CanBookAppointment(DateTime appointmentTime)
        {
            // Определяем границы интервала
            DateTime startTime = new DateTime(appointmentTime.Year, appointmentTime.Month, appointmentTime.Day, 10, 15, 0);
            DateTime endTime = new DateTime(appointmentTime.Year, appointmentTime.Month, appointmentTime.Day, 10, 45, 0);

            // Проверяем, попадает ли время записи в запрещенный интервал
            if (appointmentTime >= startTime && appointmentTime <= endTime)
            {
                return false; // Запись запрещена
            }

            return true; // Запись разрешена
        }

        public async Task MakeAppointment2(Appointment appointment)
        {
            List<Appointment> appointments = await GetAllAppointments();
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
                if (newTimeStart > t.start && newTimeStart > t.end || newTimeStart < t.start && newTimeEnd <= t.start)
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
                await InsertAppointment(appointment);
                Console.WriteLine($"Appointment Date: {appointment.Date} is Created");
            }
        }
        public async Task<List<(TimeSpan start, TimeSpan end)>> CreateBusyTime()
        {
            List<Appointment> appointments = await GetAllAppointments();
            foreach (var a in appointments)
            {
                a.EndTime = a.Date + a.Duration;
            }


            List<(TimeSpan start, TimeSpan end)> busyTime = new List<(TimeSpan, TimeSpan)>();
            foreach (var a in appointments)
            {
                busyTime.Add((a.Date.TimeOfDay, a.EndTime.TimeOfDay));
            }
            return busyTime;
        }
    }
}
