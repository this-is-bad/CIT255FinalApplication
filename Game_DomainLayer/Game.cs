using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_DomainLayer
{
    /// <summary>
    /// Model for a Game object
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public int FormatId { get; set; }
        public string FormatName { get; set; }
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public int MinimumPlayerCount { get; set; }
        public int MaximumPlayerCount { get; set; }
        public string ReleaseDate { get; set; }
        public bool Discontinued { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public Game() { }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Format a string as a date value
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>date-formatted string</returns>
        public static string ConvertDateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy");
        }
    }
}
