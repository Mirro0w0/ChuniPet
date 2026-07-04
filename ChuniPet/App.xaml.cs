using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using ChuniPet.Models;
using ChuniPet.Services;

namespace ChuniPet;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static AppSettings Settings { get; private set; } = new();
    public static SystemAudioRecorder Recorder { get; } = new();
    public static MusicPlayerService MusicPlayer { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        Settings = SettingsService.Load();
        SeedTracks();
    }

    private void SeedTracks()
    {
        MusicPlayer.Tracks.AddRange(new[]
        {
            new Track
            {
                Title = "神鳴", Artist = "光吉猛修", AudioPath = BuildMusicPath("Kaminari.wav"),
                Jacket = "/Assets/Images/Jackets/Kaminari.png",
                GateIcon = "/Assets/Images/Linked_GATE_ORIGIN.png",
                Link = ""
            },
            new Track
            {
                Title = "Tru'nembra", Artist = "Team Grimoire & 穴山大輔", AudioPath = BuildMusicPath("Tru'nembra.wav"),
                Jacket = "/Assets/Images/Jackets/Tru'nembra.png",
                GateIcon = "/Assets/Images/Linked_GATE_AIR.png"
            },
            new Track
            {
                Title = "Everything Will Be One", Artist = "void(Mournfinale)",
                AudioPath = BuildMusicPath("Everything_Will_Be_One.wav"),
                Jacket = "/Assets/Images/Jackets/Everything_Will_Be_One.png",
                GateIcon = "/Assets/Images/Linked_GATE_STAR.png"
            },
            new Track
            {
                Title = "OUTRAGE", Artist = "USAO vs DJ Myosuke", AudioPath = BuildMusicPath("Outrage.wav"),
                Jacket = "/Assets/Images/Jackets/OUTRAGE.png",
                GateIcon = "/Assets/Images/Linked_GATE_AMAZON.png"
            },
            new Track
            {
                Title = "輪廻玲々", Artist = "suzu", AudioPath = BuildMusicPath("Rinne_Reirei.wav"),
                Jacket = "/Assets/Images/Jackets/Rinne_reirei.png",
                GateIcon = "/Assets/Images/Linked_GATE_CRYSTAL.png"
            },
            new Track
            {
                Title = "創 -汝ら新世界へ歩む者なり-", Artist = "BlackY VS Yooh VS siromaru VS xi VS モリモリあつし",
                AudioPath = BuildMusicPath("Tsukuri.wav"), Jacket = "/Assets/Images/Jackets/Tsukuri.png",
                GateIcon = "/Assets/Images/Linked_GATE_PARADISE.png"
            },
            new Track
            {
                Title = "轆轤首", Artist = "かねこちはる", AudioPath = BuildMusicPath("Rokurokubi.wav"),
                Jacket = "/Assets/Images/Jackets/Rokurokubi.png",
                GateIcon = "/Assets/Images/Linked_GATE_NEW.png"
            },
            new Track
            {
                Title = "Sweet & Sour", Artist = "Sakuzyo or Sobrem", AudioPath = BuildMusicPath("Sweet_and_Sour.wav"),
                Jacket = "/Assets/Images/Jackets/Sweet_and_Sour.png",
                GateIcon = "/Assets/Images/Linked_GATE_SUN.png"
            },
            new Track
            {
                Title = "Phantom Crisis", Artist = "t+pazolite vs Yuta Imai",
                AudioPath = BuildMusicPath("Phantom_Crisis.wav"), Jacket = "/Assets/Images/Jackets/Phantom_Crisis.png",
                GateIcon = "/Assets/Images/Linked_GATE_LUMINOUS.png"
            },
            new Track
            {
                Title = "月葬", Artist = "黒魔 × rintaro soma", AudioPath = BuildMusicPath("Gessou.wav"),
                Jacket = "/Assets/Images/Jackets/Gessou.png",
                GateIcon = "/Assets/Images/Linked_GATE_VERSE.png"
            },
            new Track
            {
                Title = "YOUNITHM", Artist = "大国奏音", AudioPath = BuildMusicPath("Younithm.wav"),
                Jacket = "/Assets/Images/Jackets/YOUNITHM.png",
                GateIcon = "/Assets/Images/Linked_GATE_X-VERSE.png"
            },
            new Track
            {
                Title = "Melodiniq", Artist = "onoken a.k.a. owl＊tree", AudioPath = BuildMusicPath("Melodiniq.wav"),
                Jacket = "/Assets/Images/Jackets/Melodiniq.png",
                GateIcon = "/Assets/Images/Linked_GATE_UNIVERSE.png"
            },
            new Track
            {
                Title = "Linked Tune", Artist = "水野健治", AudioPath = BuildMusicPath("Linked_Tune.wav"),
                Jacket = "/Assets/Images/Jackets/Linked_Tune.png",
                GateIcon = "/Assets/Images/gateFrame.png"
            },
        });

        MusicPlayer.LoadTrack(0, autoPlay: false);
    }

    private static string BuildMusicPath(string fileName)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine(Path.Combine(baseDir, "Assets", "Music", fileName));
        return Path.Combine(baseDir, "Assets", "Music", fileName);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SettingsService.Save(Settings);
        if (Recorder.IsRecording)
            Recorder.StopRecording();
        base.OnExit(e);
    }
}