﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Services.Publishers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OutcoldSolutions.GoogleMusic.Models;
    using OutcoldSolutions.GoogleMusic.Web.Lastfm;

    public class LastFmCurrentSongPublisher : ICurrentSongPublisher
    {
        private readonly IApplicationStateService stateService;
        private readonly ILastfmWebService webService;

        public LastFmCurrentSongPublisher(
            IApplicationStateService stateService,
            ILastfmWebService webService)
        {
            this.stateService = stateService;
            this.webService = webService;
        }

        public PublisherType PublisherType
        {
            get { return PublisherType.Immediately; }
        }

        public async Task PublishAsync(Song song, IPlaylist currentPlaylist, Uri imageUri, CancellationToken cancellationToken)
        {
            if (this.stateService.IsOnline())
            {
                var startPlaying = DateTime.UtcNow;

                cancellationToken.ThrowIfCancellationRequested();

                var parameters = new Dictionary<string, string>()
                                 {
                                     { "artist", song.ArtistTitle },
                                     { "track", song.Title },
                                     { "album", song.AlbumTitle },
                                     { "trackNumber", song.Track.HasValue ? song.Track.Value.ToString("D") : string.Empty },
                                     { "duration", ((int)song.Duration.TotalSeconds).ToString("D") }
                                 };

                if (!string.IsNullOrEmpty(song.AlbumArtistTitle)
                    && string.Equals(song.AlbumArtistTitle, song.ArtistTitle, StringComparison.OrdinalIgnoreCase))
                {
                    parameters.Add("albumArtist", song.AlbumArtistTitle);
                }

                Task nowPlayingTask = this.webService.CallAsync("track.updateNowPlaying", new Dictionary<string, string>(parameters));

                cancellationToken.ThrowIfCancellationRequested();

                // Last.fm only accept songs with > 30 seconds
                if (song.Duration.TotalSeconds >= 30)
                {
                    // 4 minutes or half of the track
                    await Task.Delay(Math.Min(4 * 60 * 1000, (int)(song.Duration.TotalMilliseconds / 2)), cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    var scrobbleParameters = new Dictionary<string, string>(parameters)
                                             {
                                                 { "timestamp", ((int)(startPlaying.ToUnixFileTime() / 1000)).ToString("D") }
                                             };

                    await this.webService.CallAsync("track.scrobble", scrobbleParameters);
                }

                cancellationToken.ThrowIfCancellationRequested();

                await nowPlayingTask;
            }
        }
    }
}