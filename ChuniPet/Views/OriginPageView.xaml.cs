using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ChuniPet.Models;
using ChuniPet.Services;

namespace ChuniPet.Views;

public partial class OriginPageView : UserControl
{
    public event Action? BackClicked;

    private static readonly BitmapImage _bgNormal = new(new Uri("pack://application:,,,/Assets/Images/honor_bg_gold.png"));
    private static readonly BitmapImage _bgPressed = new(new Uri("pack://application:,,,/Assets/Images/honor_bg_master.png"));

    private DateTime _selectedDate = DateTime.Today;
    private readonly List<Button> _dayButtons = new();

    public OriginPageView()
    {
        InitializeComponent();
        BuildDayButtons();
        RefreshList();

        ClipboardService.HistoryChanged += OnHistoryChanged;
    }

    // TODO fix the date display
    private void BuildDayButtons()
    {
        DayButtonsPanel.Children.Clear();
        _dayButtons.Clear();

        // 7 days, oldest on the left, today on the right (rightmost, default selected)
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.Today.AddDays(-i);
            var button = new Button
            {
                Content = date == DateTime.Today ? "Today" : date.ToString("ddd"),
                Tag = date,
                Style = (Style)FindResource("DayToggleButtonStyle"),
                Margin = new Thickness(2, 0, 2, 0)
            };
            button.Click += DayButton_Click;
            _dayButtons.Add(button);
            DayButtonsPanel.Children.Add(button);
        }

        SetSelectedDay(DateTime.Today);
    }

    private void DayButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is DateTime date)
        {
            SetSelectedDay(date);
            RefreshList(SearchBox.Text);
        }
    }

    private void SetSelectedDay(DateTime date)
    {
        _selectedDate = date;

        foreach (var btn in _dayButtons)
        {
            bool isSelected = btn.Tag is DateTime d && d == date;
            btn.Opacity = isSelected ? 1.0 : 0.55;
            btn.FontWeight = isSelected ? FontWeights.Bold : FontWeights.Normal;
        }
    }

    private void OnHistoryChanged()
    {
        Dispatcher.Invoke(() => RefreshList(SearchBox.Text));
    }

    private void RefreshList(string filter = "")
    {
        var items = ClipboardService.GetHistory()
            .Where(x => x.CopiedAt.Date == _selectedDate.Date);

        if (!string.IsNullOrWhiteSpace(filter))
            items = items.Where(x => x.Content.Contains(filter, StringComparison.OrdinalIgnoreCase));

        HistoryList.ItemsSource = items.ToList();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshList(SearchBox.Text);
    }

    private void HistoryItem_Hold(object sender, MouseButtonEventArgs e)
    {
        Console.WriteLine("Holding");
        if (sender is Grid grid)
            grid.Children.OfType<Image>().FirstOrDefault()!.Source = _bgPressed; //_bgNormal
    }

    private void HistoryItem_Click(object sender, MouseButtonEventArgs e)
    {
        Console.WriteLine("clicked");
        if (sender is Grid grid && grid.DataContext is ClipboardEntry entry)
        {
            grid.Children.OfType<Image>().FirstOrDefault()!.Source = _bgNormal;
            ClipboardService.SetText(entry.Content);
        }
    }

    private void DeleteEntry_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ClipboardEntry entry)
        {
            ClipboardService.Remove(entry.Content);
            RefreshList(SearchBox.Text);
        }
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            $"Delete all clipboard history for {_selectedDate:MMM dd}?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        var itemsToDelete = ClipboardService.GetHistory()
            .Where(x => x.CopiedAt.Date == _selectedDate.Date)
            .ToList();

        foreach (var item in itemsToDelete)
            ClipboardService.Remove(item.Content);

        RefreshList(SearchBox.Text);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ClipboardService.HistoryChanged -= OnHistoryChanged;
        BackClicked?.Invoke();
    }
}