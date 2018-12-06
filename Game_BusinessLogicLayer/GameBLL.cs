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
    public class GameBLL
    {

        IGameRepository _gameRepository;
        BooleanConverter _booleanConverter;
       
        public GameBLL(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        /// <summary>
        /// Get an IEnumerable of games
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<Game> GetAllGames(out string error_message)
        {
            List<Game> games = null;

            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    games = _gameRepository.GetAllGames(out error_message) as List<Game>;
                    
                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return games;
        }

        /// <summary>
        /// Get an IEnumerable of game formats
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GameFormat> GetAllGameFormats(out string error_message)
        {
            List<GameFormat> gameFormats = null;

            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    gameFormats = _gameRepository.GetAllGameFormats(out error_message) as List<GameFormat>;

                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return gameFormats;
        }

        /// <summary>
        /// Get an IEnumerable of game publishers
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public IEnumerable<GamePublisher> GetAllGamePublishers(out string error_message)
        {
            List<GamePublisher> gamePublishers = null;

            error_message = "";

            try
            {
                using (_gameRepository)
                {
                    gamePublishers = _gameRepository.GetAllGamePublishers(out error_message) as List<GamePublisher>;

                }
            }
            catch (Exception ex)
            {
                error_message = ex.ToString();
            }

            return gamePublishers;
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
            return _gameRepository.Insert(game);
        }

        /// <summary>
        /// Modify an existing game in the persistence
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public string UpdateGame(Game game)
        {
            return _gameRepository.Update(game);
        }

        /// <summary>
        /// Delete a game from the persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeleteGame(int id)
        {
            return _gameRepository.Delete(id);
        }      
    }
}
