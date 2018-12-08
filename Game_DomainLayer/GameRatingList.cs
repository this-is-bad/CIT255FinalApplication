using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_DomainLayer
{
    /// <summary>
    /// Model for a GameRatingList object
    /// </summary>
    public static class GameRatingList
    { 
        public static List<string> RatingList = new List<string> { "", "1", "2", "3", "4", "5" };

        static GameRatingList () { }        

    }
}
