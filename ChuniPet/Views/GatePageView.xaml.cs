using System.Windows;
using System.Windows.Controls;
using System.Media;
using ChuniPet.Services;


namespace ChuniPet.Views;

// Views/GatePageView.xaml.cs
public partial class GatePageView : UserControl
{
    public event Action? BackClicked;

    public GatePageView(string gateName)
    {
        InitializeComponent();
        // SoundPlayer player = new SoundPlayer("/Assets/Audio/pressed.wav");
        // player.Play();
        GateTitle.Text = gateName;
        
        if(gateName == "ORIGIN")
        {
            ClipboardService.SetText("ubiaesubiafsdubifabeuis");
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackClicked?.Invoke();
}