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
using Game_DomainLayer;
using Game_BusinessLogicLayer;

namespace Game_PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //, INotifyPropertyChanged
    {
       private ObservableCollection<Game> _filteredGameCollection;
       private IEnumerable<Game> _originalGameList { get; set; }
       private ObservableCollection<Game> _gameCollection { get; set; }
       private IEnumerable<GameFormat> _gameFormatList { get; set; }
       private IEnumerable<GamePublisher> _gamePublisherList { get; set; }

        static GameBLL _gamesBLL;

        public MainWindow(GameBLL gameBLL)
        {
            _gamesBLL = gameBLL;
            InitializeComponent();
            //gameViewModelObject = new GameViewer.ViewModel.GameViewModel();
            this.DataContext = gameBLL;//gameViewModelObject;
            cmb_Rating.ItemsSource = GameRatingList.RatingList;
        }

        private void GameViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            _originalGameList = _gamesBLL.GetAllGames(out string error_message);
        }

        private void GamePublisherControl_Loaded(object sender, RoutedEventArgs e)
        {
            _gamePublisherList = _gamesBLL.GetAllGamePublishers(out string error_message);
        }

        private void GameFormatControl_Loaded(object sender, RoutedEventArgs e)
        {
            _gameFormatList = _gamesBLL.GetAllGameFormats(out string error_message);
        }

        private void FilterGrid()
        {
            //MessageBox.Show(dtpick_BeginDate.SelectedDate.Value.Date.ToShortDateString());
            // if I switch to N-Tier, I'll need to use StringBuilder with lambda expressions on ints to generate query string
            //int? publisherId = (Int32?)cmb_Publisher.SelectedValue;
            //int? formatId = (Int32?)cmb_GameFormat.SelectedValue;
            //int? rating =  (cmb_Rating.SelectedValue.Equals(null) || cmb_Rating.SelectedValue.Equals("") ? null : (Int32?)cmb_Rating.SelectedValue);
            //string beginDate = (dtpick_BeginDate.SelectedDate == null || dtpick_BeginDate.SelectedDate.Value.ToString().Trim() == "" ? "" : dtpick_BeginDate.SelectedDate.Value.Date.ToShortDateString());
            //string endDate = (dtpick_EndDate.SelectedDate == null || dtpick_EndDate.SelectedDate.ToString().Trim() == "" ? "" : dtpick_EndDate.SelectedDate.Value.Date.ToShortDateString()); 
            string publisherId = (cmb_Publisher.SelectedValue == null ? "" : cmb_Publisher.SelectedValue.ToString().Trim());
            string formatId = (cmb_GameFormat.SelectedValue == null ? "" : cmb_GameFormat.SelectedValue.ToString().Trim());
            string rating = (cmb_Rating.SelectedValue == null ? "" : cmb_Rating.SelectedValue.ToString().Trim());
            string beginDate = (dtpick_BeginDate.SelectedDate == null ? "" : dtpick_BeginDate.SelectedDate.Value.ToString().Trim());
            string endDate = (dtpick_EndDate.SelectedDate == null ? "" : dtpick_EndDate.SelectedDate.ToString().Trim());
            string gameName = (txt_GameName.Text == null ? "" : txt_GameName.Text.ToString().Trim());

            _gamesBLL.FilterGameCollection(formatId, publisherId, rating, beginDate, endDate, gameName);

            //ObservableCollection<Game> filteredGameCollection = gameViewModelObject.GameCollection.Where(x => x.ReleaseDate.ToString().Equals(releaseDate));

            //dataGridView_Games.ItemsSource = filteredGameCollection;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dtpick_BeginDate.SelectedDate = null;
            dtpick_EndDate.SelectedDate = null;
        }

        private void dataGridView_Games_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
