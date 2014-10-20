﻿using System;
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
        private MovieDBQueries dbQueries = null;
        private MovieDBAPI dbApi = null;
        private Windows.ApplicationModel.Resources.ResourceLoader resLoader = null;

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

            dbQueries = new MovieDBQueries();
            dbApi = new MovieDBAPI();

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

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            if (autoSuggest1.Tag == null || autoSuggest2.Tag == null)
            {
                commonMovies.Text = "Unknown actor(s) selected!";
                return;
            }
            string id1 = autoSuggest1.Tag.ToString();
            string id2 = autoSuggest2.Tag.ToString();

            var actorInfo1 = await dbApi.GetActorInfoFromID(dbQueries, id1);
            var actorInfo2 = await dbApi.GetActorInfoFromID(dbQueries, id2);

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
        }

        private async void OnSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            sender.Tag = null;
            if (sender.Text.Length <= 2)
            {
                sender.ItemsSource = new string[] {};
                return;
            }

            var searchInfo = await dbApi.SearchForActor(dbQueries, sender.Text);
            if (searchInfo.results.Count == 0)
                return;

            var suggestedActors = (from result in searchInfo.results
                                    select result).Take(7);

            sender.ItemsSource = suggestedActors;
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selection = args.SelectedItem as SearchActor_ActorInfo;

            if (selection != null)
            {
                sender.Tag = selection.id.ToString();
            }
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
            var str = ResLoader.GetString("empty_actor_name");

            suggestBox.Tag = null;
            suggestBox.Text = str;
            suggestBox.FontStyle = Windows.UI.Text.FontStyle.Italic;
            suggestBox.FontWeight = Windows.UI.Text.FontWeights.Light;
        }

    }
}