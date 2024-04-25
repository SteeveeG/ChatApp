using System.Collections.ObjectModel;
using System.Windows;
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
    public DelegateCommand CopyUserIdCommand { get; set; }
    private string userId;
    private string copyText;

    public ContactViewModel(MainViewModel mainViewModel, AccUser accUser = null)
    {
        AccUser = accUser ??= new AccUser()
        {
            UserId = "-1",
            Username = "-1",
            Password = "-1"
        };
        OwnUserId = accUser.UserId;
        userId = accUser.UserId;
        OwnUsername = accUser.Username;
        MainViewModel = mainViewModel;
        Contacts = new ObservableCollection<EditContactViewModel>();
        CopyUserIdCommand = new DelegateCommand(CopyUserId);
        CopyText = "Copy User-Id";
    }

    private void CopyUserId()
    {
      Clipboard.SetText(userId);
      CopyText = "Copied";
      _ = OldText();

    }
    private async Task OldText()
    {
        await Task.Delay(2000);  
        CopyText = "Copy User-Id";
    }

    public void UpdateContactList(Library.Model.Contact contact)
    {
        MainViewModel.UpdateContacts(contact);
    }

    public async void Delete(EditContactViewModel contact)
    {
        if (!await MainViewModel.DeleteContact(contact.UserId))
        {
            return;
        }
        Contacts.Remove(Contacts[Contacts.IndexOf(contact)]);
        MainViewModel.ChatListViewModel.RemoveContact(contact);
        MainViewModel.ChatViewModel.ClearFont();
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

    public string CopyText
    {
        get => copyText;
        set
        {
            if (value == copyText) return;
            copyText = value;
            OnPropertyChanged();
        }
    }
}