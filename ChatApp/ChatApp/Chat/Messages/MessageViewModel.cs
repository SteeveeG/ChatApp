using System.Windows;
using System.Windows.Media;

namespace ChatApp.Chat.Messages;

public class MessageViewModel : ViewModelBase
{
    private int colum;
    private string message;
    private Brush background;
    private Thickness padding;
    private double cornerRadiusBl;
    private double cornerRadiusBr;
    private CornerRadius cornerRadius;
    private SolidColorBrush OwnMessage;
    private SolidColorBrush NotOwnMessage;
    private HorizontalAlignment horizontalAlignment;
    public MessageViewModel(string message, bool isUserMessage)
    {
        Message = message;
        Background = new SolidColorBrush();

        OwnMessage = (SolidColorBrush)new BrushConverter().ConvertFrom("#641185");
        NotOwnMessage = (SolidColorBrush)new BrushConverter().ConvertFrom("#213c57");

        Background = isUserMessage ? OwnMessage : NotOwnMessage;
        cornerRadiusBl = isUserMessage ? 12.0 : 0;
        cornerRadiusBr = isUserMessage ? 0 : 12.0;
        CornerRadius = new CornerRadius
        {
            BottomRight = cornerRadiusBr,
            BottomLeft = cornerRadiusBl,
            TopRight = 12.0,
            TopLeft = 12.0,
        };
        Colum = isUserMessage ? 2 : 0;
        HorizontalAlignment = isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        Padding = new Thickness
        {
            Left = cornerRadiusBl * 8,
            Right = cornerRadiusBr * 8,
            Top = 0,
            Bottom = 0,
        };
    }
    public Thickness Padding
    {
        get => padding;
        set
        {
            if (value.Equals(padding)) return;
            padding = value;
            OnPropertyChanged();
        }
    }
    public string Message
    {
        get => message;
        set
        {
            if (value == message) return;
            message = value;
            OnPropertyChanged();
        }
    }
    public Brush Background
    {
        get => background;

        set
        {
            if (Equals(value, background)) return;
            background = value;
            OnPropertyChanged();
        }
    }
    public CornerRadius CornerRadius
    {
        get => cornerRadius;

        set
        {
            if (value.Equals(cornerRadius)) return;
            cornerRadius = value;
            OnPropertyChanged();
        }
    }
    public int Colum
    {
        get => colum;
        set
        {
            if (value == colum) return;
            colum = value;
            OnPropertyChanged();
        }
    }
    public HorizontalAlignment HorizontalAlignment
    {
        get => horizontalAlignment;
        set
        {
            if (value == horizontalAlignment) return;
            horizontalAlignment = value;
            OnPropertyChanged();
        }
    }
}