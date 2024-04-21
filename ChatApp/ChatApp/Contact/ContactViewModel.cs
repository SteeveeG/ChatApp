using System.Collections.ObjectModel;
using ChatApp.Contact.EditContact;
using Library.Model;

namespace ChatApp.Contact;

public class ContactViewModel : ViewModelBase
{
    private ObservableCollection<EditContactViewModel> contacts;
    private string ownUserId;
    private string ownUsername;


    private MainViewModel MainViewModel { get; set; }

    public AccUser AccUser { get; set; }
    public ContactViewModel(MainViewModel mainViewModel, AccUser accUser = null 
        )
    {
     AccUser = accUser ??= new AccUser()
        {
            UserId = "-1",
            Username = "-1",
            Password = "-1"
        };
        OwnUserId = accUser.UserId;
        OwnUsername = accUser.Username;
        MainViewModel = mainViewModel;
        Contacts= new ObservableCollection<EditContactViewModel>();
    }
   
    public void UpdateContactList()
    {
        
    }
    public void Delete(EditContactViewModel contact)
    {
        Contacts.Remove(Contacts[Contacts.IndexOf(contact)]);
        MainViewModel.DeleteContact(contact.UserId);
    }
    


    public ObservableCollection<EditContactViewModel> Contacts
    {
        get => contacts;
        set
        {
            if (Equals(value, contacts)) return;
            contacts = value;
            OnPropertyChanged();
        }
    }

    public string OwnUserId
    {
        get => ownUserId;
        set
        {
            if (value == ownUserId) return;
            ownUserId = $"Own User-Id: {value}";
            OnPropertyChanged();
        }
    }

    public string OwnUsername
    {
        get => ownUsername;
        set
        {
            if (value == ownUsername) return;
            ownUsername = $"Own Username: {value}";
            OnPropertyChanged();
        }
    }
}