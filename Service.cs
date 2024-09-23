using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation
{
    internal class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public int Master_id { get; set; }

        public async Task AddServiceAsync(Service service)
        {
            string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
            using (var con = new NpgsqlConnection(connString))
            {
                string query = @"INSERT INTO services (name, duration, master_id) VALUES (@Name, @Duration, @Master_id)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, service);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: Task AddServiceAsync - " + ex.Message);
                }
            }
        }
    }
}
