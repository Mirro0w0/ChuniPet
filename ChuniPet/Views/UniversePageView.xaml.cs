using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ChuniPet.Services;

namespace ChuniPet.Views;

public partial class UniversePageView : UserControl
{
    public event Action? BackClicked;
    private readonly AppSettings _settings = App.Settings;
    private bool _suppressSync = false;
    private readonly MainWindow _mainWindow;

    public UniversePageView(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        LoadFromSettings();
    }

    private void LoadFromSettings()
    {
        StopMovementToggle.IsChecked = _settings.StopPenguinMovement;
        MoveOnStartupToggle.IsChecked = _settings.PenguinMovesOnStartup;
        RunOnStartupToggle.IsChecked = _settings.RunOnStartup;

        _suppressSync = true;
        VolumeSlider.Value = _settings.AppVolume * 100;
        VolumeTextBox.Text = ((int)(_settings.AppVolume * 100)).ToString();
        _suppressSync = false;
    }

    private void StopMovementToggle_Click(object sender, RoutedEventArgs e)
    {
        _settings.StopPenguinMovement = StopMovementToggle.IsChecked == true;
        _mainWindow.TogglePause();
        SettingsService.Save(_settings);
    }

    private void MoveOnStartupToggle_Click(object sender, RoutedEventArgs e)
    {
        _settings.PenguinMovesOnStartup = MoveOnStartupToggle.IsChecked == true;
        SettingsService.Save(_settings);
    }

    private void RunOnStartupToggle_Click(object sender, RoutedEventArgs e)
    {
        bool enabled = RunOnStartupToggle.IsChecked == true;
        _settings.RunOnStartup = enabled;
        SettingsService.Save(_settings);

        // Apply the actual registry change immediately
        if (enabled) StartupService.EnableStartup();
        else StartupService.DisableStartup();
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_suppressSync) return;

        int rounded = (int)Math.Round(e.NewValue);

        _suppressSync = true;
        VolumeTextBox.Text = rounded.ToString();
        _suppressSync = false;

        _settings.AppVolume = rounded / 100.0;
        SettingsService.Save(_settings);
    }

    private void VolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressSync) return;

        if (int.TryParse(VolumeTextBox.Text, out int value))
        {
            value = Math.Clamp(value, 0, 100);

            _suppressSync = true;
            VolumeSlider.Value = value;
            _suppressSync = false;

            _settings.AppVolume = value / 100.0;
            SettingsService.Save(_settings);
        }
    }

    private void GitHubButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/Mirro0w0/ChuniPet",
            UseShellExecute = true
        });
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to exit ChuniPet?",
            "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            // Application.Current.Shutdown();
            _mainWindow.ExternalClose();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackClicked?.Invoke();
}