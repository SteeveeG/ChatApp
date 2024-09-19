namespace ChatApp.Contact.EditContact;

public class EditContactViewModel : ViewModelBase
{
    private string contactUserId;
    private string name;
    public Action<EditContactViewModel> Action { get; set; }
    public EditContactViewModel(string name,string contactUserId , Action<EditContactViewModel> action)
    {
        ContactUserId = contactUserId;
        Name = name;
        Action = action;
    }
    public string ContactUserId
    {
        get => contactUserId;
        set
        {
            if (value == contactUserId) return;
            contactUserId = value;
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
}