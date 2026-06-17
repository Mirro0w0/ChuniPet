using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ChuniPet.Services;

public static class ClipboardMonitor
{
    [DllImport("user32.dll")]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);
    [DllImport("user32.dll")]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    private const int WM_CLIPBOARDUPDATE = 0x031D;
    private static Action? _callback;

    public static void Start(Window window, Action callback)
    {
        _callback = callback;
        var source = PresentationSource.FromVisual(window) as HwndSource;
        source?.AddHook(WndProc);
        AddClipboardFormatListener(new WindowInteropHelper(window).Handle);
    }

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, 
        IntPtr lParam, ref bool handled)
    {
        if (msg == WM_CLIPBOARDUPDATE)
        {
            _callback?.Invoke();
            handled = true;
        }
        return IntPtr.Zero;
    }
}