using System.IO;
namespace ChatApp.ChatList.ChatListItem;

public class ChatListItemViewModel : ViewModelBase
{
    private DateTime lastMessageTime;
    private string lastMessage;
    private string name;
    private string contactId;
    private string pbSource;
    private MemoryStream source;
    public ChatListViewModel ChatListViewModel { get; set; }

    public ChatListItemViewModel(Library.Model.Contact contact, ChatListViewModel chatListViewModel, string username, string id, string byteString)
    {
        ChatListViewModel = chatListViewModel;
        Name = username;
        LastMessage =  contact.LastMessage;
        LastMessageTime =  contact.LastMessageTime;
        ContactId =  id;
        Test(byteString);
    }

    private void Test(string byteString)
    {
        var list =new List<byte>();
        var str = string.Empty;
        foreach (var bytechar in byteString)
        {
            if (bytechar != '{' && bytechar != ',')
            {
                str += bytechar;
            }
            else if (bytechar == ',')
            {
                list.Add(Convert.ToByte(str));
                str = string.Empty;
            }
            
        }
        var array = list.ToArray();
        Source = new MemoryStream(array);
    }
    public MemoryStream Source
    {
        get => source;
        set
        {
            if (Equals(value, source)) return;
            source = value;
            OnPropertyChanged();
        }
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