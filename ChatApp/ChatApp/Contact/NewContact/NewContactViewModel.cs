using System.Net.Http;
using System.Web;
using ChatApp.ApiHandler;
using Library.Model;

namespace ChatApp.Contact.NewContact;

public class NewContactViewModel : ViewModelBase
{
    public DelegateCommand AddNewContactCommand { get; set; }

    public AccUser AccUser { get; set; }

    private Action<Library.Model.Contact> action;
    private string newContactId;

    public NewContactViewModel()
    {
        AddNewContactCommand = new DelegateCommand(AddNewContact);
    }

    public NewContactViewModel(AccUser user, Action<Library.Model.Contact> action)
    {
        AddNewContactCommand = new DelegateCommand(AddNewContact);
        AccUser = user;
        this.action = action;
    }

    private async void AddNewContact()
    {
        var contact = new Library.Model.Contact(); 
        if (string.IsNullOrEmpty(NewContactId))
        {
            return;
        }
 
 
        contact = await Api.Post<Library.Model.Contact>($"Sql/CreateContact?userId={NewContactId}&createdContactUserId={AccUser.UserId}",
            new StringContent(string.Empty));
        action(contact);
        NewContactId = string.Empty;
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