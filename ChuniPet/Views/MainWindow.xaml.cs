using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ChuniPet.Services;
using WpfAnimatedGif;

namespace ChuniPet.Views;

public partial class MainWindow : Window
{
    private FunctionWindow? _functionWindow;
    
    private enum PetState { Idle, WalkLeft, WalkRight, Falling, Dragging, Jumping }
    private PetState _currentState = PetState.Idle;
    private const double WalkSpeed = 0.5;
    private readonly Random _random = new();
    private DateTime _nextStateChange = DateTime.Now;
    
    private double _velocityY = 0;
    private const double Gravity = 1.2;
    private const double MaxFallSpeed = 20;
    private bool _isFalling = false;
    private DateTime _lastRenderTime;
    
    private bool _isDragging = false;
    private Point _dragStartScreenPos;
    private Point _dragStartWindowPos;
    
    private bool _isPaused = false;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) => SnapToTaskbar();
        StartGroundBehaviour();
    }
    
    
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        ClipboardMonitor.Start(this, OnClipboardChanged);  // ← starts here
    }
    
    private void OnClipboardChanged()
    {
        try
        {
            if (System.Windows.Clipboard.ContainsText())
                ClipboardService.Add(System.Windows.Clipboard.GetText());
        }
        catch { }
    }
    
    private void SnapToTaskbar()
    {
        var workArea = SystemParameters.WorkArea;
        this.Left = (workArea.Width / 2) - (this.Width / 2);
        this.Top  = workArea.Bottom - this.Height;
    }
    
    private void StartFalling()
    {
        _velocityY = 0;
        _isFalling = true;
        _lastRenderTime = DateTime.Now;
        // SetStaticImage("/Assets/Images/penguin10.png");
        SetStaticImage("/Assets/Images/penguin10.png");
        CompositionTarget.Rendering += PhysicsTick;
    }

    private void StopFalling()
    {
        _isFalling = false;
        CompositionTarget.Rendering -= PhysicsTick;
        _currentState = PetState.Idle;
        SetAnimation("/Assets/Images/pen_sleep_apng.gif");
        if (!_isPaused) StartGroundBehaviour();
    }

    private void PhysicsTick(object? sender, EventArgs e)
    {
        // Delta time — framerate-independent physics
        var now = DateTime.Now;
        double deltaMs = (now - _lastRenderTime).TotalMilliseconds;
        _lastRenderTime = now;
        double deltaFactor = deltaMs / 16.0;   // 1.0 = baseline at 60fps

        var workArea = SystemParameters.WorkArea;
        double floor = workArea.Bottom - this.Height;

        _velocityY = Math.Min(_velocityY + (Gravity * deltaFactor), MaxFallSpeed);
        double newTop = this.Top + (_velocityY * deltaFactor);

        if (newTop >= floor)
        {
            this.Top = floor;
            _velocityY = 0;
            StopFalling();
            // TODO: switch to idle/walk animation
        }
        else
        {
            this.Top = newTop;
        }

        // Clamp to screen edges horizontally
        var clampedLeft = Math.Max(0, Math.Min(this.Left, workArea.Width - this.Width));
        if (this.Left != clampedLeft) this.Left = clampedLeft;
    }
    
    private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        StartFalling();
    }
    
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SetStaticImage("/Assets/Images/penguin02.png");
        _isDragging = false;
        StopGroundBehaviour(); 
        DragMove();
    }
    
    private void PetSprite_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Don't open a second one if already open
        if (_functionWindow != null && _functionWindow.IsVisible)
            return;

        // Position the menu near the pet
        _functionWindow = new FunctionWindow(this);
        _functionWindow.Left = this.Left - 230;   // appears to the right
        _functionWindow.Top  = this.Top - 290;           // slightly above

        _functionWindow.Show();
        e.Handled = true;  // prevent event bubbling
    }
    
    private void StartGroundBehaviour()
    {
        CompositionTarget.Rendering += GroundTick;
    }

    private void StopGroundBehaviour()
    {
        CompositionTarget.Rendering -= GroundTick;
    }
    
    private DateTime _lastGroundTickTime = DateTime.Now;

    private void GroundTick(object? sender, EventArgs e)
    {
        if (_currentState == PetState.Dragging || _currentState == PetState.Falling)
            return;

        // Delta time is for making speed framerate-independent
        var now = DateTime.Now;
        double deltaMs = (now - _lastGroundTickTime).TotalMilliseconds;
        _lastGroundTickTime = now;
        double deltaFactor = deltaMs / 16.0;   // 1.0 = baseline at 60fps

        var workArea = SystemParameters.WorkArea;

        if (DateTime.Now >= _nextStateChange)
            PickNextState(workArea);

        if (_currentState == PetState.WalkLeft)
        {
            this.Left -= WalkSpeed * deltaFactor;
            if (this.Left <= 0)
            {
                this.Left = 0;
                PickNextState(workArea);
            }
        }
        else if (_currentState == PetState.WalkRight)
        {
            this.Left += WalkSpeed * deltaFactor;
            if (this.Left >= workArea.Width - this.Width)
            {
                this.Left = workArea.Width - this.Width;
                PickNextState(workArea);
            }
        }
        // else if (_currentState == PetState.Jumping)
        // {
        //     double floor = workArea.Bottom - this.Height;
        //
        //     if (this.Top < floor) return;
        //
        //     _currentState = PetState.Falling;
        //     StopGroundBehaviour();
        //
        //     _velocityY = -12.0;
        //     StartFalling();
        // }
    }
    
    // private void Jump()
    // {
    //     var workArea = SystemParameters.WorkArea;
    //     double floor = workArea.Bottom - this.Height;
    //     
    //     if (this.Top < floor)
    //     {
    //         Console.WriteLine("No Jump");
    //         return;
    //     }
    //
    //     _currentState = PetState.Falling;
    //     StopGroundBehaviour();
    //
    //     _velocityY = -12.0;
    //     StartFalling();
    // }

    private void PickNextState(Rect workArea)
    {
        int roll = _random.Next(100);

        if (roll < 40)
        {
            _currentState = PetState.Idle;
            _nextStateChange = DateTime.Now.AddSeconds(_random.Next(1, 4));
        }
        else if (roll < 75)
        {
            _currentState = PetState.WalkLeft;
            _nextStateChange = DateTime.Now.AddSeconds(_random.Next(1, 3));
        }
        else if (roll < 85)
        {
            _currentState = PetState.WalkRight;
            _nextStateChange = DateTime.Now.AddSeconds(_random.Next(1, 3));
        }
        // else
        // {
        //     Jump();
        //     Console.WriteLine("Jump");
        // }
    }
    
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Console.WriteLine("paused");
            _currentState = PetState.Idle;
            StopGroundBehaviour();
        }
        else
        {
            Console.WriteLine("resume");
            _lastGroundTickTime = DateTime.Now;
            _lastRenderTime = DateTime.Now; 
            _currentState = PetState.Idle;
            StartGroundBehaviour();
        }
    }

    public bool IsPaused => _isPaused;
    
    private void SetAnimation(string gifPath)
    {
        // Clear static source first
        PetSprite.Source = null;
        // Set animated GIF
        var image = new BitmapImage(new Uri(gifPath, UriKind.Relative));
        ImageBehavior.SetAnimatedSource(PetSprite, image);
        ImageBehavior.SetRepeatBehavior(PetSprite, RepeatBehavior.Forever);
    }

    private void SetStaticImage(string pngPath)
    {
        // Clear animated source first
        ImageBehavior.SetAnimatedSource(PetSprite, null);
        // Set static PNG
        PetSprite.Source = new BitmapImage(new Uri(pngPath, UriKind.Relative));
    }
}