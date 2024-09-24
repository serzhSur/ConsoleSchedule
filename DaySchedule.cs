using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleSchedule.models;
using Dapper;
using Npgsql;

namespace DbCreation
{
    internal class DaySchedule
    {
        public DateTime Date { get; set; }
        public TimeSpan Interval { get; set; }
        public bool Busy { get; set; }
        public int MasterId { get; set; }
        public int? AppointmentId { get; set; }

       private string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
        public IEnumerable<DaySchedule> CreteDaySchedule(DateTime startTime, DateTime finishTime, TimeSpan interval, Master master)
        {
            var daySchedule = new List<DaySchedule>();
            for (DateTime i = startTime; i < finishTime; i += interval)
            {
                var dayInterval = new DaySchedule()
                {
                    Date = i,
                    Interval = interval,
                    Busy = false,
                    MasterId = master.Id,
                    AppointmentId = null
                };
                daySchedule.Add(dayInterval);
            }
            return daySchedule;
        }
        public async Task InsertDbDaySchedule(IEnumerable<DaySchedule> daySchedule)
        {
            string connString = $"Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";
            using (var con = new NpgsqlConnection(connString))
            {
                await con.OpenAsync();
                var query = @"INSERT INTO dayschedules (date, interval, busy, master_id, appointment_id) 
VALUES (@Date, @Interval, @Busy, @MasterId, @AppointmentId)";
                foreach (var interval in daySchedule)
                {
                    try
                    {
                        await con.ExecuteAsync(query, interval);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        public async Task <List<DaySchedule>>  GetAll(DateTime date) 
        {
            string query = @"SELECT * FROM dayschedules WHERE date(date) = @Date";
            using (var con = new NpgsqlConnection(connString)) 
            { 
                await con.OpenAsync();
                var result = await con.QueryAsync<DaySchedule>(query, new { Date = date });
                return result.ToList();
            }
        }

    }
}
