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
        public async Task AddMaster(Master master)
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
                    Console.WriteLine("ERROR: MasterRepository, AddMaster - " + ex.Message);
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
                    Console.WriteLine("ERROR MasterRepository, GetMasterById: " + ex.Message);
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Master>> GetAllMasters()
        {
            using (var con = new NpgsqlConnection(_connString))
            {
                try
                {
                    string query = "SELECT * FROM masters";
                    await con.OpenAsync();

                    return await con.QueryAsync<Master>(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR MasterRepository, GetAllMasters(): " + ex.Message);
                    throw;
                }
            }
        }

    }
}
