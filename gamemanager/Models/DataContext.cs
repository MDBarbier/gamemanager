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

        internal object GetGame(int id)
        {
            GameEntry game = new GameEntry();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from games where id = @p";
                    cmd.Parameters.AddWithValue("p", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new GameEntry()
                            {
                                Id = (long)reader["id"],
                                Name = reader["name"].ToString(),
                                Price = (decimal)reader["price"],
                                Genre = reader["genre"].ToString(),
                                Owned = (bool)reader["owned"],
                                Notes = reader["notes"].ToString(),
                                Ranking = reader["ranking"] == DBNull.Value ? (short?)-1 : (short)reader["ranking"],
                                Rating = reader["rating"] == DBNull.Value ? (short?)-1 : (short)reader["rating"]
                            };
                        }
                    }
                }
            }

            return game;
        }

        internal bool InsertGame(GameEntry game)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO games (name, genre, owned, price, notes, ranking, rating)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4, @p5, @p6, @p7)";
                        cmd.Parameters.AddWithValue("p", game.Name);
                        cmd.Parameters.AddWithValue("p2", game.Genre);
                        cmd.Parameters.AddWithValue("p3", game.Owned);
                        cmd.Parameters.AddWithValue("p4", game.Price);
                        cmd.Parameters.AddWithValue("p5", game.Notes);
                        cmd.Parameters.AddWithValue("p6", game.Ranking);
                        cmd.Parameters.AddWithValue("p7", game.Rating);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error trying to insert the row", ex);
                }
            }

        }

        internal bool EditGame(GameEntry game)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE games SET name = @p, genre = @p2, owned = @p3,";
                        cmd.CommandText += " price = @p4, notes = @p5, ranking = @p6, rating = @p7 ";
                        cmd.CommandText += " WHERE id = @p8";
                        cmd.Parameters.AddWithValue("p", game.Name);
                        cmd.Parameters.AddWithValue("p2", game.Genre);
                        cmd.Parameters.AddWithValue("p3", game.Owned);
                        cmd.Parameters.AddWithValue("p4", game.Price);
                        cmd.Parameters.AddWithValue("p5", game.Notes);
                        cmd.Parameters.AddWithValue("p6", game.Ranking);
                        cmd.Parameters.AddWithValue("p7", game.Rating);
                        cmd.Parameters.AddWithValue("p8", game.Id);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error trying to insert the row", ex);
                }
            }

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
                            Owned = (bool)reader["owned"],
                            Notes = reader["notes"].ToString(),
                            Ranking = reader["ranking"] == DBNull.Value ? (short?)-1 : (short)reader["ranking"],
                            Rating = reader["rating"] == DBNull.Value ? (short?)-1 : (short)reader["rating"]
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
