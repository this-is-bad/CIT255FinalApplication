using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_DomainLayer
{
    /// <summary>
    /// Model for a GamePublisher object
    /// </summary>
    public class GamePublisher
    {

        public int Id { get; set; }
        public string PublisherName { get; set; }

        public GamePublisher() { }
    }
}
