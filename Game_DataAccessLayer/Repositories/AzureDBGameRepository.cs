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

        public AzureDBGameRepository()
        {
            _games = new List<Game>();
        }

        public IEnumerable<Game> GetAll(out string error_message)
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

                        while (reader.Read() == true)
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

        private DataSet GetGameCollection(out string error_message)
        {

            DataSet dataSet = new DataSet();
            error_message = "";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(AzureDbDataSettings.connectionString))
                {
                    
                    string s = "SELECT * FROM GameView ORDER BY name, publisher_name, format";
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(
                        s, connection);
                    adapter.Fill(dataSet);

                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }
            
            return dataSet;
        }

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

        public void Dispose()
        {
            _games = null;
        }
    }
}
