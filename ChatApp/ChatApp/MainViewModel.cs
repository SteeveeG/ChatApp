using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.Chat;
using ChatApp.ChatList;
using ChatApp.ChatList.ChatListItem;
using ChatApp.Contact;
using ChatApp.Contact.EditContact;
using ChatApp.HomeNavBar;
using ChatApp.Settings;
using Library.Model;

namespace ChatApp;

//TODO Die Funktion Add New Contact erstellen und eine Liste Mit ChatId/UserId/ContactID erstellen damit nichtjedes mal ein 
//TODO API Call Kommen muss 
//TODO Context ContactID ist die UserId von deinem Contact :D
public class MainViewModel : ViewModelBase
{
    private List<Library.Model.Contact> contacts;
    public HomeNavbarViewModel HomeNavbarViewModel { get; set; }
    public ChatListViewModel ChatListViewModel { get; set; }
    public ChatViewModel ChatViewModel { get; set; }
    public SettingsViewModel SettingsViewModel { get; set; }
    public ContactViewModel ContactViewModel { get; set; }


    private AccUser user;
    private List<List<Message>> messages;
    public Func<int, List<Message>> GetMessage { get; set; }
    public Func<string, string> GetChatIdFunc { get; set; }

    public MainViewModel()
    {
        /*Init MainViewModel*/

        ChatViewModel = new ChatViewModel();
        ChatListViewModel = new ChatListViewModel(ChatViewModel);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel();
        ContactViewModel = new ContactViewModel(this);
    }
    public MainViewModel(AccUser acc)
    {
        GetChatIdFunc =  GetChatId;
        GetMessage = GetMessages;
        Messages = new List<List<Message>>();
        user = acc;
        ChatViewModel = new ChatViewModel(acc);
        ChatListViewModel = new ChatListViewModel(ChatViewModel, acc.UserId, GetMessage ,GetChatIdFunc);
        HomeNavbarViewModel = new HomeNavbarViewModel();
        SettingsViewModel = new SettingsViewModel(acc.UserId);
        ContactViewModel = new ContactViewModel(this, acc);
        GetContacts();
    }

    private string GetChatId(string ContactId)
    {
        return string.Empty;
    }

    private List<Message> GetMessages(int index)
    {
        return Messages[index];
    }
    public void DeleteContact(string userId)
    {
        
    }

    private async Task AddChatMessages(string chatId)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"Sql/GetMessages?chatId={chatId}");
        if (response.IsSuccessStatusCode)
        {
            var chat = await response.Content.ReadFromJsonAsync<List<Message>>();
            Messages.Add(chat);
        }
        else
        {
            Messages.Add(new List<Message>());
        }
    }

    private async Task<string> GetChatId(string contactId, string userId)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"Sql/GetChat?userId={userId}&contactId={contactId}");
        if (response.IsSuccessStatusCode)
        {
            var chat = await response.Content.ReadFromJsonAsync<Library.Model.Chat>();
            return chat.ChatId;
        }

        Console.WriteLine("Internal server Error");
        return "-1";
    }
    private async void GetContacts()
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"Sql/GetContacts?userId={user.UserId}");
        if (response.IsSuccessStatusCode)
        {
            Contacts = await response.Content.ReadFromJsonAsync<List<Library.Model.Contact>>();
            foreach (var contact in Contacts)
            {
                ChatListViewModel.List.Add(new ChatListItemViewModel(contact, ChatListViewModel));
                ContactViewModel.Contacts.Add(new EditContactViewModel(contact.ContactUsername, contact.ContactId,
                    ContactViewModel.Delete));

                var chatId = await GetChatId(contact.ContactId,user.UserId);
                await AddChatMessages(chatId);
            } 
        }
        Console.WriteLine("Internal server Error");
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