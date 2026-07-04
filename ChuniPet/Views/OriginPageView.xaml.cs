using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ChuniPet.Services;

namespace ChuniPet.Views;

public partial class OriginPageView : UserControl
{
    public event Action? BackClicked;
    
    private static readonly BitmapImage _bgNormal = new (new Uri("pack://application:,,,/Assets/Images/honor_bg_gold.png"));
    private static readonly BitmapImage _bgPressed = new (new Uri("pack://application:,,,/Assets/Images/honor_bg_master.png"));

    public OriginPageView()
    {
        InitializeComponent();
        RefreshList();

        // Listen for new clipboard entries while this page is open
        ClipboardService.HistoryChanged += OnHistoryChanged;
    }

    private void OnHistoryChanged()
    {
        // Marshal to UI thread since clipboard events may come from background
        Dispatcher.Invoke(RefreshList);
    }

    private void RefreshList(string filter = "")
    {
        var items = ClipboardService.GetHistory();

        if (!string.IsNullOrWhiteSpace(filter))
            items = items.Where(x => 
                x.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        HistoryList.ItemsSource = items;
    }
    
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshList(SearchBox.Text);
    }
    
    private void HistoryItem_Hold(object sender, MouseButtonEventArgs e)
    {
        if (sender is Grid grid)
            grid.Children.OfType<Image>().FirstOrDefault()!.Source = _bgNormal;
    }
    
    private void HistoryItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Grid grid && grid.DataContext is string text)
        {
            grid.Children.OfType<Image>().FirstOrDefault()!.Source = _bgPressed;
            Clipboard.SetText(text);
        }
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ClipboardService.HistoryChanged -= OnHistoryChanged;
        BackClicked?.Invoke();
    }
}