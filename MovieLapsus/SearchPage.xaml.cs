using MovieLapsus.Common;
using System;
using System.Linq;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class SearchPage : Page
    {
        private readonly NavigationHelper navigationHelper;

        private TMDB.TMDBQueries m_dbQueries = null;
        private TMDB.TMDBAPI m_dbApi = null;
        private Windows.ApplicationModel.Resources.ResourceLoader m_resLoader = null;
        private string m_searchParameter = "";
        private string m_oldSearchParameter = "";

        public bool SearchForMovie
        {
            get
            {
                return m_searchParameter == "movie";
            }
        }

        public bool SearchForActor
        {
            get
            {
                return m_searchParameter == "actor";
            }
        }

        public Windows.ApplicationModel.Resources.ResourceLoader ResLoader
        {
            get
            {
                if (m_resLoader == null)
                {
                    m_resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                }

                return m_resLoader;
            }
        }

        public SearchPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            m_dbQueries = new TMDB.TMDBQueries();
            m_dbApi = new TMDB.TMDBAPI(m_dbQueries);

            //this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            m_searchParameter = e.NavigationParameter.ToString();

            if (e.PageState != null)
            {
                autoSuggest1.Tag = e.PageState["t1"];
                SetSuggestBoxText(autoSuggest1, e.PageState["n1"] as string);
                autoSuggest2.Tag = e.PageState["t2"];
                SetSuggestBoxText(autoSuggest2, e.PageState["n2"] as string);
                actorImg.Source = e.PageState["url"] as BitmapImage;
            }

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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
            e.PageState["t1"] = autoSuggest1.Tag;
            e.PageState["n1"] = autoSuggest1.Text;
            e.PageState["t2"] = autoSuggest2.Tag;
            e.PageState["n2"] = autoSuggest2.Text;
            e.PageState["url"] = actorImg.Source;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            if (autoSuggest1.Tag == null && autoSuggest2.Tag == null)
            {
                commonMovies.Text = "Unknown actor(s) selected!";
                return;
            }

            var calc = new CommonCalculator(m_dbApi);
            string id1 = (autoSuggest1.Tag == null)? null : autoSuggest1.Tag.ToString();
            string id2 = (autoSuggest2.Tag == null) ? null : autoSuggest2.Tag.ToString();

            if (SearchForMovie)
            {
                var list = await calc.CalculateCommonMovies(id1, id2);

                Frame.Navigate(typeof(ResultsListPage), list);
            }
            else
            {
                var list = await calc.CalculateCommonActors(id1, id2);

                Frame.Navigate(typeof(ResultsListPage), list);
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

            var searchInfo = await m_dbApi.SearchForActor(sender.Text);
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

            var searchInfo = await m_dbApi.SearchForMovie(sender.Text, true);
            if (searchInfo.results.Count == 0)
                return;

            var suggestions = (from result in searchInfo.results
                                   select result).Take(7);

            sender.ItemsSource = suggestions;
        }

        private async void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            await m_dbApi.GetConfiguration();

            if (SearchForMovie)
            {
                var selection = args.SelectedItem as SearchActor_ActorInfo;

                if (selection != null)
                {
                    sender.Tag = selection.id.ToString();
                }

                string path = await m_dbApi.GetActorImageFromID(selection.id.ToString());

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

                var desc = await m_dbApi.GetMovieDescriptionFromID(selection.id.ToString());

                BitmapImage src = new BitmapImage(new Uri(m_dbApi.MakeMoviePosterPath(desc.poster_path)));
                actorImg.Source = src;
            }

            this.searchBtn.Focus(FocusState.Programmatic);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var suggestBox = sender as AutoSuggestBox;

            
            if (suggestBox.FontStyle == Windows.UI.Text.FontStyle.Italic)
            {
                SetSuggestBoxText(suggestBox, "");
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

        private void SetSuggestBoxText(AutoSuggestBox suggestBox, string text)
        {
            suggestBox.Text = text;
            suggestBox.FontStyle = Windows.UI.Text.FontStyle.Normal;
            suggestBox.FontWeight = Windows.UI.Text.FontWeights.Normal;
        }
    }
}
