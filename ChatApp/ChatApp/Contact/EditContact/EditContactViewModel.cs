namespace ChatApp.Contact.EditContact;

public class EditContactViewModel : ViewModelBase
{
    private string userId;
    private string name;
    public Action<EditContactViewModel> Action { get; set; }
    public string UserIdView => $"User-Id: {UserId}";
    public EditContactViewModel(string name,string userId , Action<EditContactViewModel> action)
    {
        UserId = userId;
        Name = name;
        Action = action;
    }
    public string UserId
    {
        get => userId;
        set
        {
            if (value == userId) return;
            userId = value;
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