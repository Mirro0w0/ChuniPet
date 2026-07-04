using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace ChuniPet.Services;

public static class StartupService
{
    private const string AppName = "ChuniPet";
    private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    
    public static void EnableStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        key?.SetValue(AppName, $"\"{GetExecutablePath()}\"");
    }

    /// <summary>
    /// Removes the registry entry, disabling auto-start.
    /// </summary>
    public static void DisableStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        if (key?.GetValue(AppName) != null)
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    /// <summary>
    /// Checks whether the startup entry currently exists and points to the correct path.
    /// Useful for syncing settings.json state with the actual registry state on load.
    /// </summary>
    public static bool IsStartupEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        var value = key?.GetValue(AppName) as string;

        if (string.IsNullOrEmpty(value))
            return false;

        // Confirm it still points to the current exe location,
        // not a stale path from a previous install location.
        string expectedPath = $"\"{GetExecutablePath()}\"";
        return string.Equals(value, expectedPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Resolves the actual .exe path, even when running from a single-file
    /// publish or when Assembly.Location points to a .dll instead of .exe.
    /// </summary>
    private static string GetExecutablePath()
    {
        return Environment.ProcessPath
               ?? Process.GetCurrentProcess().MainModule!.FileName;
    }
}