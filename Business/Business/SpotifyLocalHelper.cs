﻿using System;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;

namespace Business.Business
{
    public class SpotifyLocalHelper
    {
        private readonly SpotifyLocalAPI _spotify;

        public bool IsConnected;
        public event EventHandler<TrackChangeEventArgs> OnTrackChange;
        public event EventHandler<TrackTimeChangeEventArgs> OnTrackTimeChange;
        public event EventHandler<PlayStateEventArgs> OnPlayStateChange;

        public SpotifyLocalHelper()
        {
            _spotify = new SpotifyLocalAPI();
            _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
            _spotify.OnTrackChange += _spotify_OnTrackChange;
            _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;
        }

        public void Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning() || !SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                IsConnected = false;
                return;
            }

            var successful = _spotify.Connect();
            if (successful)
            {
                IsConnected = true;
                UpdateInfos();
                _spotify.ListenForEvents = true;
            }
        }

        public Track UpdateInfos()
        {
            var status = _spotify.GetStatus();
            return status?.Track ?? new Track();
        }

        private void _spotify_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            OnTrackTimeChange?.Invoke(this, e);
        }

        private void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            OnTrackChange?.Invoke(this, e);
        }

        private void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            OnPlayStateChange?.Invoke(this, e);
        }
    }
}