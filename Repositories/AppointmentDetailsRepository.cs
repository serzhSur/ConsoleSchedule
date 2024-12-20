﻿using VizitConsole.Models;
using Dapper;
using Npgsql;

namespace VizitConsole.Repositories
{
    internal class AppointmentDetailsRepository
    {
        public string _connectionString;
        public AppointmentDetailsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task <IEnumerable<AppointmentDetails>> GetAll(Master master) 
        {
            using (var con = new NpgsqlConnection(_connectionString)) 
            {
                int masterId=master.Id;
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
                    users u ON a.user_id = u.id
                WHERE a.master_id = @id
                ORDER BY a.date ASC";
                    await con.OpenAsync();
                    return await con.QueryAsync<AppointmentDetails>(query, new { id = masterId});
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
