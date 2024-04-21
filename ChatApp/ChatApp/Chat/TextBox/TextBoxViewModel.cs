using System.Windows.Media;
using ChatApp.Chat.Messages;

namespace ChatApp.Chat.TextBox;

public class TextBoxViewModel : ViewModelBase
{
    private string message;
    private SolidColorBrush foreground;
    private SolidColorBrush borderBrush;
    private ChatViewModel ChatViewModel { get; set; }
    public DelegateCommand SendMessageCommand { get; set; }

    public TextBoxViewModel(ChatViewModel chatViewModel)
    {
        SendMessageCommand = new DelegateCommand(SendMessage);
        ChatViewModel = chatViewModel;
        Foreground = Brushes.Gray;
        BorderBrush = Brushes.LightSlateGray;
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(Message))
        {
            BorderBrush = Brushes.Red;
            return;
        }
        BorderBrush = Brushes.LightSlateGray;
        ChatViewModel.Messages.Add(new MessageViewModel(Message, true));
        Message = string.Empty;
    }

    public void HoverForeground(bool isMouseOver)
    {
        Foreground = isMouseOver ? Brushes.GhostWhite : Brushes.Gray;
    }


    public string Message
    {
        get => message;
        set
        {
            if (value == message) return;
            message = value;
            HoverForeground(!string.IsNullOrEmpty(message));
            if (BorderBrush == Brushes.Red && !string.IsNullOrEmpty(message))
            {
                BorderBrush = Brushes.LightSlateGray;
            }

            OnPropertyChanged();
        }
    }

    public SolidColorBrush Foreground
    {
        get => foreground;
        set
        {
            if (Equals(value, foreground)) return;
            foreground = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush BorderBrush
    {
        get => borderBrush;
        set
        {
            if (Equals(value, borderBrush)) return;
            borderBrush = value;
            OnPropertyChanged();
        }
    }
}