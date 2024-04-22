using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Library.Model;

namespace ChatApp.Contact.NewContact;

public class NewContactViewModel : ViewModelBase
{
    public DelegateCommand AddNewContactCommand { get; set; }

    public AccUser AccUser { get; set; }

    private Action action;
    private string newContactId;

    public NewContactViewModel()
    {
        AddNewContactCommand = new DelegateCommand(AddNewContact);
    }

    public NewContactViewModel(AccUser user, Action action)
    {
        AccUser = user;
        this.action = action;
    }

    private async void AddNewContact()
    {
        if (string.IsNullOrEmpty(NewContactId))
        {
            return;
        }
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7049");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.PostAsync($"Sql/AddContact", new StringContent(""));
        if (response.IsSuccessStatusCode)
        {
            var contacts = await response.Content.ReadFromJsonAsync<List<Library.Model.Contact>>();
          
        }
        

        Console.WriteLine("Internal server Error");
    }

    public string NewContactId
    {
        get => newContactId;
        set
        {
            if (value == newContactId) return;
            newContactId = value;
            OnPropertyChanged();
        }
    }
}