using Npgsql;
using System;
using System.Collections.Generic;

namespace gamemanager.Models
{
    public class DataContext
    {
        //Property to hold connection string
        public string ConnectionString { get; set; }

        //Constructor
        public DataContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        //Method to acquire a new connection - to be called from "using" statement to ensure lifecycle management
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        //Method to get data
        public List<DLister> GetData()
        {
            List<DLister> list = new List<DLister>();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                NpgsqlCommand cmd;

                cmd = new NpgsqlCommand("select * from test_table", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DLister()
                        {
                            Id = (int)reader["id"],
                            Name = reader["name"].ToString(),
                            Age = (int)reader["age"]
                        });
                    }
                }
            }

            return list;
        }

        public List<GameEntry> GetAllGames()
        {
            List<GameEntry> list = new List<GameEntry>();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                NpgsqlCommand cmd;

                cmd = new NpgsqlCommand("select * from games", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GameEntry()
                        {
                            Id = (long)reader["id"],
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Genre = reader["genre"].ToString(),
                            Owned = (bool)reader["owned"]
                        });
                    }
                }
            }

            return list;
        }

        private void InsertNewRow(string name, string age)
        {            
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                bool result = int.TryParse(age, out int parsedAge);

                if (!result)
                    throw new ArgumentException("The value passed in for age was not a valid number");

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO test_table (name, age) VALUES (@p, @p2)";
                    cmd.Parameters.AddWithValue("p", name);
                    cmd.Parameters.AddWithValue("p2", parsedAge);
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }
}
