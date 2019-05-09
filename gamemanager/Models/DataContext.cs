using gamemanager.Code;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gamemanager.Models
{
    public class DataContext
    {
        /// <summary>
        /// Property to hold connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Constructor that sets the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public DataContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Method to acquire a new connection - to be called from "using" statement to ensure lifecycle management
        /// </summary>
        /// <returns></returns>
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        /// <summary>
        /// Gets the next available ranking
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        internal string GetNextRanking(ItemType itemType)
        {
            int possibleRank = 1;
            bool foundFreeRanking = false;

            switch (itemType)
            {
                case ItemType.Game:

                    var games = GetAllGames().Where(a => a.Owned == false);

                    while (!foundFreeRanking)
                    {
                        var match = games.Where(a => a.Ranking == possibleRank).FirstOrDefault();

                        if (match == null)
                        {
                            foundFreeRanking = true;
                            continue;
                        }

                        possibleRank++;
                    }

                    break;

                case ItemType.Dlc:

                    var dlcs = GetAllDlc();

                    while (!foundFreeRanking)
                    {
                        var match = dlcs.Where(a => a.Ranking == possibleRank).FirstOrDefault();

                        if (match == null)
                        {
                            foundFreeRanking = true;
                            continue;
                        }


                        possibleRank++;
                    }

                    break;

                default:
                    throw new Exception("Type not recognised");
            }

            return possibleRank.ToString();
        }

        /// <summary>
        /// Gets all DLC
        /// </summary>
        /// <returns></returns>
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
                            Store = reader["store"].ToString(),
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

        /// <summary>
        /// Deletes the specified DLC from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool DeleteDlc(int id)
        {
            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();


                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from dlc where id = @p";
                    cmd.Parameters.AddWithValue("p", id);

                    try
                    {
                        cmd.ExecuteNonQuery();

                        //Ensure rankings consistent
                        AdjustRankings(ItemType.Dlc);

                        return true;
                    }
                    catch (Exception dex)
                    {
                        throw new Exception("There was a problem trying to delete the record", dex);
                    }
                }
            }
        }

        /// <summary>
        /// Fixes any inconsistencies in the rankings
        /// </summary>
        /// <param name="itemType"></param>
        internal void AdjustRankings(ItemType itemType)
        {
            int i = 0;

            switch (itemType)
            {
                case ItemType.Game:

                    var games = GetAllGames().Where(a => a.Owned == false).ToList().OrderBy(a => a.Ranking);
                    i = 1;
                    foreach (var game in games)
                    {
                        game.Ranking = (short)i;
                        var result = EditGame(game);

                        if (!result)
                        {
                            throw new Exception("There was a problem updating the rankings");
                        }
                        else
                        {
                            i++;
                        }
                    }

                    break;

                case ItemType.Dlc:

                    var dlcs = GetAllDlc().Where(a => a.Owned == false).ToList().OrderBy(a => a.Ranking);
                    i = 1;
                    foreach (var dlc in dlcs)
                    {
                        dlc.Ranking = (short)i;
                        var result = EditDlc(dlc);

                        if (!result)
                        {
                            throw new Exception("There was a problem updating the rankings");
                        }
                        else
                        {
                            i++;
                        }
                    }

                    break;

                default:
                    throw new Exception("Unrecognised type");
            }
        }

        internal GameEntry GetGameByName(string name)
        {
            GameEntry game = new GameEntry();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from games where name = @p";
                    cmd.Parameters.AddWithValue("p", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new GameEntry()
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
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

        internal Dlc GetDlcByName(string name)
        {
            Dlc dlc = new Dlc();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from dlc where name = @p";
                    cmd.Parameters.AddWithValue("p", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dlc = new Dlc()
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
                                Price = (decimal)reader["price"],
                                Owned = (bool)reader["owned"],
                                Notes = reader["notes"].ToString(),
                                Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                                Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"],
                                ParentGameId = reader["parentgameid"] == DBNull.Value ? (long)-1 : (long)reader["parentgameid"]
                            };
                        }
                    }
                }
            }

            return dlc;
        }

        internal bool InsertStoreDataEntry(int id, string storeUrl)
        {
            if (string.IsNullOrEmpty(storeUrl) || id == 0)
            {
                return false;
            }

            int appIdCalculated = int.Parse(storeUrl.Split('/')[4].ToString());

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO storedata (storeurl, storename, appid, parentid)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4)";

                        cmd.Parameters.AddWithValue("p", storeUrl);
                        cmd.Parameters.AddWithValue("p2", "steam");
                        cmd.Parameters.AddWithValue("p3", appIdCalculated);
                        cmd.Parameters.AddWithValue("p4", id);

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

        internal bool InsertStoreDataDlcEntry(int id, string storeUrl)
        {
            int appIdCalculated = int.Parse(storeUrl.Split('/')[4].ToString());

            if (string.IsNullOrEmpty(storeUrl) || id == 0)
            {
                return false;
            }

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO storedatadlc (storeurl, storename, appid, parentid)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4)";

                        cmd.Parameters.AddWithValue("p", storeUrl);
                        cmd.Parameters.AddWithValue("p2", "steam");
                        cmd.Parameters.AddWithValue("p3", appIdCalculated);
                        cmd.Parameters.AddWithValue("p4", id);

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

        /// <summary>
        /// Edits a DLC entry to match the supplied object
        /// </summary>
        /// <param name="dlc"></param>
        /// <param name="adjustRankings"></param>
        /// <returns></returns>
        internal bool EditDlc(Dlc dlc, bool adjustRankings = false)
        {
            //If the game is owned, adjust rankings accordingly
            if (dlc.Owned == true)
            {
                dlc.Ranking = 0;
            }

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
                        cmd.CommandText += " price = @p4, notes = @p5, ranking = @p6, rating = @p7, store = @p9 ";
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

                        if (dlc.Store != null)
                        {
                            cmd.Parameters.AddWithValue("p9", dlc.Store);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p9", DBNull.Value);
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
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error trying to update the row", ex);
                }
            }

            //Ensure rankings consistent
            if (adjustRankings)
            {
                AdjustRankings(ItemType.Dlc);
            }

            return true;

        }

        /// <summary>
        /// Gets a game matching the supplied ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                                Store = reader["store"].ToString(),
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

        /// <summary>
        /// Gets a game which matches the supplied ranking
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        internal GameEntry GetGameByRanking(int rank)
        {
            GameEntry game = new GameEntry();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from games where ranking = @p";
                    cmd.Parameters.AddWithValue("p", rank);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new GameEntry()
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
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

        /// <summary>
        /// Gets a DLC which matches the supplied ranking
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        internal Dlc GetDlcByRanking(int rank)
        {
            Dlc dlc = new Dlc();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from dlc where ranking = @p";
                    cmd.Parameters.AddWithValue("p", rank);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dlc = new Dlc()
                            {
                                Id = (int)reader["id"],
                                ParentGameId = (long)reader["parentgameid"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
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

            return dlc;
        }

        /// <summary>
        /// Deletes a game item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool DeleteGame(int id)
        {

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();


                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from games where id = @p";
                    cmd.Parameters.AddWithValue("p", id);

                    try
                    {
                        cmd.ExecuteNonQuery();

                        //Ensure rankings consistent
                        AdjustRankings(ItemType.Game);

                        return true;
                    }
                    catch (Exception dex)
                    {
                        throw new Exception("There was a problem trying to delete the record", dex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a single DLC item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                                Store = reader["store"].ToString(),
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

        /// <summary>
        /// Gets all dlc for a specified game
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                            Store = reader["store"].ToString(),
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

        /// <summary>
        /// Creates a new game entry
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
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
                        cmd.CommandText = "INSERT INTO games (name, genre, owned, price, notes, ranking, rating, store)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                        if (game.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p", game.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }

                        if (game.Store != null)
                        {
                            cmd.Parameters.AddWithValue("p8", game.Store);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p8", DBNull.Value);
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

        /// <summary>
        /// Creates a new DLC entry
        /// </summary>
        /// <param name="dlc"></param>
        /// <returns></returns>
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
                        cmd.CommandText = "INSERT INTO dlc (parentgameid, name, owned, price, notes, ranking, rating, store)";
                        cmd.CommandText += " VALUES (@p, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                        if (dlc.Name != null)
                        {
                            cmd.Parameters.AddWithValue("p2", dlc.Name);
                        }
                        else
                        {
                            throw new Exception("Name cannot be null!");
                        }

                        if (dlc.Store != null)
                        {
                            cmd.Parameters.AddWithValue("p8", dlc.Store);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p8", DBNull.Value);
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

        /// <summary>
        /// Edits a game entry
        /// </summary>
        /// <param name="game"></param>
        /// <param name="adjustRankings"></param>
        /// <returns></returns>
        internal bool EditGame(GameEntry game, bool adjustRankings = false)
        {

            //If the game is owned, adjust rankings accordingly
            if (game.Owned == true)
            {
                game.Ranking = 0;
            }

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
                        cmd.CommandText += " price = @p4, notes = @p5, ranking = @p6, rating = @p7, store = @p9 ";
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

                        if (game.Store != null)
                        {
                            cmd.Parameters.AddWithValue("p9", game.Store);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("p9", DBNull.Value);
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
                }
                catch (Exception ex)
                {
                    throw new Exception("There was an error trying to insert the row", ex);
                }
            }

            //Ensure rankings consistent
            if (adjustRankings)
            {
                AdjustRankings(ItemType.Game);
            }

            return true;
        }

        /// <summary>
        /// Gets all games
        /// </summary>
        /// <returns></returns>
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
                            Store = reader["store"].ToString(),
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

        public int GetGameAppId(string name)
        {
            GameEntry game = new GameEntry();
            StoreData storeData = new StoreData();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from games where name = @p";
                    cmd.Parameters.AddWithValue("p", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new GameEntry()
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
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

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from storedata where parentid = @p";
                    cmd.Parameters.AddWithValue("p", game.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            storeData = new StoreData()
                            {
                                Id = (long)reader["id"],
                                StoreName = reader["storename"].ToString(),
                                StoreUrl = reader["storeurl"].ToString(),
                                AppId = (int)reader["appid"],
                                ParentId = (int)reader["parentid"]
                            };
                        }
                    }
                }

                if (storeData != null)
                {
                    return storeData.AppId;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetDlcAppId(string name)
        {
            Dlc game = new Dlc();
            StoreDataDlc storeData = new StoreDataDlc();

            using (NpgsqlConnection conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from dlc where name = @p";
                    cmd.Parameters.AddWithValue("p", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game = new Dlc()
                            {
                                Id = (int)reader["id"],
                                ParentGameId = (long)reader["parentgameid"],
                                Name = reader["name"].ToString(),
                                Store = reader["store"].ToString(),
                                Price = (decimal)reader["price"],
                                Owned = (bool)reader["owned"],
                                Notes = reader["notes"].ToString(),
                                Ranking = reader["ranking"] == DBNull.Value ? (short)-1 : (short)reader["ranking"],
                                Rating = reader["rating"] == DBNull.Value ? (short)-1 : (short)reader["rating"]
                            };
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from storedatadlc where parentid = @p";
                    cmd.Parameters.AddWithValue("p", game.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            storeData = new StoreDataDlc()
                            {
                                Id = (long)reader["id"],
                                StoreName = reader["storename"].ToString(),
                                StoreUrl = reader["storeurl"].ToString(),
                                AppId = (int)reader["appid"],
                                ParentId = (int)reader["parentid"]
                            };
                        }
                    }
                }

                if (storeData != null)
                {
                    return storeData.AppId;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
