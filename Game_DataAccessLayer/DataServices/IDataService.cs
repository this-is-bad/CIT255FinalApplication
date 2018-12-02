using Game_DomainLayer;
using System.Collections.Generic;

namespace Game_DataAccessLayer
{
    public interface IDataService
    {
        IEnumerable<Game> ReadAll(out string statusCode);
        void WriteAll(IEnumerable<Game> games, out string statusCode);
    }
}
