using MovieLapsus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MovieLapsus
{
    class Provider
    {
        private bool movieSearch = false;
        private TMDB.TMDBAPI m_dbApi = null;

        public Provider(TMDB.TMDBAPI dbApi, string parameter)
        {
            movieSearch = parameter == "movie";
            m_dbApi = dbApi;
        }

        public string SearchDescriptionId
        {
            get
            {
                if (movieSearch)
                    return "SearchDesc4Movie";

                return "SearchDesc4Actor";
            }
        }

        public string NoResultId
        {
            get
            {
                return movieSearch ? "no_common_movies" : "no_common_actors";
            }
        }

        public string EmptySuggestBoxId
        {
            get
            {
                return movieSearch ? "empty_actor_name" : "empty_movie_name";
            }
        }

        public async Task<System.Collections.Generic.IEnumerable<IResultsListItem>> GetCommonResults(string id1, string id2)
        {
            var calc = new CommonCalculator(m_dbApi);

            IEnumerable<IResultsListItem> list = null;
            if (movieSearch)
            {
                list = await calc.CalculateCommonMovies(id1, id2);
            }
            else
            {
                list = await calc.CalculateCommonActors(id1, id2);
            }

            return list;
        }

        public async Task<string> GetPosterPath(string id)
        {
            if (movieSearch)
            {
                return await m_dbApi.GetActorImageFromID(id);
            }
            else
            {
                var desc = await m_dbApi.GetMovieDescriptionFromID(id);
                return m_dbApi.MakeMoviePosterPath(desc.poster_path);
            }
        }

        public async Task<string> GetIdFromName(string text)
        {
            if (text == null || text.Length == 0)
                return null;

            if (movieSearch)
            {
                var searchInfoActor = await m_dbApi.SearchForActor(text);
                if (searchInfoActor.results.Count > 0)
                {
                    var actor = (from result in searchInfoActor.results
                                 orderby result.popularity descending
                                 select result).First();

                    return actor.id.ToString();
                }
            }
            else
            {
                var searchInfo = await m_dbApi.SearchForMovie(text, false);
                if (searchInfo.results.Count > 0)
                {
                    var movie = (from result in searchInfo.results
                                 orderby result.popularity descending
                                 select result).First();

                    return movie.id.ToString();
                }
            }

            return null;
        }

        public string GetObjectIdStr(Object obj)
        {
            try
            {
                if (movieSearch)
                {
                    var selection = obj as SearchActor_ActorInfo;
                    return selection.id.ToString();
                }
                else
                {
                    var selection = obj as SearchMovie_Result;
                    return selection.id.ToString();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Object> GetSuggestions(string suggestionText)
        {
            const int SUGGESTION_NO = 5;

            if (movieSearch)
            {
                var searchInfo = await m_dbApi.SearchForActor(suggestionText);
                if (searchInfo.results.Count == 0)
                    return null;

                var suggestions = (from result in searchInfo.results
                                   select result).Take(SUGGESTION_NO);

                return suggestions;
            }
            else
            {
                var searchInfo = await m_dbApi.SearchForMovie(suggestionText, true);
                if (searchInfo.results.Count == 0)
                    return null;

                var suggestions = (from result in searchInfo.results
                                   select result).Take(SUGGESTION_NO);

                return suggestions;
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private readonly NavigationHelper navigationHelper;

        private ResourceLoader m_resLoader = null;
        private Provider provider = null;
        private Task<DBConfig> m_getConfigTask = null;

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
            var dbApi = new TMDB.TMDBAPI();
            m_getConfigTask = dbApi.GetConfiguration();
            var searchParameter = e.NavigationParameter.ToString();

            provider = new Provider(dbApi, searchParameter);

            if (e.PageState != null)
            {
                autoSuggest1.Tag = e.PageState["tag1"];
                SetSuggestBoxText(autoSuggest1, e.PageState["name1"] as string);
                autoSuggest2.Tag = e.PageState["tag2"];
                SetSuggestBoxText(autoSuggest2, e.PageState["name2"] as string);
                actorImg1.Source = e.PageState["url1"] as BitmapImage;
                actorImg2.Source = e.PageState["url2"] as BitmapImage;
            }

            searchDescTB.Text = ResLoader.GetString(provider.SearchDescriptionId);

            if (autoSuggest1.Tag == null)
            {
                SetEmptySuggestBox(autoSuggest1);
            }
            if (autoSuggest2.Tag == null)
            {
                SetEmptySuggestBox(autoSuggest2);
            }

            StartAnimation();
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
            e.PageState["tag1"] = autoSuggest1.Tag;
            e.PageState["name1"] = autoSuggest1.Text;
            e.PageState["tag2"] = autoSuggest2.Tag;
            e.PageState["name2"] = autoSuggest2.Text;
            e.PageState["url1"] = actorImg1.Source;
            e.PageState["url2"] = actorImg2.Source;
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
            commonMovies.Text = "";

            if (autoSuggest1.Tag == null && autoSuggest2.Tag == null)
            {
                commonMovies.Text = "Unknown actor(s) selected!";
                return;
            }

            string id1 = (autoSuggest1.Tag == null)? null : autoSuggest1.Tag.ToString();
            string id2 = (autoSuggest2.Tag == null) ? null : autoSuggest2.Tag.ToString();

            var list = await this.provider.GetCommonResults(id1, id2);

            if (list == null || list.Count() == 0)
            {
                commonMovies.Text = ResLoader.GetString(this.provider.NoResultId);
                return;
            }

            Frame.Navigate(typeof(ResultsListPage), list);
        }

        private async void OnSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            sender.Tag = null;
            if (sender.Text.Length <= 3)
            {
                sender.ItemsSource = new string[] { };
                return;
            }

            var suggestions = await provider.GetSuggestions(sender.Text);
            sender.ItemsSource = suggestions;
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("OnSuggestionChosen");

            var tag = provider.GetObjectIdStr(args.SelectedItem);
            if (tag != null)
            {
                sender.Tag = tag;
            }

            this.searchBtn.Focus(FocusState.Programmatic);

            RefreshControl(sender);
        }

        private void StartAnimation()
        {
            if (actorImg1.Source == null && actorImg2.Source == null)
            {
                return;
            }

            double to1 = 0,
                   to2 = 0;

            if (actorImg1.Source != null && actorImg2.Source != null)
            {
                to1 = 0;
                to2 = ImagesCanvas.Width - actorImg2.Width;
            }
            else
            {
                to1 = (ImagesCanvas.Width - actorImg1.Width) / 2;
                to2 = (ImagesCanvas.Width - actorImg2.Width) / 2;
            }

            var sb = new Storyboard();
            AnimateControl(sb, actorImg1, to1);
            AnimateControl(sb, actorImg2, to2);
            sb.Begin();
        }

        private void AnimateControl(Storyboard sb, Image imgContrl, double to)
        {
            DoubleAnimation di1 = new DoubleAnimation();
            di1.To = to;
            di1.Duration = new Duration(TimeSpan.FromMilliseconds(700));
            Storyboard.SetTarget(di1, imgContrl);
            Storyboard.SetTargetProperty(di1, "(Canvas.Left)");
            sb.Children.Add(di1);
        }

        private async void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (m_getConfigTask != null)
            {
                await m_getConfigTask;
                m_getConfigTask = null;
            }

            var suggestBox = sender as AutoSuggestBox;

            if (suggestBox.FontStyle == Windows.UI.Text.FontStyle.Italic)
            {
                SetSuggestBoxText(suggestBox, "");
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var suggestBox = sender as AutoSuggestBox;

            RefreshControl(suggestBox);
        }

        private void SetEmptySuggestBox(AutoSuggestBox suggestBox)
        {
            string str = ResLoader.GetString(this.provider.EmptySuggestBoxId);

            suggestBox.Tag = null;
            suggestBox.Text = str;
            suggestBox.FontStyle = Windows.UI.Text.FontStyle.Italic;
            suggestBox.FontWeight = Windows.UI.Text.FontWeights.Light;
        }

        private Image ImageControlFromEditControl(AutoSuggestBox suggestBox)
        {
            if (suggestBox.Name == "autoSuggest1")
            {
                return actorImg1;
            }

            if (suggestBox.Name == "autoSuggest2")
            {
                return actorImg2;
            }

            throw new Exception("unrecognised control");
        }

        private void SetSuggestBoxText(AutoSuggestBox suggestBox, string text)
        {
            suggestBox.Text = text;
            suggestBox.FontStyle = Windows.UI.Text.FontStyle.Normal;
            suggestBox.FontWeight = Windows.UI.Text.FontWeights.Normal;
        }

        private async void RefreshControl(AutoSuggestBox suggestBox)
        {
            var imageControl = this.ImageControlFromEditControl(suggestBox);

            if (suggestBox.Tag == null)
            {
                suggestBox.Tag = await provider.GetIdFromName(suggestBox.Text);
            }
            if (suggestBox.Tag == null)
            {
                imageControl.Source = null;
                SetEmptySuggestBox(suggestBox);
            }
            else
            {
                string id = suggestBox.Tag as string;
                string imgPath = await provider.GetPosterPath(id);

                if (imgPath != null && imgPath.Length > 0)
                {
                    var imgUri = new Uri(imgPath);
                    var srcImg = new BitmapImage(imgUri);

                    imageControl.Source = srcImg;
                }
            }

            StartAnimation();
        }
    }
}
