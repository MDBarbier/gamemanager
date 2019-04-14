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

        internal List<Dlc> GetAllDlc()
        {
            List<Dlc> list = new List<Dlc>();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                NpgsqlCommand cmd;

                cmd = new NpgsqlCommand("select * from dlc", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Dlc()
                        {
                            Id = (int)reader["id"],
                            ParentGameId = (long)reader["parentgameid"],
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Owned = (bool)reader["owned"],
                            Notes = reader["notes"].ToString(),
                            Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                            Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                        });
                    }
                }
            }

            return list;
        }

        internal bool EditDlc(Dlc dlc)
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
                        cmd.CommandText = "UPDATE dlc SET name = @p, parentgameid = @p2, owned = @p3,";
                        cmd.CommandText += " price = @p4, notes = @p5, ranking = @p6, rating = @p7 ";
                        cmd.CommandText += " WHERE id = @p8";

                        if (dlc.Id == 0)
                        {
                            throw new Exception("Id cannot be 0!");
                        }

                        if (dlc.ParentGameId == 0)
                        {
                            throw new Exception("Parent ID cannot be 0!");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p2", dlc.ParentGameId);
                        }

                        if (dlc.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p", dlc.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }                        

                        if (dlc.Notes != null)
                        {
                            cmd.Parameters.AddWithValue("p5", dlc.Notes);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p5", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("p3", dlc.Owned);
                        cmd.Parameters.AddWithValue("p4", dlc.Price);
                        cmd.Parameters.AddWithValue("p6", dlc.Ranking);
                        cmd.Parameters.AddWithValue("p7", dlc.Rating);
                        cmd.Parameters.AddWithValue("p8", dlc.Id);

                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error trying to update the row", ex);
                }
            }

        }

        internal GameEntry GetGame(int id)
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
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Price = (decimal)reader["price"],
                                Genre = reader["genre"].ToString(),
                                Owned = (bool)reader["owned"],
                                Notes = reader["notes"].ToString(),
                                Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                                Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                            };
                        }
                    }
                }
            }

            return game;
        }

        internal Dlc GetDlc(int id)
        {
            Dlc game = new Dlc();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from dlc where id = @p";
                    cmd.Parameters.AddWithValue("p", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new Dlc()
                            {
                                Id = (int)reader["id"],
                                ParentGameId = (long)reader["parentgameid"],
                                Name = reader["name"].ToString(),
                                Price = (decimal)reader["price"],
                                Owned = (bool)reader["owned"],
                                Notes = reader["notes"].ToString(),
                                Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                                Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                            };
                        }
                    }
                }
            }

            return game;
        }

        internal Dictionary<int, Dlc> GetDlcForGame(int id)
        {
            Dictionary<int, Dlc> data = new Dictionary<int, Dlc>();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();
                NpgsqlCommand cmd;

                cmd = new NpgsqlCommand("select * from dlc where parentgameid = @p ", conn);
                cmd.Parameters.AddWithValue("p", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add((int)reader["id"], new Dlc()
                        {
                            ParentGameId = (long)reader["parentgameid"],
                            Id = (int)reader["id"],
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],                            
                            Owned = (bool)reader["owned"],
                            Notes = reader["notes"].ToString(),
                            Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                            Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                        });
                    }
                }
            }

            return data;
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
                       
                        if (game.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p", game.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }

                        if (game.Genre != null)
                        {
                            cmd.Parameters.AddWithValue("p2", game.Genre);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p2", DBNull.Value);
                        }

                        if (game.Notes != null)
                        {
                            cmd.Parameters.AddWithValue("p5", game.Notes);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p5", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("p3", game.Owned);
                        cmd.Parameters.AddWithValue("p4", game.Price);
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

        internal bool InsertDlc(ViewModels.DlcViewModel dlc)
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
                        cmd.CommandText = "INSERT INTO dlc (parentgameid, name, owned, price, notes, ranking, rating)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4, @p5, @p6, @p7)";

                        if (dlc.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p2", dlc.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }

                        if (dlc.ParentGameId != 0)
                        {
                            cmd.Parameters.AddWithValue("p", dlc.ParentGameId);
                        }
                        else
                        {
                            throw new Exception("Parent game ID cannot be zero!");
                        }

                        if (dlc.Notes != null)
                        {
                            cmd.Parameters.AddWithValue("p5", dlc.Notes);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p5", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("p3", dlc.Owned);
                        cmd.Parameters.AddWithValue("p4", dlc.Price);
                        cmd.Parameters.AddWithValue("p6", dlc.Ranking);
                        cmd.Parameters.AddWithValue("p7", dlc.Rating);

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

                        if (game.Id == 0)
                        {
                            throw new Exception("Id cannot be 0!");
                        }

                        if (game.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p", game.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }

                        if (game.Genre != null)
                        {
                            cmd.Parameters.AddWithValue("p2", game.Genre);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p2", DBNull.Value);
                        }                       
                        
                        if (game.Notes != null)
                        {
                            cmd.Parameters.AddWithValue("p5", game.Notes);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p5", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("p3", game.Owned);
                        cmd.Parameters.AddWithValue("p4", game.Price);
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
                            Id = (int)reader["id"],
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Genre = reader["genre"].ToString(),
                            Owned = (bool)reader["owned"],
                            Notes = reader["notes"].ToString(),
                            Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                            Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                        });
                    }
                }
            }

            return list;
        }       
    }
}
