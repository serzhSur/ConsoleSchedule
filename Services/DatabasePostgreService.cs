using ConsoleSchedule.Models;
using ConsoleSchedule.Repositories;
using Dapper;
using Npgsql;


namespace ConsoleSchedule.Services
{
    internal class DatabasePostgreService
    {
        private string _connString;
        public DatabasePostgreService(string connectionString)
        {
            _connString = connectionString;
        }

        public async Task CreateDataBase(string postgresConnection, string dbName)
        {
            using (var conn = new NpgsqlConnection(postgresConnection))
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
                            string sql = $"CREATE DATABASE \"{dbName}\"";
                            using (var createCommand = new NpgsqlCommand(sql, conn))
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
                                start_day_time INTERVAL NOT NULL,
                                end_day_time INTERVAL NOT NULL,
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
            var masterRepo = new MasterRepository(_connString);
            var serviceRepo = new ServiceRepository(_connString);
            var userRepo = new UserRepository(_connString);
            var appRepo = new AppointmentRepository(_connString);
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
                  new Master("masterDasha", "massage",new TimeSpan(1,00,0),new TimeSpan(8,00,0), new TimeSpan(20,0,0)),
                  new Master("masterOlesya", "barber",new TimeSpan(0,30,0), new TimeSpan(9,00,0), new TimeSpan(18,0,0))
                };
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
                foreach (var s in services)
                {
                    await serviceRepo.AddServiceAsync(s);
                }
            }
            //добавление записей к мастеру
            hasRecords = await HasRecords("appointments");
            if (hasRecords == false)
            {
                await CreateAppointmentTestRecords(new DateTime(2024, 10, 14, 10, 0, 0), 1, masterRepo, serviceRepo, userRepo, appRepo);
                await CreateAppointmentTestRecords(new DateTime(2024, 10, 14, 9, 0, 0), 2, masterRepo, serviceRepo, userRepo, appRepo);
            }
        }

        private async Task<bool> HasRecords(string tableName)
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
                    Console.WriteLine("Error DatabasePostgreService, HasRecords(): " + ex.Message);
                    throw;
                }
            }
        }
        private static async Task CreateAppointmentTestRecords(DateTime date, int masterId, MasterRepository masterRepo, ServiceRepository serviceRepo, UserRepository userRepo, AppointmentRepository appRepo)
        {
            var interval = new TimeSpan(1, 0, 0);
            var master = await masterRepo.GetMasterById(masterId);
            var services = new List<Service>(await serviceRepo.GetMasterServices(master));
            var users = new List<User>(await userRepo.GetAllUsers());
            if (!services.Any() || !users.Any())
            {
                throw new InvalidOperationException("Error class DatabasePostgreService, CreateTestRecords: Empty List (services or users)");
            }

            var appointments = new List<Appointment>();
            for (int i = 0; i < services.Count; i++)
            {
                var user = users[i % users.Count];
                var appointment = new Appointment(date, master, services[i], user);
                appointments.Add(appointment);
                date = date.Add(services[i].Duration + interval);
            }
            foreach (var a in appointments)
            {
                await appRepo.InsertAppointment(a);
            }
        }
    }
}
