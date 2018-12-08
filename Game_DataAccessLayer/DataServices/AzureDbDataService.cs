using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_DomainLayer;

namespace Game_DataAccessLayer
{
    /// <summary>
    /// Azure database data service
    /// </summary>
    public class AzureDbDataService : IDataService
    {
        static string _connectionString;

        public AzureDbDataService()
        {
            _connectionString = AzureDbDataSettings.connectionString;
        }

        /// <summary>
        /// Retrieve a list of all Games in the database
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<Game> ReadAllGames(out string error_message)
        {
            List<Game> gameList = new List<Game>();
            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string s = "SELECT * FROM GameView ORDER BY name, publisher_name, format";
                    SqlCommand cmd = new SqlCommand(s, connection);
                    cmd.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            Id = reader.GetOrdinal("id"),
                            GameName = reader.GetOrdinal("name"),
                            FormatId = reader.GetOrdinal("format_id"),
                            FormatName = reader.GetOrdinal("format"),
                            PublisherId = reader.GetOrdinal("publisher_id"),
                            PublisherName = reader.GetOrdinal("publisher_name"),
                            MinimumPlayerCount = reader.GetOrdinal("minimum_player_count"),
                            MaximumPlayerCount = reader.GetOrdinal("maximum_player_count"),
                            ReleaseDate = reader.GetOrdinal("release_date"),
                            Discontinued = reader.GetOrdinal("discontinued"),
                            Rating = reader.GetOrdinal("rating"),
                            Comment = reader.GetOrdinal("comment")
                        };

                        while (reader.Read() == true)
                        {
                            Game temp = new Game
                            {
                                Id = reader.GetInt32(ordinals.Id),
                                GameName = reader.GetString(ordinals.GameName),
                                FormatId = reader.GetInt32(ordinals.FormatId),
                                FormatName = reader.GetString(ordinals.FormatName),
                                PublisherId = reader.GetInt32(ordinals.PublisherId),
                                PublisherName = reader.GetString(ordinals.PublisherName),
                                MinimumPlayerCount = reader.GetInt32(ordinals.MinimumPlayerCount),
                                MaximumPlayerCount = reader.GetInt32(ordinals.MaximumPlayerCount),
                                ReleaseDate = Game.ConvertDateTimeToString(reader.GetDateTime(ordinals.ReleaseDate)),
                                Discontinued = reader.GetBoolean(ordinals.Discontinued),
                                Rating = reader.GetInt32(ordinals.Rating),
                                Comment = reader.GetString(ordinals.Comment),
                            };

                            gameList.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return gameList;
        }

        /// <summary>
        /// Retrieve  a list of all GameFormats in the database
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GameFormat> ReadAllGameFormats(out string error_message)
        {
            List<GameFormat> gameFormatList = new List<GameFormat>();
            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string s = "SELECT id, format FROM game_format UNION SELECT 0, '' ORDER BY format";
                    SqlCommand cmd = new SqlCommand(s, connection);
                    cmd.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            Id = reader.GetOrdinal("id"),
                            FormatName = reader.GetOrdinal("format")
                        };

                        while (reader.Read() == true)
                        {
                            GameFormat temp = new GameFormat
                            {
                                Id = reader.GetInt32(ordinals.Id),
                                FormatName = reader.GetString(ordinals.FormatName)
                            };

                            gameFormatList.Add(temp);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return gameFormatList;
        }

        /// <summary>
        /// Retrieve  a list of all GamePublishers in the database
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GamePublisher> ReadAllGamePublishers(out string error_message)
        {
            List<GamePublisher> gamePublisherList = new List<GamePublisher>();
            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string s = "SELECT id, name FROM game_publisher UNION SELECT 0, '' ORDER BY name";
                    SqlCommand cmd = new SqlCommand(s, connection);
                    cmd.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            Id = reader.GetOrdinal("id"),
                            PublisherName = reader.GetOrdinal("name")
                        };

                        while (reader.Read() == true)
                        {
                            GamePublisher temp = new GamePublisher
                            {
                                Id = reader.GetInt32(ordinals.Id),
                                PublisherName = reader.GetString(ordinals.PublisherName)
                            };

                            gamePublisherList.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return gamePublisherList;
        }

        /// <summary>
        /// Write the current list of games to the database
        /// </summary>
        /// <param name="games">list of Games</param>
        public void WriteAllGames(IEnumerable<Game> games, out string error_message)
        {
            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    //
                    // delete all games in the table to reset the collection
                    //
                    string sd = "DELETE GameView";
                    SqlCommand del = new SqlCommand(sd, connection);
                    del.Connection.Open();

                    string val = ProcessGamesForInsert(games.ToList());

                    if (val != "")
                    {
                        string si = "INSERT GameView (comment, discontinued, format_id, maximum_player_count, minimum_player_count, name, publisher_id, rating, release_date)" +
                                    "VALUES";
                        SqlCommand ins = new SqlCommand(si, connection);
                        ins.Connection.Open();
                    }
                    else
                    {
                        error_message = "";
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }


        }

        /// <summary>
        /// Scrub the data before inserting to the database
        /// </summary>
        /// <param name="gameList"></param>
        /// <returns></returns>
        private string ProcessGamesForInsert(List<Game> gameList)
        {
            string games = "";

            StringBuilder sb = new StringBuilder("");

            foreach (Game g in gameList)
            {
                if (!string.IsNullOrEmpty(g.GameName.Trim()))
                {
                    sb.Append("("
                                + (string.IsNullOrEmpty(g.Comment.Trim()) ? "NULL" : "'" + g.Comment + "'") + ","
                                + (g.Discontinued ? 1 : 0).ToString() + ","
                                + (string.IsNullOrEmpty(g.FormatId.ToString()) || g.FormatId == 0 ? "NULL" : g.FormatId.ToString()) + ","
                                + (string.IsNullOrEmpty(g.MaximumPlayerCount.ToString()) || g.MaximumPlayerCount == 0 ? "NULL" : g.MaximumPlayerCount.ToString()) + ","
                                + (string.IsNullOrEmpty(g.MinimumPlayerCount.ToString()) || g.MinimumPlayerCount == 0 ? "NULL" : g.MinimumPlayerCount.ToString()) + ","
                                + "'" + g.GameName + "'" + ","
                                + (string.IsNullOrEmpty(g.PublisherId.ToString()) || g.PublisherId == 0 ? "NULL" : g.PublisherId.ToString()) + ","
                                + (string.IsNullOrEmpty(g.Rating.ToString()) || g.Rating == 0 ? "NULL" : g.Rating.ToString()) + ","
                                + (string.IsNullOrEmpty(g.ReleaseDate.ToString()) ? "NULL" : "'" + g.ReleaseDate.ToString()) + "'"
                                + "),"
                              );
                }
                else
                {
                    gameList.Remove(g);
                }
            }

            games = sb.ToString().TrimEnd(',');

            return games;
        }
    }
}