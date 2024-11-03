using VizitConsole.Models;
using Npgsql;
using Dapper;

namespace VizitConsole.Repositories
{
    internal class UserRepository
    {
        private string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }
            using (var con = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO users (name, phone_number) 
                          VALUES (@Name, @PhoneNumber)";
                try
                {
                    await con.OpenAsync();
                    await con.ExecuteAsync(query, user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw; 
                }
            }
        }
        public async Task<User> GetUserById(int id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    string query = "SELECT * FROM public.users WHERE id = @Id";
                    await con.OpenAsync();
                    var user = await con.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
                    if (user == null)
                    {
                        throw new KeyNotFoundException($"User id:{id} Not Found");
                    }
                    return user;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR UserRepository, GetUserById: " + ex.Message);
                    throw;
                }
            }
        }
        public async Task<IEnumerable<User>> GetAllUsers() 
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    string query = "SELECT * FROM users";
                    await con.OpenAsync();
                    return await con.QueryAsync<User>(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR UserRepository, GetAllUsers(): " + ex.Message);
                    throw;
                }
            }
        }
    }
}
