using System.Windows;
using System.Windows.Controls;

namespace ChuniPet.Views;

// Views/GatePageView.xaml.cs
public partial class GatePageView : UserControl
{
    public event Action? BackClicked;

    public GatePageView(string gateName)
    {
        InitializeComponent();
        GateTitle.Text = gateName;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackClicked?.Invoke();
}