namespace ChatApp.Chat.Header;

public class HeaderViewModel : ViewModelBase
{
    private string name;
    private ChatViewModel ChatViewModel { get; set; }
    public HeaderViewModel(ChatViewModel chatViewModel, string header)
    {
        ChatViewModel = chatViewModel;
        Name = header;
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