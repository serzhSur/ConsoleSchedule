using ConsoleSchedule.models;
using Dapper;
using Npgsql;

namespace ConsoleSchedule.Repositories
{
    internal class AppointmentDetailsRepository
    {
        public string _connectionString;

        public AppointmentDetailsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<AppointmentDetails> GetAll() 
        {
            using (var con = new NpgsqlConnection(_connectionString)) 
            {
                try 
                {
                    string query = @"SELECT 
                    a.id,
                    a.date,
                    a.duration,
                    a.cancellation,
                    m.name AS MasterName,
                    s.name AS ServiceName,
                    s.duration AS ServiceDuration,
                    u.name AS UserName,
                    u.phone_number AS UserPhone
                FROM 
                    appointments a
                JOIN 
                    masters m ON a.master_id = m.id
                JOIN 
                    services s ON a.service_id = s.id
                JOIN 
                    users u ON a.user_id = u.id";
                    con.Open();
                    return con.Query<AppointmentDetails>(query);
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine("ERROR class AppointmentDetailsRepository, GetAll() " + ex.Message);
                }
                return Enumerable.Empty<AppointmentDetails>();
                
            }
        }

    }
}
