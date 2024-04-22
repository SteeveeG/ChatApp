using System.Collections.ObjectModel;
using ChatApp.Chat.Header;
using ChatApp.Chat.Messages;
using ChatApp.Chat.TextBox;
using Library.Model;

namespace ChatApp.Chat;

public
 class ChatViewModel : ViewModelBase
{
    private ObservableCollection<MessageViewModel> messages;
    public HeaderViewModel HeaderViewModel { get; set; }
    public TextBoxViewModel TextBoxViewModel { get; set; }
    private AccUser accUser;
    private string chatId;

    public ChatViewModel(AccUser acc = null)
    {
        accUser = acc;
        Messages = new ObservableCollection<MessageViewModel>();
        HeaderViewModel = new HeaderViewModel(this , "");
        TextBoxViewModel = new TextBoxViewModel(this , accUser);
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

    public string ChatId
    {
        get => chatId;
        set
        {
            if (value == chatId) return;
            chatId = value;
            OnPropertyChanged();
        }
    }
}