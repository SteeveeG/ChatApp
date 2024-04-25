namespace ChatApp.CustomMessageBox;

public class CustomMessageBoxViewModel : ViewModelBase
{
    private string message;

    public CustomMessageBoxViewModel()
    {
        
    }
    public CustomMessageBoxViewModel(string message)
    {
        Message = message;
    }

    public string Message
    {
        get => message;
        set
        {
            if (value == message) return;
            message = value;
            OnPropertyChanged();
        }
    }
}