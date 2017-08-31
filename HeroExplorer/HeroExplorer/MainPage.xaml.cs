using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> MarvelCharacters { get; set; }
        public ObservableCollection<ComicBook> MarvelComics { get; set; }


        public MainPage()
        {
            this.InitializeComponent();
            MarvelCharacters = new ObservableCollection<Character>();
            MarvelComics = new ObservableCollection<ComicBook>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var storageFile =
             await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
               new Uri("ms-appx:///VoiceCommandDiccionary.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            refreshCharacterList();
        }

        public async void refreshCharacterList() {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            MarvelCharacters.Clear();
            while (MarvelCharacters.Count < 50)
            {
                Task t = MarvelFacade.PopularMarvelCharactersAsync(MarvelCharacters);
                await t;
            }

            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;

        }

        private async void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedCharacter = (Character)e.ClickedItem;
            DetailNameTextBlock.Text = selectedCharacter.name;
            DetailDescriptionTextBlock.Text = selectedCharacter.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            DetailImage.Source = largeImage;

            MarvelComics.Clear();
            ComicDescriptionTextBlock.Text = "";
            ComicNameTextBlock.Text = "";
            ComicImage.Source = null;

            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            await MarvelFacade.PopularMarvelComicsAsync(MarvelComics, selectedCharacter.id);

            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;

        }

        private void ListComicsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedComic = (ComicBook)e.ClickedItem;
            ComicNameTextBlock.Text = selectedComic.title;
            if(selectedComic.description!= null)
                ComicDescriptionTextBlock.Text = selectedComic.description;


            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            ComicImage.Source = largeImage; 
        }
    }
}
