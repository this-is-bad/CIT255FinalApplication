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
    public class AzureDBGameRepository : IGameRepository
    {
        List<Game> _games;
        List<GameFormat> _gameFormats;
        List<GamePublisher> _gamePublishers;

        public AzureDBGameRepository()
        {
            _games = new List<Game>();
            _gameFormats = new List<GameFormat>();
            _gamePublishers = new List<GamePublisher>();
        }

        /// <summary>
        /// Retrieve a list of Games
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<Game> GetAllGames(out string error_message)
        {
            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
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

                            _games.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            } 

            return _games;
        }

        /// <summary>
        /// Retrieve a single Game by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public Game GetById(int id, out string error_message)
        {
            error_message = "";

            Game game = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
                {
                    string query = $"SELECT * FROM GameView WHERE id = {id}";
                    SqlCommand cmd = new SqlCommand(query, connection);
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

                        while (reader.Read())
                        {

                            game.Id = reader.GetInt32(ordinals.Id);
                            game.GameName = reader.GetString(ordinals.GameName);
                            game.FormatId = reader.GetInt32(ordinals.FormatId);
                            game.FormatName = reader.GetString(ordinals.FormatName);
                            game.PublisherId = reader.GetInt32(ordinals.PublisherId);
                            game.PublisherName = reader.GetString(ordinals.PublisherName);
                            game.MinimumPlayerCount = reader.GetInt32(ordinals.MinimumPlayerCount);
                            game.MaximumPlayerCount = reader.GetInt32(ordinals.MaximumPlayerCount);
                            game.ReleaseDate = Game.ConvertDateTimeToString(reader.GetDateTime(ordinals.ReleaseDate));
                            game.Discontinued = reader.GetBoolean(ordinals.Discontinued);
                            game.Rating = reader.GetInt32(ordinals.Rating);
                            game.Comment = reader.GetString(ordinals.Comment);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message;
            }

            return game; 
        }

        /// <summary>
        /// Insert a new game into the database
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public string Insert(Game game)
        {
            string query = $"IF NOT EXISTS (SELECT 1 FROM GameView WHERE name = {game.GameName} AND format_id = {game.FormatId} AND publisher_id = {game.PublisherId})" + "\n"
                        + "INSERT GameView (comment, discontinued, format_id, maximum_player_count, "
                        + "minimum_player_count, name, publisher_id, rating, release_date)" + "\n"
                        + $"VALUES ({game.Comment},{game.Discontinued},{game.FormatId},{game.MaximumPlayerCount}, "
                        + $"{game.MinimumPlayerCount},{game.GameName},{game.PublisherId},{game.Rating},{game.ReleaseDate})";
                        
            return ExecuteSql(query);
        }

        public string Update(Game game)
        {
            string query = $"IF NOT EXISTS (SELECT 1 FROM GameView WHERE name = {game.GameName} AND format_id = {game.FormatId} AND publisher_id = {game.PublisherId})" + "\n"
                          + "UPDATE GameView" + "\n"
                          + $"SET comment = '{game.Comment}',discontinued = '{(game.Discontinued ? 1 : 0)}', format_id = {game.FormatId}, maximum_player_count = {game.MaximumPlayerCount}, "
                          + $"minimum_player_count = {game.MinimumPlayerCount}, name = '{game.GameName}' , publisher_id = {game.PublisherId}, rating = {game.Rating}, release_date = '{game.ReleaseDate}')"
                          + $"WHERE id = {game.Id}";

            return ExecuteSql(query);
        }

        public string Delete(int id)
        {

            string query = "DELETE GameView" + "\n"
                       + $"WHERE id = {id}";
           
            return ExecuteSql(query);
        }

        /// <summary>
        /// Execute a SQL query string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private string ExecuteSql(string query)
        {
            string error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message;
            }

            return error_message;
        }

        /// <summary>
        /// Retrieve a list of GameFormats
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GameFormat> GetAllGameFormats(out string error_message)
        {

            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
                {
                    string query = "SELECT id, format FROM game_format UNION SELECT 0, '' ORDER BY format";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            Id = reader.GetOrdinal("id"),
                            FormatName = reader.GetOrdinal("format")
                        };

                        while (reader.Read())
                        {
                            GameFormat temp = new GameFormat
                            {
                                Id = reader.GetInt32(ordinals.Id),
                                FormatName = reader.GetString(ordinals.FormatName)
                            };

                            _gameFormats.Add(temp);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return _gameFormats;
        }

        /// <summary>
        /// Retrieve a list of GamePublishers
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GamePublisher> GetAllGamePublishers(out string error_message)
        {

            error_message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
                {
                    string query = "SELECT id, name FROM game_publisher UNION SELECT 0, '' ORDER BY name";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            Id = reader.GetOrdinal("id"),
                            PublisherName = reader.GetOrdinal("name")
                        };

                        while (reader.Read())
                        {
                            bool ro = reader.HasRows;
                            GamePublisher temp = new GamePublisher
                            {
                                Id = reader.GetInt32(ordinals.Id),
                                PublisherName = reader.GetString(ordinals.PublisherName)
                            };

                            _gamePublishers.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return _gamePublishers;
        }

        /// <summary>
        /// Dispose of unmanaged objects
        /// Required because IRepository implements IDisposable
        /// </summary>
        public void Dispose()
        {    
            //
            // Commented out this code because it was clearing out the collections in the BLL
            // To be reviewed for proper implementation of this method
            //
            //_gameFormats.RemoveAll(item => item.Id > -1);
            //_gamePublishers.RemoveAll(item => item.Id > -1);
            //_games.RemoveAll(item => item.Id > -1);
        }
    }
}
