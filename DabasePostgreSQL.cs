using Dapper;
using Npgsql;


namespace DbCreation
{
    internal class DabasePostgreSQL
    {
        private string connString { get; } = "Host=localhost;Username=postgres;Password=Sur999; Database=mastersscheduledata";

        public async Task CreateDataBase()
        {
            string connString = $"Host=localhost;Username=postgres;Password=Sur999";
            string dbName = "mastersscheduledata";

            using (var conn = new NpgsqlConnection(connString))
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
                }
            }
        }

        public async Task CreateTables()
        {
            using (var conn = new NpgsqlConnection(connString))
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
                };

                string createTableQ = @"CREATE TABLE IF NOT EXISTS masters (
id SERIAL PRIMARY KEY,
name VARCHAR(30) NOT NULL
)";
                try
                {
                    await conn.ExecuteAsync(createTableQ);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                };

                createTableQ = @"CREATE TABLE IF NOT EXISTS services (
id SERIAL PRIMARY KEY,
name VARCHAR(50) NOT NULL,
duration INTERVAL NOT NULL,
master_id INTEGER,
FOREIGN KEY (master_id) REFERENCES masters (id) ON DELETE CASCADE
)";
                try
                {
                    await conn.ExecuteAsync(createTableQ);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                };

                createTableQ = @"CREATE TABLE IF NOT EXISTS appointments (
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
                    await conn.ExecuteAsync(createTableQ);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                createTableQ = @"CREATE TABLE IF NOT EXISTS dayschedules (
id SERIAL PRIMARY KEY,
date TIMESTAMP NOT NULL,
interval INTERVAL NOT NULL,
busy BOOLEAN DEFAULT FALSE,
master_id INTEGER,
appointment_id INTEGER, 
FOREIGN KEY (appointment_id) REFERENCES appointments(id) ON DELETE SET NULL,
FOREIGN KEY (master_id) REFERENCES masters(id) ON DELETE CASCADE    
)";
                try
                {
                    await conn.ExecuteAsync(createTableQ);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                };

            }
        }
    }
}
