using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Data.Common;
using Game_DataAccessLayer;
using Game_DomainLayer;
using Game_BusinessLogicLayer;

namespace Game_PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window 
    {
        GameBLL _gameBLL;
        Game _game;

        public DetailWindow(GameBLL gameBLL)
        {
            _gameBLL = gameBLL;
            _game = new Game();
            InitializeComponent();
            this.DataContext = gameBLL;
        }

        public DetailWindow(GameBLL gameBLL, int id)
        {
            _gameBLL = gameBLL;
            _game = _gameBLL.GetById(id,out string error_message);
            InitializeComponent();
            this.DataContext = _gameBLL;
        }


        /// <summary>
        /// Load GamePublishers when control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePublisherControl_Loaded(object sender, RoutedEventArgs e)
        {
            _gameBLL.GetAllGamePublishers(out string error_message);
        }

        /// <summary>
        /// Load GameFormats when control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameFormatControl_Loaded(object sender, RoutedEventArgs e)
        {
            _gameBLL.GetAllGameFormats(out string error_message);
        }



        

        /// <summary>
        /// Clear datepicker fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dtpick_ReleaseDate.SelectedDate = null;
        }

    }

}
