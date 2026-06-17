using System.Windows;
using System.Windows.Controls;

namespace ChuniPet.Views;

public partial class UniversePageView : UserControl
{
    public event Action? BackClicked;
    
    public UniversePageView()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackClicked?.Invoke();
}