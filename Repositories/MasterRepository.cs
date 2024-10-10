using Npgsql;
using Dapper;
using ConsoleSchedule.Models;


namespace ConsoleSchedule.Repositories
{
    internal class MasterRepository
    {

        private string _connString;

        public MasterRepository(string connString)
        {
            _connString = connString;
        }
        public async Task AddMasterAsync(Master master)
        {
            using (var con = new NpgsqlConnection(_connString))
            {
                var query = @"INSERT INTO masters (name, day_interval, speciality) 
                              VALUES (@Name, @Day_interval, @Speciality)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, master);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: MasterRepository, AddMasterAsync - " + ex.Message);
                }
            }
        }
        public async Task<Master> GetMasterById(int id)
        {
            using (var con = new NpgsqlConnection(_connString))
            {
                try
                {
                    string query = "SELECT * FROM masters WHERE id = @Id";
                    await con.OpenAsync();
                    var master = await con.QueryFirstOrDefaultAsync<Master>(query, new { Id = id });
                    if (master == null)
                    {
                        throw new KeyNotFoundException($"master id-{id} not founf");
                    }
                    return master;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR Master, GetMasterById: " + ex.Message);
                    throw;
                }
            }
        }

    }
}
