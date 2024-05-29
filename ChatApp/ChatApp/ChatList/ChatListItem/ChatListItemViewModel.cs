using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatApp.ChatList.ChatListItem;

public class ChatListItemViewModel : ViewModelBase
{
    private DateTime lastMessageTime;
    private string lastMessage;
    private string name;
    private string contactId;
    private string pbSource;
    private MemoryStream source;
    private byte[] bytes;
    private ImageSource imageSource;
    public ChatListViewModel ChatListViewModel { get; set; }

    public ChatListItemViewModel(Library.Model.Contact contact, ChatListViewModel chatListViewModel, string username, string id, string byteString)
    {
        ChatListViewModel = chatListViewModel;
        Name = username;
        LastMessage =  contact.LastMessage;
        LastMessageTime =  contact.LastMessageTime;
        ContactId =  id;
        CreatePb(byteString);
    }

    private void CreatePb(string byteString)
    {
        if (string.IsNullOrEmpty(byteString))
        {
            return;
        }
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
        Bytes = list.ToArray();
        using var stream = new MemoryStream(Bytes);
        ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    }

    public ImageSource ImageSource
    {
        get => imageSource;
        set
        {
            if (Equals(value, imageSource)) return;
            imageSource = value;
            OnPropertyChanged();
        }
    }

    private byte[] Bytes
    {
        get => bytes;
        set
        {
            if (Equals(value, bytes)) return;
            bytes = value;
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