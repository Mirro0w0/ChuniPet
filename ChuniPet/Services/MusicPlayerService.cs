using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;
using ChuniPet.Models;

namespace ChuniPet.Services;

public class MusicPlayerService
{
    private readonly MediaPlayer _player = new();
    private readonly DispatcherTimer _positionTimer;

    public List<Track> Tracks { get; } = new();
    public int CurrentIndex { get; private set; } = 0;
    public bool IsPlaying { get; private set; } = false;
    public bool IsLooping { get; private set; } = false;

    public event Action? TrackChanged;
    public event Action? PlaybackStateChanged;
    public event Action<TimeSpan, TimeSpan>? PositionChanged;

    public Track CurrentTrack => Tracks[CurrentIndex];

    public MusicPlayerService()
    {
        _player.MediaEnded += Player_MediaEnded;
        _player.MediaFailed += (s, e) =>
        {
            Console.WriteLine($"MEDIA FAILED: {e.ErrorException?.Message}");
        };
        _player.MediaOpened += (s, e) =>
        {
            Console.WriteLine($"MEDIA OPENED: Duration = {_player.NaturalDuration}");
        };

        _positionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _positionTimer.Tick += (s, e) =>
        {
            if (_player.NaturalDuration.HasTimeSpan)
                PositionChanged?.Invoke(_player.Position, _player.NaturalDuration.TimeSpan);
        };
    }

    public void LoadTrack(int index, bool autoPlay)
    {
        CurrentIndex = index;
        _player.Open(new Uri(Tracks[index].AudioPath));

        if (autoPlay)
            Play();
        else
            IsPlaying = false;

        TrackChanged?.Invoke();
    }

    public void Play()
    {
        _player.Play();
        IsPlaying = true;
        _positionTimer.Start();
        PlaybackStateChanged?.Invoke();
    }

    public void Pause()
    {
        _player.Pause();
        IsPlaying = false;
        _positionTimer.Stop();
        PlaybackStateChanged?.Invoke();
    }

    public void Stop()
    {
        _player.Stop();
        IsPlaying = false;
        _positionTimer.Stop();
        PlaybackStateChanged?.Invoke();
    }

    public void Next()
    {
        int next = (CurrentIndex + 1) % Tracks.Count;
        LoadTrack(next, autoPlay: true);
    }

    public void Previous()
    {
        int prev = (CurrentIndex - 1 + Tracks.Count) % Tracks.Count;
        LoadTrack(prev, autoPlay: true);
    }

    public void SeekTo(TimeSpan position)
    {
        _player.Position = position;
    }

    public void ToggleLoop()
    {
        IsLooping = !IsLooping;
    }

    private void Player_MediaEnded(object? sender, EventArgs e)
    {
        if (IsLooping)
        {
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }
        else
        {
            Next();
        }
    }
}