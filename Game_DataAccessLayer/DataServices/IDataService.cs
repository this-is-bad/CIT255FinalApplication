using Game_DomainLayer;
using System.Collections.Generic;

namespace Game_DataAccessLayer
{
    public interface IDataService
    {
        IEnumerable<Game> ReadAllGames(out string statusCode);
        IEnumerable<GameFormat> ReadAllGameFormats(out string statusCode);
        IEnumerable<GamePublisher> ReadAllGamePublishers(out string statusCode);
        void WriteAllGames(IEnumerable<Game> games, out string statusCode);
    }
}
