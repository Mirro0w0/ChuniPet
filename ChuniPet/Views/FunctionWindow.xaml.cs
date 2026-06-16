// using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace ChuniPet.Views;

public partial class FunctionWindow : Window
{
    public FunctionWindow()
    {
        InitializeComponent();
        ShowMainMenu();
    }
    
    private void ShowMainMenu()
    {
        var menu = new MainMenuView();
        menu.ButtonClicked += OnMenuButtonClicked;
        PageContent.Content = menu;
    }
    
    private void OnMenuButtonClicked(string gateName)
    {
        // For now just show a placeholder page for each
        var page = new GatePageView(gateName);
        page.BackClicked += ShowMainMenu;
        PageContent.Content = page;
    }
    
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(500),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        this.BeginAnimation(OpacityProperty, fadeIn);
    }
    
    private void FunctionWindow_Deactivated(object sender, EventArgs e)
    {
        // this.Close();
        Dispatcher.BeginInvoke(new Action(() => this.Close()),
            System.Windows.Threading.DispatcherPriority.Background);
    }
}