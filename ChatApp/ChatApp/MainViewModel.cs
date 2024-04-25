using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using ChatApp.ApiHandler;
using ChatApp.Chat;
using ChatApp.ChatList;
using ChatApp.ChatList.ChatListItem;
using ChatApp.Contact;
using ChatApp.Contact.EditContact;
using ChatApp.HomeNavBar;
using ChatApp.Settings;
using Library.Model;

namespace ChatApp;

//TODO Ids for Call Encoden
//TODO Context ContactID ist die UserId von deinem Contact :D
public class MainViewModel : ViewModelBase
{
    private List<Library.Model.Contact> contacts;
    private AccUser user;
    private List<List<Message>> messages;
    public HomeNavbarViewModel HomeNavbarViewModel { get; set; }
    public ChatListViewModel ChatListViewModel { get; set; }
    public ChatViewModel ChatViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
    public ContactViewModel ContactViewModel { get; set; }
    public Func<int, List<Message>> GetMessage { get; set; }
    public Func<string, Task<string>> GetChatIdFunc { get; set; }

    public MainViewModel()
    {
        /*Init MainViewModel*/

        ChatViewModel = new ChatViewModel();
        ChatListViewModel = new ChatListViewModel(ChatViewModel);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel(this);
        ContactViewModel = new ContactViewModel(this);
    }

    public MainViewModel(AccUser acc)
    {
        GetChatIdFunc = GetChatId;
        GetMessage = GetMessages;
        Messages = new List<List<Message>>();
        user = acc;
        ChatViewModel = new ChatViewModel(acc);
        ChatListViewModel = new ChatListViewModel(ChatViewModel, acc.UserId, GetMessage, GetChatIdFunc);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel(this,acc.UserId);
        ContactViewModel = new ContactViewModel(this, acc);
        GetContacts();
    }

    private async Task<string> GetChatId(string ContactId)
    {
        var chatId = await GetChatId(ContactId, user.UserId);
        return chatId;
    }

    private List<Message> GetMessages(int index)
    {
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
        var result = await ApiGet.GetApiIn<List<Message>>($"Sql/GetMessages?chatId={convertedchatId}");
        Messages.Add(result ?? new List<Message>());
    }

    private async Task<string> GetChatId(string contactId, string userId)
    {
        var convertedcontactId = HttpUtility.UrlEncode(contactId);
        var converteduserId = HttpUtility.UrlEncode(userId);
        var chat = await ApiGet.GetApiIn<Library.Model.Chat>($"Sql/GetChat?userId={converteduserId}&contactId={convertedcontactId}");
        return chat.ChatId;
    }

    private async void GetContacts()
    {
        var converteduserId = HttpUtility.UrlEncode(user.UserId);
        Contacts = await ApiGet.GetApiIn<List<Library.Model.Contact>>($"Sql/GetUserContacts?userId={converteduserId}");
        foreach (var contact in Contacts)
        {
            ChatListViewModel.List.Add(new ChatListItemViewModel(contact, ChatListViewModel));
            ContactViewModel.Contacts.Add(new EditContactViewModel(contact.ContactUsername, contact.ContactId,
                ContactViewModel.Delete));
            var chatId = await GetChatId(contact.ContactId, user.UserId);
            //Todo Returns Null 
            await AddChatMessages(chatId);
        }
    }


    public async void UpdateContacts(Library.Model.Contact contact)
    {
        await ControlCreateChatId(contact);
        ChatListViewModel.List.Add(new ChatListItemViewModel(contact, ChatListViewModel));
        ContactViewModel.Contacts.Add(new EditContactViewModel(contact.ContactUsername, contact.ContactId,
            ContactViewModel.Delete));
        Messages.Add(new List<Message>());
    }

    private async Task ControlCreateChatId(Library.Model.Contact contact)
    {
        if (await GetChatId(contact.ContactId) != null)
        {
            return;
        }
        var convertedUserId = HttpUtility.UrlEncode(contact.UserId);
        var convertedContactId = HttpUtility.UrlEncode(contact.ContactId);
        await ApiGet.GetApiIn<Library.Model.Chat>($"Sql/CreateChat?userId={convertedUserId}&contactId={convertedContactId}");
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