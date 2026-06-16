using System.Windows;
using System.Windows.Controls;

namespace ChuniPet.Views
{
    public partial class MainMenuView : UserControl
    {
        // FunctionWindow listens to this event
        public event Action<string>? ButtonClicked;

        public MainMenuView() => InitializeComponent();

        private void Btn_Click1(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("ORIGIN");
        private void Btn_Click2(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("AIR");
        private void Btn_Click3(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("STAR");
        private void Btn_Click4(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("AMAZON");
        private void Btn_Click5(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("CRYSTAL");
        private void Btn_Click6(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("PARADISE");
        private void Btn_Click7(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("NEW");
        private void Btn_Click8(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("SUN");
        private void Btn_Click9(object sender, RoutedEventArgs e)  => ButtonClicked?.Invoke("LUMINOUS");
        private void Btn_Click10(object sender, RoutedEventArgs e) => ButtonClicked?.Invoke("VERSE");
        private void Btn_Click11(object sender, RoutedEventArgs e) => ButtonClicked?.Invoke("X-VERSE");
        private void Btn_Click12(object sender, RoutedEventArgs e) => ButtonClicked?.Invoke("UNIVERSE");
    }
}