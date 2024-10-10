using ConsoleSchedule.Models;
using ConsoleSchedule.Repositories;
using Dapper;
using Npgsql;


namespace ConsoleSchedule.Services
{
    internal class DatabasePostgreSQL
    {
        private string _connString; 
        public DatabasePostgreSQL(string connectionString)
        {
            _connString = connectionString;
        }

        public async Task CreateDataBase(string dbName)
        {
            //string connString = $"Host=localhost;Username=postgres;Password=Sur999";
            //dbName = "mastersscheduledata";

            using (var conn = new NpgsqlConnection(_connString))
            {
                string checkDbQ = "SELECT 1 FROM pg_database WHERE datname = @dbname";
                try
                {
                    await conn.OpenAsync();
                    using (var command = new NpgsqlCommand(checkDbQ, conn))
                    {
                        command.Parameters.AddWithValue("dbname", dbName);
                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            string createDbQ = $"CREATE DATABASE \"{dbName}\""; // Используйте кавычки для имен с заглавными буквами
                            using (var createCommand = new NpgsqlCommand(createDbQ, conn))
                            {
                                await createCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public async Task CreateTables()
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                await conn.OpenAsync();

                string query = @"CREATE TABLE IF NOT EXISTS users (
                                    id SERIAL PRIMARY KEY,
                                    name VARCHAR(30) NOT NULL,
                                    phone_number VARCHAR(15) NOT NULL 
                                 )";
                try
                {
                    await conn.ExecuteAsync(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                };

                string sql = @"CREATE TABLE IF NOT EXISTS masters (
                                id SERIAL PRIMARY KEY,
                                name VARCHAR(30) NOT NULL,
                                day_interval INTERVAL NOT NULL,
                                speciality VARCHAR(30)
                              )";
                try
                {
                    await conn.ExecuteAsync(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                };

                sql = @"CREATE TABLE IF NOT EXISTS services (
                          id SERIAL PRIMARY KEY,
                          name VARCHAR(50) NOT NULL,
                          duration INTERVAL DEFAULT '30 minutes',
                          master_id INTEGER,
                        FOREIGN KEY (master_id) REFERENCES masters (id) ON DELETE CASCADE
                       )";
                try
                {
                    await conn.ExecuteAsync(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                };

                sql = @"CREATE TABLE IF NOT EXISTS appointments (
                            id SERIAL PRIMARY KEY,
                            date TIMESTAMP NOT NULL,
                            duration INTERVAL NOT NULL,
                            master_id INTEGER NOT NULL,                           
                            service_id INTEGER NOT NULL,                                                  
                            user_id INTEGER NOT NULL,   
                            Cancellation BOOLEAN DEFAULT FALSE, 
                       FOREIGN KEY (master_id) REFERENCES masters(id) ON DELETE SET NULL,
                       FOREIGN KEY (service_id) REFERENCES services(id) ON DELETE SET NULL,
                       FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
                     )";
                try
                {
                    await conn.ExecuteAsync(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }

            }
        }
        public async Task CreateTestRecords()
        {
            //добавление пользователей
            bool hasRecords = await HasRecords("users");
            if (hasRecords == false)
            {
                var users = new List<User>()
                {
                  new User(){Name= "userBob", PhoneNumber= "+79101100000" },
                  new User(){Name = "userTom", PhoneNumber= "+89061002222"},
                  new User(){Name= "userDeadPool", PhoneNumber = "+79001001111"},
                  new User(){Name = "userTerminator", PhoneNumber= "+89031002233"},
                };
                var userRepo = new UserRepository(_connString);
                foreach (var u in users)
                {
                    await userRepo.AddUser(u);
                }
            }
            //добавление мастеров и сервисов
            hasRecords = await HasRecords("masters");
            if (hasRecords == false) 
            {
                var masters = new List<Master>()
                {
                  new Master(){Name="masterDasha", Speciality="massage",Day_interval=new TimeSpan(1,00,0)},
                  new Master(){Name="masterOlesya", Speciality="barber",Day_interval=new TimeSpan(0,30,0)}
                };
                var masterRepo = new MasterRepository(_connString);
                foreach (var m in masters)
                {
                    await masterRepo.AddMaster(m);
                }
                var master1 = await masterRepo.GetMasterById(1);
                var master2 = masterRepo.GetMasterById(2);
                //добавление сервисов
                var services = new List<Service>()
                {
                  new Service("massage-Top", new TimeSpan(1,0,0), master1),
                  new Service("massage-full", new TimeSpan(2,0,0), master1),
                  new Service("massage-Thai", new TimeSpan(3,0,0), master1),
                  new Service("haircut-man", new TimeSpan(0,30,0), await master2),
                  new Service("haircut-woman", new TimeSpan(1,00,0), await master2),
                  new Service("hair-coloring", new TimeSpan(2,00,0), await master2)
                };
                var serviceRepo = new ServiceRepository(_connString);
                foreach (var s in services)
                {
                    await serviceRepo.AddServiceAsync(s);
                }
            }
        }

        public async Task<bool> HasRecords(string tableName)
        {
            using (var con = new NpgsqlConnection(_connString))
            {
                try
                {
                    string sql = $"SELECT COUNT(*) FROM \"{tableName}\"";
                    await con.OpenAsync();
                    var count = await con.ExecuteScalarAsync<int>(sql);
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error DatabasePostgreSQL, HasRecords(): " + ex.Message);
                    throw;
                }
            }
        }
    }
}
