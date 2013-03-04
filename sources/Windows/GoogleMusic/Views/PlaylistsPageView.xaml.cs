﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.GoogleMusic.Views
{
    using OutcoldSolutions.GoogleMusic.BindingModels;
    using OutcoldSolutions.GoogleMusic.Models;
    using OutcoldSolutions.GoogleMusic.Presenters;
    using OutcoldSolutions.GoogleMusic.Services;
    using OutcoldSolutions.GoogleMusic.Shell;
    using OutcoldSolutions.Views;

    using Windows.System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public interface IPlaylistsPageView : IDataPageView
    {
        void EditPlaylist(PlaylistBindingModel selectedItem);

        void ShowPlaylist(PlaylistBindingModel playlistBindingModel);

        void Refresh();
    }

    public sealed partial class PlaylistsPageView : DataPageViewBase, IPlaylistsPageView
    {
        private PlaylistsPageViewPresenter presenter;

        public PlaylistsPageView()
        {
            this.InitializeComponent();
            this.TrackItemsControl(this.ListView);
        }

        public override void OnUnfreeze(NavigatedToEventArgs eventArgs)
        {
            if (eventArgs.Parameter is PlaylistsRequest && ((PlaylistsRequest)eventArgs.Parameter) == PlaylistsRequest.Playlists)
            {
                this.ListView.SelectionMode = ListViewSelectionMode.Single;
            }
            else
            {
                this.ListView.SelectionMode = ListViewSelectionMode.None;
            }

            this.Refresh();
        }

        public void EditPlaylist(PlaylistBindingModel selectedItem)
        {
            this.Container.Resolve<ISearchService>().SetShowOnKeyboardInput(false);
            this.PlaylistNamePopup.VerticalOffset = this.ActualHeight - 240;
            this.TextBoxPlaylistName.Text = selectedItem.Playlist.Title;
            this.SaveNameButton.IsEnabled = !string.IsNullOrEmpty(this.TextBoxPlaylistName.Text);
            this.PlaylistNamePopup.IsOpen = true;
            this.TextBoxPlaylistName.Focus(FocusState.Keyboard);
        }

        public void ShowPlaylist(PlaylistBindingModel playlistBindingModel)
        {
            if (playlistBindingModel != null)
            {
                this.ListView.ScrollIntoView(playlistBindingModel);
            }
        }

        public void Refresh()
        {
            this.Groups.Source = this.presenter.BindingModel.Groups;
            if (Groups.View == null)
            {
                ((ListViewBase)SemanticZoom.ZoomedOutView).ItemsSource = null;
            }
            else
            {
                ((ListViewBase)SemanticZoom.ZoomedOutView).ItemsSource = Groups.View.CollectionGroups;
            }

            this.ListView.SelectedIndex = -1;
            this.SemanticZoom.IsZoomedInViewActive = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.presenter = this.GetPresenter<PlaylistsPageViewPresenter>();
        }

        private void PlaylistItemClick(object sender, ItemClickEventArgs e)
        {
            var playlistBindingModel = e.ClickedItem as PlaylistBindingModel;
            if (playlistBindingModel != null)
            {
                this.NavigationService.ResolveAndNavigateTo<PlaylistViewResolver>(playlistBindingModel.Playlist);
            }
        }
        
        private void SaveNameClick(object sender, RoutedEventArgs e)
        {
            this.presenter.ChangePlaylistName(this.TextBoxPlaylistName.Text);
            this.PlaylistNamePopup.IsOpen = false;
        }

        private void CancelChangeNameClick(object sender, RoutedEventArgs e)
        {
            this.PlaylistNamePopup.IsOpen = false;
        }

        private void TextBoxPlaylistNameKeyUp(object sender, KeyRoutedEventArgs e)
        {
            this.SaveNameButton.IsEnabled = !string.IsNullOrEmpty(this.TextBoxPlaylistName.Text);
        }

        private void TextBoxPlaylistNameOnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.SaveNameClick(sender, e);
                e.Handled = true;
            }
        }

        private void PlaylistNamePopupOnClosed(object sender, object e)
        {
            this.Container.Resolve<ISearchService>().SetShowOnKeyboardInput(true);
        }

        private void ZoomOutClick(object sender, RoutedEventArgs e)
        {
            this.SemanticZoom.IsZoomedInViewActive = false;
        }
    }
}
