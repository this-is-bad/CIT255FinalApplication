using Game_DataAccessLayer;
using Game_DomainLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_BusinessLogicLayer
{
    /// <summary>
    /// GameViewer business logic class
    /// </summary>
    public class GameBLL
    {
        private IEnumerable<Game> _originalGameList { get; set; }
        public ObservableCollection<Game> GameCollection { get; set; }
        public ObservableCollection<GameFormat> GameFormatCollection { get; set; }
        public ObservableCollection<GamePublisher> GamePublisherCollection { get; set; }
        IGameRepository _gameRepository;

        public GameBLL(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
            GameFormatCollection = new ObservableCollection<GameFormat>();
            GamePublisherCollection = new ObservableCollection<GamePublisher>();
            GameCollection = new ObservableCollection<Game>();
        }
        
        /// <summary>
        /// Load an ObservableCollection with Games
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public void GetAllGames(out string error_message)
        {
            _originalGameList = null;
            GameCollection.Clear();
            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    _originalGameList = _gameRepository.GetAllGames(out error_message);
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            foreach (Game game in _originalGameList)
            {
                GameCollection.Add(game);
            }
        }

        /// <summary>
        /// Get an IEnumerable of game formats
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public void GetAllGameFormats(out string error_message)
        {
            IEnumerable<GameFormat> gameFormats = new List<GameFormat>();
                   
            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    gameFormats = _gameRepository.GetAllGameFormats(out error_message);
                }
                GameFormatCollection.Clear();
                foreach (GameFormat format in gameFormats)
                {
                    GameFormatCollection.Add(format);
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

        }

        /// <summary>
        /// Get an IEnumerable of game publishers
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public void GetAllGamePublishers(out string error_message)
        {
            IEnumerable<GamePublisher> gamePublishers = new List<GamePublisher>();
            
            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    gamePublishers = _gameRepository.GetAllGamePublishers(out error_message);
                }
                GamePublisherCollection.Clear();
                foreach (GamePublisher publisher in gamePublishers)
                {
                    GamePublisherCollection.Add(publisher);
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }
        }

        /// <summary>
        /// Get a specific game by its ID
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
                using (_gameRepository)
                {
                    game = _gameRepository.GetById(id, out error_message);
                }
            }
            catch (Exception ex)
            {
                error_message = ex.Message;
            }

            return game;
        }

        /// <summary>
        /// Add a game to the persistence object
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public string AddGame(Game game)
        {
            string result = "";
            result = _gameRepository.Insert(game);

            GetAllGames(out string error_message);

            return result;
        }

        /// <summary>
        /// Modify an existing game in the persistence
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public string UpdateGame(Game game)
        {
            string result = "";
            result = _gameRepository.Update(game);

            GetAllGames(out string error_message);

            return result;
        }

        /// <summary>
        /// Delete a game from the persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteGame(int id)
        {
            string result = "";
            result = _gameRepository.Delete(id);

            GetAllGames(out string error_message);

            return result;

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
            IEnumerable<Game> outputGameList =
                            (from g in _originalGameList
                             where g.FormatId == (formatId == 0 ? g.FormatId : formatId)
                             && g.PublisherId == (publisherId == 0 ? g.PublisherId : publisherId)
                             && g.Rating == (rating == 0 ? g.Rating : rating)
                             && g.GameName.ToLower().Contains(gameName.ToLower())
                             && (beginDate == null || beginDate == "" ? 1 == 1 : DateTime.Parse(g.ReleaseDate) >= DateTime.Parse(beginDate == "" ? g.ReleaseDate : beginDate))
                             && DateTime.Parse(g.ReleaseDate) <= DateTime.Parse(endDate == "" ? g.ReleaseDate : endDate)
                             orderby g.GameName, g.PublisherName, g.FormatName
                             select g);

            GameCollection.Clear();

            foreach (Game game in outputGameList)
            {
                GameCollection.Add(game);
            }
        }

    }
}