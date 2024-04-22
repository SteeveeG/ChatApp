using System.Collections.ObjectModel;
using ChatApp.Chat;
using ChatApp.Chat.Messages;
using ChatApp.ChatList.ChatListItem;
using Library.Model;

namespace ChatApp.ChatList;

public class ChatListViewModel : ViewModelBase
{
    private ObservableCollection<ChatListItemViewModel> list;
    private ChatViewModel ChatViewModel { get; set; }
    private string userId;
    private Func<int, List<Message>> getMessage;
    private Func<string, string> getChatId;

    public ChatListViewModel(ChatViewModel chatViewModel)
    {
        List = new ObservableCollection<ChatListItemViewModel>();
        ChatViewModel = chatViewModel;
    }

    public ChatListViewModel(ChatViewModel chatViewModel, string userId, Func<int, List<Message>> getMessage
    , Func<string, string> getChatId)
    {
        this.getChatId = getChatId;
        this.getMessage = getMessage;
        List = new ObservableCollection<ChatListItemViewModel>();
        this.userId = userId;
        ChatViewModel = chatViewModel;
    }
    public void Click(int index)
    {
        ChatViewModel.HeaderViewModel.Name = list[index].Name;
        var chatId = getChatId(list[index].ContactId);
        ChatViewModel.ChatId = chatId;
        var messages = getMessage(index);
        ChatViewModel.Messages = new ObservableCollection<MessageViewModel>();
        if (messages == null)
        {
            return;
        }

        foreach (var message in messages)
        {
            ChatViewModel.Messages.Add(new MessageViewModel(message.Content, userId == message.UserId));
        }
    }
    
    public ObservableCollection<ChatListItemViewModel> List
    {
        get => list;
        set
        {
            if (Equals(value, list)) return;
            list = value;
            OnPropertyChanged();
        }
    }
}