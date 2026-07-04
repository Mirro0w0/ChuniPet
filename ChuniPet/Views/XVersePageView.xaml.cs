using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ChuniPet;
using ChuniPet.Services;

namespace ChuniPet.Views;

public partial class XVersePageView : UserControl
{
    
    private readonly MusicPlayerService _player = App.MusicPlayer;
    private bool _isDraggingSlider = false;

    private static readonly BitmapImage PlayIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/play_audio.png"));
    private static readonly BitmapImage PauseIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/pause_audio.png"));
    private static readonly BitmapImage LoopOnIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/loop_audio.png"));
    private static readonly BitmapImage LoopOffIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/loop_audio_end.png"));
    // private static readonly BitmapImage PlayIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/play.png"));
    // private static readonly BitmapImage PauseIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/play.png"));
    // private static readonly BitmapImage LoopOnIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/play.png"));
    // private static readonly BitmapImage LoopOffIcon = new(new Uri("pack://application:,,,/Assets/Images/UiElements/play.png"));
    
    public XVersePageView()
    {
        InitializeComponent();
        
        _player.TrackChanged += OnTrackChanged;
        _player.PlaybackStateChanged += OnPlaybackStateChanged;
        _player.PositionChanged += OnPositionChanged;

        // Sync UI to whatever state the player is already in
        RefreshTrackInfo();
        RefreshPlayPauseIcon();
        RefreshLoopIcon();

        Unloaded += (s, e) =>
        {
            // Unsubscribe to avoid leaking handlers when view is recreated
            _player.TrackChanged -= OnTrackChanged;
            _player.PlaybackStateChanged -= OnPlaybackStateChanged;
            _player.PositionChanged -= OnPositionChanged;
        };
    }
    private void OnTrackChanged() => Dispatcher.Invoke(RefreshTrackInfo);
    private void OnPlaybackStateChanged() => Dispatcher.Invoke(RefreshPlayPauseIcon);

    private void OnPositionChanged(TimeSpan current, TimeSpan total)
    {
        Dispatcher.Invoke(() =>
        {
            if (_isDraggingSlider) return;

            SeekSlider.Maximum = total.TotalSeconds;
            SeekSlider.Value = current.TotalSeconds;

            CurrentTimeText.Text = current.ToString(@"m\:ss");
            TotalTimeText.Text = total.ToString(@"m\:ss");
        });
    }

    private void RefreshTrackInfo()
    {
        var track = _player.CurrentTrack;
        TrackTitleText.Text = track.Title;
        TrackArtistText.Text = track.Artist;
        AlbumArtImage.Source = new BitmapImage(new Uri("pack://application:,,," + track.Jacket));
        GateIcon.Source = new BitmapImage(new Uri("pack://application:,,," + track.GateIcon));
    }

    private void RefreshPlayPauseIcon()
    {
        PlayPauseImage.Source = _player.IsPlaying ? PauseIcon : PlayIcon;
    }

    private void RefreshLoopIcon()
    {
        LoopImage.Source = _player.IsLooping ? LoopOnIcon : LoopOffIcon;
    }

    // ─── Controls ────────────────────────────────────────────────────
    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_player.IsPlaying) _player.Pause();
        else
        {
            Console.WriteLine("playing");
            _player.Play();
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e) => _player.Stop();

    private void NextButton_Click(object sender, RoutedEventArgs e) => _player.Next();

    private void PrevButton_Click(object sender, RoutedEventArgs e) => _player.Previous();

    private void LoopButton_Click(object sender, RoutedEventArgs e)
    {
        _player.ToggleLoop();
        RefreshLoopIcon();
    }

    private void SeekSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {
        _isDraggingSlider = true;
    }

    private void SeekSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        _player.SeekTo(TimeSpan.FromSeconds(SeekSlider.Value));
        _isDraggingSlider = false;
    }
}