using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using ChuniPet.Models;
using SQLite;

namespace ChuniPet.Services;

public static class ClipboardService
{
    private const int PreviewSingleLineLength = 80;
    private const int PreviewMultiLineLength = 60;

    private static SQLiteConnection? _db;
    private static string? _lastSeenText;

    public static event Action? HistoryChanged;

    public static void Initialize()
    {
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChuniPet", "clipboard.db");

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        _db = new SQLiteConnection(dbPath);
        _db.CreateTable<ClipboardEntry>();
        Console.WriteLine("DB initialized successfully");
    }

    public static void DeleteOlderThan(int days)
    {
        _db?.Execute("DELETE FROM ClipboardEntry WHERE CopiedAt < ?", DateTime.Now.AddDays(-days));
    }

    public static string? GetText()
    {
        try
        {
            return Clipboard.ContainsText() ? Clipboard.GetText() : null;
        }
        catch { return null; }
    }

    public static void SetText(string text)
    {
        try { Clipboard.SetText(text); }
        catch { }
    }

    public static void Add(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || _db == null) return;

        // Remove existing duplicate so it re-appears at top instead of being listed twice
        _db.Execute("DELETE FROM ClipboardEntry WHERE Content = ?", text);

        var entry = BuildEntry(text);
        _db.Insert(entry);
        HistoryChanged?.Invoke(); //tells the View "something changed, go refetch"
    }

    public static void Remove(string text)
    {
        _db?.Execute("DELETE FROM ClipboardEntry WHERE Content = ?", text);
        HistoryChanged?.Invoke();
    }

    public static List<ClipboardEntry> GetHistory() =>
        _db?.Table<ClipboardEntry>()
            .OrderByDescending(e => e.CopiedAt)
            .ToList() ?? new List<ClipboardEntry>();

    private static ClipboardEntry BuildEntry(string content)
    {
        bool hasLineBreaks = content.Contains('\n') || content.Contains('\r');
        string preview;

        if (hasLineBreaks)
        {
            var firstLine = content.Split('\n')[0].TrimEnd('\r');
            preview = firstLine.Length > PreviewMultiLineLength
                ? firstLine[..PreviewMultiLineLength] + "..."
                : firstLine + " ⏎";
        }
        else
        {
            preview = content.Length > PreviewSingleLineLength
                ? content[..PreviewSingleLineLength] + "..."
                : content;
        }

        return new ClipboardEntry
        {
            Content = content,
            Preview = preview,
            HasLineBreaks = hasLineBreaks,
            CopiedAt = DateTime.Now
        };
    }
}