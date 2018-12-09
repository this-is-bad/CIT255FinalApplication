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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Game_PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window , INotifyPropertyChanged
    {

        private static readonly Regex _regexInteger = new Regex("[0-9]{1,10}");

        private DateTime? _date;

        GameBLL _gameBLL;
        Game _game;

        /// <summary>
        /// Solution to enforcing MM/dd/yyyy format on the datepicker was found here
        /// https://blog.magnusmontin.net/2014/09/28/binding-datepicker-wpf/
        /// </summary>
        public DateTime? Date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged(); }
        }        

        public DetailWindow(GameBLL gameBLL)
        {
            _gameBLL = gameBLL;
            _game = new Game();
            InitializeComponent();
            this.DataContext = _gameBLL;
            cmb_Rating.ItemsSource = GameRatingList.RatingList;
        }

        public DetailWindow(GameBLL gameBLL, int id)
        {
            _gameBLL = gameBLL;
            _game = _gameBLL.GetById(id,out string error_message);
            InitializeComponent();
            this.DataContext = _gameBLL;
            cmb_Rating.ItemsSource = GameRatingList.RatingList;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Determines whether the first value is greater than or equal to the second value
        /// </summary>
        /// <param name="sOne"></param>
        /// <param name="sTwo"></param>
        /// <returns></returns>
        public static bool IsGreaterThanOrEqualTo(string sOne, string sTwo)
        {
            return ((sOne == "" ? 0 : int.Parse(sOne)) >= (sTwo == "" ? 0 : int.Parse(sTwo)));
        }



        /// <summary>
        /// Check a value for non-numeric characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsNumeric(string text)
        {
            return _regexInteger.IsMatch(text);
        }

        /// <summary>
        /// Clear the input fields on the form
        /// </summary>
        private void ClearForm()
        {
            chk_Discontinued.IsChecked = false;
            txt_Comment.Text = "";
            txt_GameName.Text = "";
            txt_MaximumPlayerCount.Text = "";
            txt_MinimumPlayerCount.Text = "";
            cmb_GameFormat.SelectedIndex = -1;
            cmb_Publisher.SelectedIndex = -1;
            cmb_Rating.SelectedIndex = -1;
            dtpick_ReleaseDate.SelectedDate = null;
        }

        /// <summary>
        /// Clear or reset fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (_game.Id == 0)
            {
                ClearForm();
            }
            else
            {
                DisplayGameDetail();
            }
        }

        public void DisplayGameDetail()
        {
            chk_Discontinued.IsChecked = _game.Discontinued;
            txt_Comment.Text = _game.Comment;
            txt_GameName.Text = _game.GameName;
            txt_MaximumPlayerCount.Text = _game.MaximumPlayerCount.ToString();
            txt_MinimumPlayerCount.Text = _game.MinimumPlayerCount.ToString();
            cmb_GameFormat.SelectedValue = _game.FormatId;
            cmb_Publisher.SelectedValue = _game.PublisherId;
            cmb_Rating.SelectedValue = _game.Rating.ToString();//SelectedIndex = GameRatingList.RatingList.IndexOf(_game.Rating.ToString());
            dtpick_ReleaseDate.SelectedDate = Convert.ToDateTime(_game.ReleaseDate);
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {               
            string result = _gameBLL.DeleteGame(_game.Id);

            if (result == "")
            {
                ClearForm();
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(txt_MinimumPlayerCount.Text) && IsNumeric(txt_MaximumPlayerCount.Text) && IsNumeric(cmb_Rating.SelectedValue.ToString()) && IsNumeric(cmb_Publisher.SelectedValue.ToString()) && IsNumeric(cmb_GameFormat.SelectedValue.ToString()))
            {
                Game updateGame = new Game()
                {
                    Comment = txt_Comment.Text,
                    Discontinued = chk_Discontinued.IsChecked ?? false,
                    FormatId = int.Parse(cmb_GameFormat.SelectedValue.ToString()),
                    GameName = txt_GameName.Text,
                    Id = _game.Id,
                    MaximumPlayerCount = int.Parse(txt_MaximumPlayerCount.Text),
                    MinimumPlayerCount = int.Parse(txt_MinimumPlayerCount.Text),
                    PublisherId = int.Parse(cmb_Publisher.SelectedValue.ToString()),
                    Rating = int.Parse(cmb_Rating.SelectedValue.ToString()),
                    ReleaseDate = dtpick_ReleaseDate.SelectedDate.ToString()
                };

                string result = _gameBLL.UpdateGame(updateGame);

                if (result == "")
                {
                    _game = updateGame;
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
        }

        /// <summary>
        /// Show or hide buttons that manage CRUD operations, based on
        /// </summary>
        private void Show_Hide_Buttons()
        {
            if (_game.Id > 0)
            {
                btn_Add.Visibility = Visibility.Hidden;
            }
            else
            {
                btn_Update.Visibility = Visibility.Hidden;
                btn_Delete.Visibility = Visibility.Hidden;
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumeric(txt_MinimumPlayerCount.Text) && IsNumeric(txt_MaximumPlayerCount.Text) && IsNumeric(cmb_Rating.SelectedValue.ToString()) 
                && IsNumeric(cmb_Publisher.SelectedValue.ToString()) && IsNumeric(cmb_GameFormat.SelectedValue.ToString())
                && txt_GameName.Text != "" && txt_Comment.Text != "" && dtpick_ReleaseDate.SelectedDate.ToString() != "")
            {
                Int32.TryParse(cmb_Publisher.SelectedValue.ToString(), out int publisherId);
                Int32.TryParse(cmb_GameFormat.SelectedValue.ToString(), out int formatId);
                Int32.TryParse(cmb_Rating.SelectedValue.ToString(), out int rating);
                Int32.TryParse(txt_MinimumPlayerCount.Text, out int minimumPlayerCount);
                Int32.TryParse(txt_MaximumPlayerCount.Text, out int maximumPlayerCount);


                _game.GameName = txt_GameName.Text;
                _game.PublisherId = publisherId;
                _game.FormatId = formatId;
                _game.Rating = rating;
                _game.MaximumPlayerCount = maximumPlayerCount;
                _game.MinimumPlayerCount = minimumPlayerCount;
                _game.ReleaseDate = Game.ConvertDateTimeToString(dtpick_ReleaseDate.SelectedDate ?? DateTime.MinValue);
                _game.Discontinued = (chk_Discontinued.IsChecked ?? false);
                _game.Comment = txt_Comment.Text;

               

                string result = _gameBLL.AddGame(_game);

                if (result == "")
                {
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ValidateNumeric(object sender, TextCompositionEventArgs e)
        {
           e.Handled = !IsNumeric(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_game.Id > 0)
            {
                DisplayGameDetail();
            }

            Show_Hide_Buttons();
        }

        /// <summary>
        /// Ensure that maximum number of players is greater than or equal to minimum number of players
        /// </summary>
        private void ComparePlayerNumbers(object sender, RoutedEventArgs e)
        {
            if (!IsGreaterThanOrEqualTo(txt_MaximumPlayerCount.Text, txt_MinimumPlayerCount.Text))
            {
                txt_MaximumPlayerCount.Text = "";
            }
        }
    }

}
