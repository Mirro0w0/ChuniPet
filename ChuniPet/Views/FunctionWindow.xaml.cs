// using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ChuniPet.Views;

public partial class FunctionWindow : Window
{
    private readonly MainWindow _mainWindow;
    
    public FunctionWindow(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        PauseImage.Source = _mainWindow.IsPaused ? 
            new BitmapImage(new Uri("/Assets/Images/link_level_03.png", UriKind.Relative)) : 
            new BitmapImage(new Uri("/Assets/Images/link_level_02.png", UriKind.Relative));
        ShowMainMenu();
    }
    
    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.TogglePause();
        // Assumes you have an <Image x:Name="myImage"/> control in your XAML

        PauseImage.Source = _mainWindow.IsPaused ? 
            new BitmapImage(new Uri("/Assets/Images/link_level_03.png", UriKind.Relative)) : 
            new BitmapImage(new Uri("/Assets/Images/link_level_02.png", UriKind.Relative));
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
        // var page = new GatePageView(gateName);
        // page.BackClicked += ShowMainMenu;
        // PageContent.Content = page;
        UserControl page = gateName switch
        {
            "ORIGIN"   => new OriginPageView(),
            "UNIVERSE" => new UniversePageView(),
            _          => new GatePageView(gateName)
        };

        if (page is GatePageView gp)
            gp.BackClicked += ShowMainMenu;
        else if (page is OriginPageView op)
            op.BackClicked += ShowMainMenu;
        else if (page is UniversePageView up)
            up.BackClicked += ShowMainMenu;

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