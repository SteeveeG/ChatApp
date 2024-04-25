using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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

    private Action<MessageViewModel> action;

    public ChatViewModel(Action<MessageViewModel> action = null, AccUser acc = null)
    {
        this.action = action;
        accUser = acc;
        Messages = new ObservableCollection<MessageViewModel>();
        HeaderViewModel = new HeaderViewModel(this, "");
        TextBoxViewModel = new TextBoxViewModel(this, accUser);
    }

    public async Task SendMessage(Message message)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.PostAsJsonAsync("Sql/AddMessage", message);
        if (response.IsSuccessStatusCode)
        {
            Messages.Add(new MessageViewModel(message.Content, true));
            action(new MessageViewModel(message.Content, true));
        }
    }

    public void ClearFont()
    {
        Messages.Clear();
        HeaderViewModel.Name = string.Empty;
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