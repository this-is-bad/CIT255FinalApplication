using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_DomainLayer
{
    /// <summary>
    /// Model for a GameFormat object
    /// </summary>
    public class GameFormat
    {
        public int Id { get; set; }
        public string FormatName { get; set; }

        public GameFormat() { }
    }
}
