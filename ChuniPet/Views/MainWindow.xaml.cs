using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChuniPet.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private FunctionWindow? _functionWindow;
    
    
    public MainWindow()
    {
        InitializeComponent();
        
        this.MouseLeftButtonDown += (s, e) => this.DragMove();
        
        // this.MouseRightButtonUp
    }
    
    // private void PetSprite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    // {
    // MouseLeftButtonDown="PetSprite_MouseLeftButtonDown"
    //     DragMove();
    // }
    
    private void PetSprite_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Don't open a second one if already open
        if (_functionWindow != null && _functionWindow.IsVisible)
            return;

        // Position the menu near the pet
        _functionWindow = new FunctionWindow();
        _functionWindow.Left = this.Left + this.Width;   // appears to the right
        _functionWindow.Top  = this.Top - 100;           // slightly above

        _functionWindow.Show();
        e.Handled = true;  // prevent event bubbling
    }
}