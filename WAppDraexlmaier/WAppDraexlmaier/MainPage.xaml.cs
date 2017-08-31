using System.Collections.ObjectModel;
using WAppDraexlmaier.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WAppDraexlmaier
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<NewsItem> NewsItems;

        public MainPage()
        {
            this.InitializeComponent();
            NewsItems = new ObservableCollection<NewsItem>(); 
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Financial.IsSelected = true;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Financial.IsSelected) {
                NewsManager.getNews("Financial", NewsItems);
                TitleTextBox.Text = "Financial";        
            }
            else if (Food.IsSelected) {
                NewsManager.getNews("Food", NewsItems);
                TitleTextBox.Text = "Food";
            }
        }
    }
}
