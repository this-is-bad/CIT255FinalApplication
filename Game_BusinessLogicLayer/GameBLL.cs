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
        public IEnumerable<Game> OriginalGameList { get; set; }
        public ObservableCollection<Game> GameCollection { get; set; }
        public IEnumerable<GameFormat> GameFormatList { get; set; }
        public IEnumerable<GamePublisher> GamePublisherList { get; set; }

        public List<string> RatingList = new List<string> { "", "1", "2", "3", "4", "5" };

        public GameBLL()
        {
            GameFormatList = new List<GameFormat>();
            AzureDbDataService azdb = new AzureDbDataService();
            GamePublisherList = azdb.ReadAllGamePublishers(out string x);
            GameCollection = new ObservableCollection<Game>();
        }
    }
}
