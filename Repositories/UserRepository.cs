using ConsoleSchedule.models;
using Npgsql;
using Dapper;


namespace ConsoleSchedule.Repositories
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
                    throw; // Это позволит вызывающему коду узнать о возникшей ошибке
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
                        throw new KeyNotFoundException($"User id-{id} not found");
                    }
                    return user;
                }
                catch (Exception ex)
                {
                    // Логирование исключения
                    Console.WriteLine("ERROR UserRepository, GetUserById: " + ex.Message);
                    throw;// Повторно выбрасываем исключение для дальнейшей обработки
                }
            }
        }
    }
}
