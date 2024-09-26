using ConsoleSchedule.models;
using Npgsql;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ConsoleSchedule
{
    internal class ServiceRepository 
    {
        private string _connectionString;
        public ServiceRepository(string connectionString) 
        { 
            _connectionString = connectionString;
        }
        public async Task AddServiceAsync(Service service)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                string query = @"INSERT INTO services (name, duration, master_id) 
VALUES (@Name, @Duration, @Master_id)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, service);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR ServiceRepository, AddServiceAsync: " + ex.Message);
                }
            }
        }

        public async Task<List<Service>> GetMasterServices(int masterId) 
        {
            using (var con = new NpgsqlConnection(_connectionString)) 
            {
                string query = @"Select * FROM services WHERE master_id = @Id";
                try
                {
                    await con.OpenAsync();
                    var services = await con.QueryAsync<Service>(query, new { Id = masterId });
                    if (services == null && !services.Any()) 
                    {
                        throw new KeyNotFoundException($"Services not found for masterId: {masterId}");
                    }
                    return services.ToList();
                }
                catch (Exception ex)
                { 
                    Console.WriteLine("ERROR ServiceRepository, AddServiceAsync: " +ex.Message);
                    throw;
                }
            }
        }
    }
}
