using System.IO;
using System.Text.Json;

namespace ChuniPet.Services;

public class AppSettings
{
    public string? RecordingFolder { get; set; }
    public bool StopPenguinMovement { get; set; } = true;
    public bool PenguinMovesOnStartup { get; set; } = false;
    public bool RunOnStartup { get; set; } = true;
    public double AppVolume { get; set; } = 1.0;
}

public static class SettingsService
{
    private static readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ChuniPet", "settings.json");

    public static AppSettings Load()
    {
        if (!File.Exists(_path)) return new AppSettings();
        return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path))
               ?? new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings));
    }
}