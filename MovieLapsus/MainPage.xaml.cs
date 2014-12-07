using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MovieLapsus
{
    internal struct ActorSuggestInfo
    {
        public ActorSuggestInfo(string _name, int _id)
        {
            name = _name;
            id = _id;
        }

        public override string ToString()
        {
            return name;
        }

        public string name;
        public int id;
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TMDB.TMDBQueries dbQueries = null;
        private TMDB.TMDBAPI dbApi = null;
        private Windows.ApplicationModel.Resources.ResourceLoader resLoader = null;
        private string searchParameter = "";

        public bool SearchForMovie
        {
            get
            {
                return searchParameter == "movie";
            }
        }

        public bool SearchForActor
        {
            get
            {
                return searchParameter == "actor";
            }
        }

        public Windows.ApplicationModel.Resources.ResourceLoader ResLoader
        {
            get
            {
                if (resLoader == null)
                {
                    resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                }

                return resLoader;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            dbQueries = new TMDB.TMDBQueries();
            dbApi = new TMDB.TMDBAPI(dbQueries);

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            searchParameter = e.Parameter.ToString();
            if (SearchForActor)
            {
                searchDescTB.Text = ResLoader.GetString("SearchDesc4Actor");
            }
            if (SearchForMovie)
            {
                searchDescTB.Text = ResLoader.GetString("SearchDesc4Movie");
            }

            if (autoSuggest1.Tag == null)
            {
                SetEmptySuggestBox(autoSuggest1);
            }
            if (autoSuggest2.Tag == null)
            {
                SetEmptySuggestBox(autoSuggest2);
            }

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            base.OnNavigatedTo(e);
        }

        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            if (autoSuggest1.Tag == null || autoSuggest2.Tag == null)
            {
                commonMovies.Text = "Unknown actor(s) selected!";
                return;
            }

            if (SearchForMovie)
            {
                string id1 = autoSuggest1.Tag.ToString();
                string id2 = autoSuggest2.Tag.ToString();

                var actorInfo1 = await dbApi.GetActorMoviesFromID(id1);
                var actorInfo2 = await dbApi.GetActorMoviesFromID(id2);

                var commonList = from mov1 in actorInfo1.cast
                                 join mov2 in actorInfo2.cast on mov1.id equals mov2.id
                                 select mov1;

                var c = commonList.Count();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Found " + c.ToString() + " movies:");
                foreach (MovieInfoByID_Cast ai in commonList)
                {
                    System.Diagnostics.Debug.WriteLine(ai.original_title);
                    sb.AppendLine("  - " + ai.original_title);
                }
                commonMovies.Text = sb.ToString();

                foreach (var movie in commonList)
                {
                    movie.poster_path = dbApi.MakeMoviePosterPath(movie.poster_path);
                }
                Frame.Navigate(typeof(MovieListPage), commonList);
            }
            else
            {
                string id1 = autoSuggest1.Tag.ToString();
                string id2 = autoSuggest2.Tag.ToString();

                var list1 = await dbApi.GetMovieCreditsFromID(id1);
                var list2 = await dbApi.GetMovieCreditsFromID(id2);

                var commonList = from mov1 in list1.cast
                                 join mov2 in list2.cast on mov1.id equals mov2.id
                                 select mov1;

                var c = commonList.Count();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Found " + c.ToString() + " actors:");

                foreach (var ai in commonList)
                {
                    System.Diagnostics.Debug.WriteLine(ai.name);
                    sb.AppendLine("  - " + ai.name);
                }
                commonMovies.Text = sb.ToString();

                //foreach (var movie in commonList)
                //{
                //    movie.poster_path = dbApi.MakeMoviePosterPath(movie.poster_path);
                //}
                //Frame.Navigate(typeof(MovieListPage), commonList);
            }
        }

        private async void OnSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            if (SearchForActor)
            {
                await AutoCompleteMovie(sender);
            }
            else
            {
                await AutoCompleteActor(sender);
            }
        }

        private async System.Threading.Tasks.Task AutoCompleteActor(AutoSuggestBox sender)
        {
            sender.Tag = null;
            if (sender.Text.Length <= 2)
            {
                sender.ItemsSource = new string[] { };
                return;
            }

            var searchInfo = await dbApi.SearchForActor(sender.Text);
            if (searchInfo.results.Count == 0)
                return;

            var suggestions = (from result in searchInfo.results
                                   select result).Take(7);

            sender.ItemsSource = suggestions;
        }

        private async System.Threading.Tasks.Task AutoCompleteMovie(AutoSuggestBox sender)
        {
            sender.Tag = null;
            if (sender.Text.Length <= 3)
            {
                sender.ItemsSource = new string[] { };
                return;
            }

            var searchInfo = await dbApi.SearchForMovie(sender.Text, true);
            if (searchInfo.results.Count == 0)
                return;

            var suggestions = (from result in searchInfo.results
                                   select result).Take(7);

            sender.ItemsSource = suggestions;
        }

        private async void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            await dbApi.GetConfiguration();

            if (SearchForMovie)
            {
                var selection = args.SelectedItem as SearchActor_ActorInfo;

                if (selection != null)
                {
                    sender.Tag = selection.id.ToString();
                }

                string path = await dbApi.GetActorImageFromID(selection.id.ToString());

                BitmapImage src = new BitmapImage(new Uri(path));
                actorImg.Source = src;
            }
            else
            {
                var selection = args.SelectedItem as SearchMovie_Result;
                

                if (selection != null)
                {
                    sender.Tag = selection.id.ToString();
                }

                var desc = await dbApi.GetMovieDescriptionFromID(selection.id.ToString());

                BitmapImage src = new BitmapImage(new Uri(dbApi.MakeMoviePosterPath(desc.poster_path)));
                actorImg.Source = src;
            }

            this.searchBtn.Focus(FocusState.Programmatic);
            return;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var suggestBox = sender as AutoSuggestBox;

            if (suggestBox.FontStyle == Windows.UI.Text.FontStyle.Italic)
            {
                suggestBox.FontStyle = Windows.UI.Text.FontStyle.Normal;
                suggestBox.FontWeight = Windows.UI.Text.FontWeights.Normal;

                suggestBox.Text = "";
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var suggestBox = sender as AutoSuggestBox;

            if (suggestBox.Text == null || suggestBox.Text.Length == 0)
            {
                SetEmptySuggestBox(suggestBox);
            }
        }

        private void SetEmptySuggestBox(AutoSuggestBox suggestBox)
        {
            string str = ResLoader.GetString("empty_actor_name");
            if (SearchForActor)
            {
                str = ResLoader.GetString("empty_movie_name");
            }

            suggestBox.Tag = null;
            suggestBox.Text = str;
            suggestBox.FontStyle = Windows.UI.Text.FontStyle.Italic;
            suggestBox.FontWeight = Windows.UI.Text.FontWeights.Light;
        }

    }
}
