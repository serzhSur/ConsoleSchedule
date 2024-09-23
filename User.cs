using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCreation
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNamber { get; set; }

        public async Task AddUserAsync(User user)
        {
            string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
            using (var con = new NpgsqlConnection(connString))
            {
                var query = @"INSERT INTO users (name, phone_number) 
                          VALUES (@Name, @PhoneNamber)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }

}
