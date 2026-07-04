using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChuniPet.Services;
using Microsoft.Win32;

namespace ChuniPet.Views;

public partial class AirPageView : UserControl
{
    public event Action? BackClicked;
    public AirPageView()
    {
        InitializeComponent();
        FolderPathTextBox.Text = App.Settings.RecordingFolder ?? "";
        
        if (App.Recorder.IsRecording)
        {
            RecordButtonImage.Source = StopIcon;
            FolderPathTextBox.IsEnabled = false;
            BrowseButton.IsEnabled = false;
        }

        UpdateButtonStates();
    }

    private readonly SystemAudioRecorder _recorder = new();
    private bool _isRecording = false;

    private static readonly BitmapImage StartIcon = new (new Uri("pack://application:,,,/Assets/Images/UiElements/play.png"));
    private static readonly BitmapImage StopIcon = new (new Uri("pack://application:,,,/Assets/Images/UiElements/stop.png"));
    
    private void FolderPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        string path = FolderPathTextBox.Text.Trim();
        bool isValid = !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        if (isValid)
        {
            App.Settings.RecordingFolder = path;
            SettingsService.Save(App.Settings);
        }

        // While actively recording, keep the toggle button enabled
        // so the user can always press Stop, even if text is edited.
        RecordToggleButton.IsEnabled = isValid || _isRecording;
        OpenFolderButton.IsEnabled = isValid;
    }
    
    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select folder to save recordings",
            InitialDirectory = Directory.Exists(FolderPathTextBox.Text)
                ? FolderPathTextBox.Text
                : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (dialog.ShowDialog() == true)
        {
            FolderPathTextBox.Text = dialog.FolderName;
            App.Settings.RecordingFolder = dialog.FolderName;
            SettingsService.Save(App.Settings);
        }
    }
    
    private void RecordToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (!App.Recorder.IsRecording)
        {
            App.Recorder.StartRecording(FolderPathTextBox.Text.Trim());
            RecordButtonImage.Source = StopIcon;
            FolderPathTextBox.IsEnabled = false;
            BrowseButton.IsEnabled = false;
        }
        else
        {
            App.Recorder.StopRecording();
            RecordButtonImage.Source = StartIcon;
            FolderPathTextBox.IsEnabled = true;
            BrowseButton.IsEnabled = true;
        }

        UpdateButtonStates();
    }
    
    private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
    {
        string path = FolderPathTextBox.Text.Trim();

        if (Directory.Exists(path))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackClicked?.Invoke();
}