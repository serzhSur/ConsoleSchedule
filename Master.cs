using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace DbCreation
{
    internal class Master
    {
        public int Id { get; set; }
        public string Name { get; set; } = "undefined";    
        public TimeSpan DayInterval { get; set; }

        public async Task AddMasterAsync(Master master)
        {
            string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
            using (var con = new NpgsqlConnection(connString))
            {
                var query = @"INSERT INTO masters (name) 
                              VALUES (@Name)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, master);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: Task AddMasterAsync - " + ex.Message);
                }
            }
        }
    }
}
