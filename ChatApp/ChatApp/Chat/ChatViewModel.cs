using System.Collections.ObjectModel;
using ChatApp.Chat.Header;
using ChatApp.Chat.Messages;
using ChatApp.Chat.TextBox;

namespace ChatApp.Chat;

public
 class ChatViewModel : ViewModelBase
{
    private ObservableCollection<MessageViewModel> messages;
    public HeaderViewModel HeaderViewModel { get; set; }
    public TextBoxViewModel TextBoxViewModel { get; set; }
    public ChatViewModel()
    {
        Messages = new ObservableCollection<MessageViewModel>();
        HeaderViewModel = new HeaderViewModel(this , "");
        TextBoxViewModel = new TextBoxViewModel(this);
    }
    
    public ObservableCollection<MessageViewModel> Messages
    {
        get => messages;
        set
        {
            if (Equals(value, messages)) return;
            messages = value;
            OnPropertyChanged();
        }
    }
}