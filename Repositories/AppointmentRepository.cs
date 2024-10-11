using ConsoleSchedule.Models;
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
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, appointment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error class AppointmentRepository, InserAppointment: " + ex.Message);
                    throw;
                } 
            }
        }

        public async Task<Appointment> GetAppointmentById(int id) 
        {
            using (var con = new NpgsqlConnection(_connectionString)) 
            {
                string sql = @"SELECT * FROM appointments WHERE id = @Id";
                try 
                { 
                    await con.OpenAsync();
                    var appointment= await con.QuerySingleOrDefaultAsync<Appointment>(sql, new {Id = id });
                    if (appointment == null) 
                    {
                        throw new InvalidOperationException($"Appointment id:{id} Not Found");
                    }
                    return appointment;
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine("Error class AppointmentRepository, GetAppointmentById "+ex.Message);
                    throw;
                }
            }
        }
        public async Task<List<Appointment>> GetAllAppointments(int masterId)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM public.appointments WHERE cancellation = @Cancel AND master_id = @Id";
                try
                {
                    await con.OpenAsync();
                    var appointments = await con.QueryAsync<Appointment>(query, new { Cancel=false, Id= masterId });
                    return appointments.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error class AppointmentRepository, GetAllAppointments: " + ex.Message);
                    throw;
                }
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
                    Console.WriteLine("Error class AppointmentRepository, DeleteAppointment " + ex.Message);
                    throw;
                }
            }
        }

        public async Task UpdateAppointment(Appointment appointment) 
        {
            using (var con = new NpgsqlConnection(_connectionString)) 
            {
                string sql = @"UPDATE appointments SET 
date = @Date, duration = @Duration, master_id = @Master_id, 
service_id = @Service_id, user_id = @User_id, 
cancellation = @Cancellation 
WHERE id = @Id";
                try 
                { 
                    await con.OpenAsync();
                    await con.ExecuteAsync(sql, appointment);
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine("Error class AppointmentRepository, UpdeteAppointment() "+ex.Message);
                    throw;
                }
            }
        }

        

    }
}
