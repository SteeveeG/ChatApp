using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Windows;
using ChatApp.ApiHandler;
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
    private string pbSource;

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
        PbSource = @$"../Pb/pb.{AccUser.ProfilePicType}";
    }

    private void CopyUserId()
    {
      Clipboard.SetText(userId);
      CopyText = "Copied";
      OldText();

    }
    private async void OldText()
    {
        await Task.Delay(2000);  
        CopyText = "Copy User-Id";
    }

    public void UpdateContactList(Library.Model.Contact contact)
    {
        MainViewModel.UpdateContacts(contact , false);
    }

    public async void Delete(EditContactViewModel contact)
    {
        if (!await MainViewModel.DeleteContact(contact.ContactUserId))
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

    public string PbSource
    {
        get => pbSource;
        set
        {
            if (value == pbSource) return;
            pbSource = value;
            OnPropertyChanged();
        }
    }
}