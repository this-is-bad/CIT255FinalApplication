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
    public partial class MainWindow : Window
    {
        GameBLL gameBLL;
        IGameRepository gameRepository;

        public MainWindow()
        {
            gameRepository = new AzureDBGameRepository();

            gameBLL = new GameBLL(gameRepository);

            InitializeComponent();
            this.DataContext = gameBLL;
            cmb_Rating.ItemsSource = GameRatingList.RatingList;
        }

        /// <summary>
        /// Load Games when control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            gameBLL.GetAllGames(out string error_message);
        }

        /// <summary>
        /// Load GamePublishers when control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamePublisherControl_Loaded(object sender, RoutedEventArgs e)
        {
            gameBLL.GetAllGamePublishers(out string error_message);
        }

        /// <summary>
        /// Load GameFormats when control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameFormatControl_Loaded(object sender, RoutedEventArgs e)
        {
            gameBLL.GetAllGameFormats(out string error_message);
        }

        /// <summary>
        /// Filter the datagrid
        /// </summary>
        private void FilterGrid()
        {
            string publisherId = (cmb_Publisher.SelectedValue == null ? "" : cmb_Publisher.SelectedValue.ToString().Trim());
            string formatId = (cmb_GameFormat.SelectedValue == null ? "" : cmb_GameFormat.SelectedValue.ToString().Trim());
            string rating = (cmb_Rating.SelectedValue == null ? "" : cmb_Rating.SelectedValue.ToString().Trim());
            string beginDate = (dtpick_BeginDate.SelectedDate == null ? "" : dtpick_BeginDate.SelectedDate.Value.ToString().Trim());
            string endDate = (dtpick_EndDate.SelectedDate == null ? "" : dtpick_EndDate.SelectedDate.ToString().Trim());
            string gameName = (txt_GameName.Text == null ? "" : txt_GameName.Text.ToString().Trim());

            gameBLL.FilterGameCollection(formatId, publisherId, rating, beginDate, endDate, gameName);
        }


        private void txt_GameName_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterGrid();
        }

        private void cmb_Publisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterGrid();
        }

        private void cmb_GameFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterGrid();
        }

        private void cmb_Rating_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterGrid();
        }

        private void dtpick_BeginDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterGrid();
        }

        private void dtpick_EndDate_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            FilterGrid();
        }

        /// <summary>
        /// Clear datepicker fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearDates_Click(object sender, RoutedEventArgs e)
        {
            dtpick_BeginDate.SelectedDate = null;
            dtpick_EndDate.SelectedDate = null;
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Detail_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridView_Games.SelectedIndex > -1)
            {
                Game game = dataGridView_Games.SelectedItem as Game;

                if (game.Id > 0)
                {
                    DetailWindow detailWindow = new DetailWindow(gameBLL, game.Id);
                    detailWindow.ShowDialog();
                }
            }
            else
            {
                txtblk_Message.Text = "Please select a game from the grid";
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detailWindow = new DetailWindow(gameBLL);
            detailWindow.ShowDialog();
        }
    }

}