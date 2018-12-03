using Game_DataAccessLayer;
using Game_DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_BusinessLogicLayer
{
    public class GameBLL
    {
        public const string connectionString = "Server=tcp:cit255.database.windows.net,1433;Initial Catalog=Gamiverse;Persist Security Info=False;User ID=user;Password=Password1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public IEnumerable<Game> OriginalGameList { get; set; }
        public ObservableCollection<Game> GameCollection { get; set; }
        public ObservableCollection<GameFormat> GameFormatCollection { get; set; }
        public ObservableCollection<GamePublisher> GamePublisherCollection { get; set; }
        public List<string> RatingList = new List<string> { "", "1", "2", "3", "4", "5" };

        public GameViewModel()
        {
            GameFormatCollection = new ObservableCollection<GameFormat>();
            GamePublisherCollection = new ObservableCollection<GamePublisher>();
            GameCollection = new ObservableCollection<Game>();
            GameCollection = new ObservableCollection<Game>();
        }

        /// <summary>
        /// Loads the OriginalGameList IEnumerable and the GameCollection ObservableCollection
        /// </summary>
        public void LoadGames()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    List<Game> gameList = new List<Game>();
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
                                ReleaseDate = ConvertDateTimeToString(reader.GetDateTime(ordinals.ReleaseDate)),
                                Discontinued = reader.GetBoolean(ordinals.Discontinued),
                                Rating = reader.GetInt32(ordinals.Rating),
                                Comment = reader.GetString(ordinals.Comment),
                            };

                            gameList.Add(temp);
                            GameCollection.Add(temp);
                        }
                        OriginalGameList = gameList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Loads the GameFormatCollection ObservableCollection
        /// </summary>
        public void LoadGameFormats()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                            GameFormatCollection.Add(temp);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Filters the game collection
        /// </summary>
        /// <param name="formatIdString"></param>
        /// <param name="publisherIdString"></param>
        /// <param name="ratingString"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="gameName"></param>
        public void FilterGameCollection(string formatIdString, string publisherIdString, string ratingString, string beginDate, string endDate, string gameName)
        {

            // process the inputs
            int.TryParse(formatIdString, out int formatId);
            int.TryParse(publisherIdString, out int publisherId);
            int.TryParse(ratingString, out int rating);

            if (DateTime.TryParse(beginDate, out DateTime beginDT))
            {
                beginDate = beginDT.Date.ToShortDateString();
            }

            if (DateTime.TryParse(endDate, out DateTime endDT))
            {
                endDate = endDT.Date.ToShortDateString();
            }

            //GameCollection = (ObservableCollection<Game>)
            IEnumerable<Game> gameList =
                            (from g in OriginalGameList
                             where g.FormatId == (formatId == 0 ? g.FormatId : formatId)
                             && g.PublisherId == (publisherId == 0 ? g.PublisherId : publisherId)
                             && g.Rating == (rating == 0 ? g.Rating : rating)
                             && g.GameName.ToLower().Contains(gameName.ToLower())
                             && DateTime.Parse(g.ReleaseDate) >= DateTime.Parse(beginDate == "" ? g.ReleaseDate : beginDate)
                             && DateTime.Parse(g.ReleaseDate) <= DateTime.Parse(endDate == "" ? g.ReleaseDate : endDate)
                             orderby g.GameName, g.PublisherName, g.FormatName
                             select g);

            GameCollection.Clear();

            foreach (Game g in gameList)
            {
                GameCollection.Add(g);
            }
        }

        /// <summary>
        /// Loads the GamePublisherCollection ObservableCollection
        /// </summary>
        public void LoadGamePublishers()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                            GamePublisherCollection.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
