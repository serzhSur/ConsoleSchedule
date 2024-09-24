using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Collections;
using ConsoleSchedule.models;


namespace DbCreation
{
    internal class MasterRepository
    {
       
        private string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";

        public MasterRepository(string connString) 
        { 
            this.connString = connString ;
        }
        public async Task AddMasterAsync(MasterRepository master)
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
        public async Task<Master>  GetMasterById(int id) 
        {
            using (var con = new NpgsqlConnection(connString)) 
            {
              
                string query = "SELECT * FROM masters WHERE id = @Id";
                try 
                {
                    await con.OpenAsync();
                    return await con.QueryFirstOrDefaultAsync<Master>(query, new { Id = id});
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: class Master"+ex.Message);
                    return null;
                }
            }
                
        }

    }
}
