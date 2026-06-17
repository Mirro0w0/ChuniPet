// Services/ClipboardService.cs
using System.Windows;

namespace ChuniPet.Services;

public static class ClipboardService
{
    public static string? GetText()
    {
        try
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : null;
        }
        catch { return null; }
    }
    
    private static readonly List<string> _history = new();
    private const int MaxItems = 50;

    // Fires whenever history changes — OriginPageView listens to this
    public static event Action? HistoryChanged;

    public static void Add(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // Remove duplicate if already exists, re-add at top
        _history.Remove(text);
        _history.Insert(0, text);

        // Cap at max items
        if (_history.Count > MaxItems)
            _history.RemoveAt(_history.Count - 1);

        HistoryChanged?.Invoke();
    }

    public static void Remove(string text)
    {
        _history.Remove(text);
        Console.WriteLine("removed");
        HistoryChanged?.Invoke();
    }

    public static List<string> GetHistory() => new(_history);  // return a copy

    public static void SetText(string text)
    {
        try { Clipboard.SetText(text); }
        catch { }
    }
}