using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_DomainLayer;

namespace Game_DataAccessLayer
{
    /// <summary>
    /// Game repository interface
    /// </summary>
    public interface IGameRepository : IDisposable
    {
        IEnumerable<Game> GetAllGames(out string error_message);
        IEnumerable<GameFormat> GetAllGameFormats(out string error_message);
        IEnumerable<GamePublisher> GetAllGamePublishers(out string error_message);
        Game GetById(int id, out string error_message);
        string Insert(Game game);
        string Update(Game game);
        string Delete(int id);
    }
}
