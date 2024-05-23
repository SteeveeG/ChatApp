using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using System.Windows;
using ChatApp.ApiHandler;
using ChatApp.Chat;
using ChatApp.Chat.Messages;
using ChatApp.ChatList;
using ChatApp.ChatList.ChatListItem;
using ChatApp.Contact;
using ChatApp.Contact.EditContact;
using ChatApp.HomeNavBar;
using ChatApp.Settings;
using Library.Model;
using Type = Library.Enum.Type;

namespace ChatApp;

public class MainViewModel : ViewModelBase
{
    private List<Library.Model.Contact> contacts;
    private AccUser user;
    private List<List<Message>> messages;
    private int curentChatIndex;
    private string CurrentChatId;
    public HomeNavbarViewModel HomeNavbarViewModel { get; set; }
    public ChatListViewModel ChatListViewModel { get; set; }
    public ChatViewModel ChatViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
    public ContactViewModel ContactViewModel { get; set; }
    public Func<int, List<Message>> GetMessage { get; set; }
    public Func<string, Task<string>> GetChatIdFunc { get; set; }

    public MainViewModel()
    {
        ChatViewModel = new ChatViewModel();
        ChatListViewModel = new ChatListViewModel(ChatViewModel);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel(this);
        ContactViewModel = new ContactViewModel(this);
    }

    public MainViewModel(AccUser acc)
    {
        SetPb(ref acc);
        user = acc;
        curentChatIndex = 0;
        GetChatIdFunc = GetChatId;
        GetMessage = GetMessages;
        Messages = new List<List<Message>>();
        ChatViewModel = new ChatViewModel(SetMessage, acc);
        ChatListViewModel = new ChatListViewModel(ChatViewModel, acc.UserId, GetMessage, GetChatIdFunc);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel(this, acc);
        ContactViewModel = new ContactViewModel(this, acc);
        GetContacts();
        ConnectToGrpc();
    }

    private void SetPb(ref AccUser acc)
    {
        var list =new List<byte>();
        var str = string.Empty;
        foreach (var bytechar in acc.ProfilePicByte)
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
        acc.ProfilePicByte= string.Empty;
        var array = list.ToArray();
        list.Clear();
        
         switch (acc.ProfilePicType)
         {
             case "png":
                 File.WriteAllBytes(@"..\..\..\Pb\pb.png", array);
                 break;
             case "jpg":
                 File.WriteAllBytes(@"..\..\..\Pb\pb.jpg", array);
                 break;
         }
    }

    private void ConnectToGrpc()
    {
        Task.Run(async () =>
        {
            await Task.Run(() => Library.GrpcHandler.GrpcHandler.OpenStream(SetNewInput, user.UserId));
        });
    }

    private void SetNewInput(string json)
    {
        var sub = JsonSerializer.Deserialize<Subscriber>(json);
        switch (sub.Type)
        {
            case Type.CreatedContact:
                Application.Current.Dispatcher.Invoke(() => UpdateContacts(sub.Contact, true));
                break;
            case Type.Message:
                Application.Current.Dispatcher.Invoke(() => UpdateMessages(sub.Message));
                break;
            case Type.DeleteContact:
                break;
            case Type.DeleteAccount:
                break;
            default:
                return;
        }
    }

    private void UpdateMessages(Message message)
    {
        if (message.ChatId == CurrentChatId)
        {
            ChatViewModel.Messages.Add(new MessageViewModel(message.Content, false));
        }

        var index = ChatListViewModel.List.IndexOf(
            ChatListViewModel.List.FirstOrDefault(p => p.ContactId == message.UserId));
        Messages[index].Add(message);
        ChatListViewModel.List[index].LastMessage = message.Content;
        ChatListViewModel.List[index].LastMessageTime = message.Time;
    }

    private void SetMessage(MessageViewModel message)
    {
        var time = DateTime.Now;
        Messages[curentChatIndex].Add(new Message
        {
            ChatId = CurrentChatId,
            Content = message.Message,
            Time = time,
            UserId = user.UserId
        });
        ChatListViewModel.List[curentChatIndex].LastMessage = message.Message;
        ChatListViewModel.List[curentChatIndex].LastMessageTime = time;
    }

    private async Task<string> GetChatId(string ContactId)
    {
        var chatId = await GetChatId(ContactId, user.UserId);
        CurrentChatId = chatId;
        return chatId;
    }

    private List<Message> GetMessages(int index)
    {
        curentChatIndex = index;
        return Messages[index];
    }

    public async Task<bool> DeleteContact(string contactId)
    {
        contactId = HttpUtility.UrlEncode(contactId);
        var convertedUserId = HttpUtility.UrlEncode(user.UserId);
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        try
        {
            await client.DeleteAsync($"Sql/DeleteContact?contactId={contactId}&userId={convertedUserId}");
        }
        catch
        {
            return false;
        }

        return true;
    }

    private async Task AddChatMessages(string chatId)
    {
        var convertedchatId = HttpUtility.UrlEncode(chatId);
        var result = await Api.GetIn<List<Message>>($"Sql/GetMessages?chatId={convertedchatId}");
        Messages.Add(result ?? new List<Message>());
    }

    private async Task<string> GetChatId(string contactId, string userId)
    {
        var convertedcontactId = HttpUtility.UrlEncode(contactId);
        var converteduserId = HttpUtility.UrlEncode(userId);
        var chat = await Api.GetIn<Library.Model.Chat>(
            $"Sql/GetChat?userId={converteduserId}&contactId={convertedcontactId}");
        return chat.ChatId;
    }

    private async void GetContacts()
    {
        var converteduserId = HttpUtility.UrlEncode(user.UserId);
        Contacts = await Api.GetIn<List<Library.Model.Contact>>($"Sql/GetUserContacts?userId={converteduserId}");
        var Usernames = await Api.GetIn<List<string>>($"Sql/GetContactNames?userId={converteduserId}");
        if (Contacts == null)
        {
            return;
        }

        for (var i = 0; i < Contacts.Count; i++)
        {
            var id = Contacts[i].UserId == user.UserId ? Contacts[i].CreatedContactUserId : Contacts[i].UserId;
            ChatListViewModel.List.Add(new ChatListItemViewModel(Contacts[i], ChatListViewModel, Usernames[i], id));
            ContactViewModel.Contacts.Add(new EditContactViewModel(Usernames[i], id,
                ContactViewModel.Delete));
            var chatId = await GetChatId(id, user.UserId);
            //Todo Returns Null 
            await AddChatMessages(chatId);
        }
    }


    public async void UpdateContacts(Library.Model.Contact contact, bool IsGrpc)
    {
        var id = contact.UserId == user.UserId ? contact.CreatedContactUserId : contact.UserId;
        if (!IsGrpc)
        {
            await ControlCreateChatId(contact);
        }

        var name = await Api.GetIn<string>($"Sql/GetUsername?userId={id}");
        ChatListViewModel.List.Add(new ChatListItemViewModel(contact, ChatListViewModel, name, id));
        ContactViewModel.Contacts.Add(new EditContactViewModel(name, id, ContactViewModel.Delete));
        Messages.Add(new List<Message>());
    }


    private async Task ControlCreateChatId(Library.Model.Contact contact)
    {
        if (await GetChatId(contact.UserId) != null)
        {
            return;
        }

        var convertedUserId = HttpUtility.UrlEncode(contact.CreatedContactUserId);
        var convertedContactId = HttpUtility.UrlEncode(contact.UserId);
        await Api.GetIn<Library.Model.Chat>(
            $"Sql/CreateChat?userId={convertedUserId}&contactId={convertedContactId}");
    }

    public void NewName(string newUsername)
    {
        ContactViewModel.OwnUsername = newUsername;
    }


    public List<List<Message>> Messages
    {
        get => messages;
        set
        {
            if (Equals(value, messages)) return;
            messages = value;
            OnPropertyChanged();
        }
    }

    private List<Library.Model.Contact> Contacts
    {
        get => contacts;
        set
        {
            if (Equals(value, contacts)) return;
            contacts = value;
            OnPropertyChanged();
        }
    }
}