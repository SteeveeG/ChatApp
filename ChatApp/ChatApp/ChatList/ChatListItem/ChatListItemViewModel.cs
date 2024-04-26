namespace ChatApp.ChatList.ChatListItem;

public class ChatListItemViewModel : ViewModelBase
{
    private DateTime lastMessageTime;
    private string lastMessage;
    private string name;
    private string contactId;
    public ChatListViewModel ChatListViewModel { get; set; }

    public ChatListItemViewModel(Library.Model.Contact contact, ChatListViewModel chatListViewModel)
    {
        ChatListViewModel = chatListViewModel;
        Name = contact.ContactUsername;
        LastMessage =  contact.LastMessage;
        LastMessageTime =  contact.LastMessageTime;
        ContactId =  contact.UserId;
    }


    public string ContactId
    {
        get => contactId;
        set
        {
            if (value == contactId) return;
            contactId = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => name;
        set
        {
            if (value == name) return;
            name = value;
            OnPropertyChanged();
        }
    }

    public string LastMessage
    {
        get => lastMessage;
        set
        {
            if (value == lastMessage) return;
            lastMessage = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastMessageTime
    {
        get => lastMessageTime;
        set
        {
            if (value == lastMessageTime) return;
            lastMessageTime = value;
            OnPropertyChanged();
        }
    }
}