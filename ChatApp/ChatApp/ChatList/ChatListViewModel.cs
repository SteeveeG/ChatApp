using System.Collections.ObjectModel;
using System.Web;
using ChatApp.Chat;
using ChatApp.Chat.Messages;
using ChatApp.ChatList.ChatListItem;
using ChatApp.Contact.EditContact;
using Library.Model;
using Api = ChatApp.ApiHandler.Api;

namespace ChatApp.ChatList;

public class ChatListViewModel : ViewModelBase
{
    private ObservableCollection<ChatListItemViewModel> list;
    private ChatViewModel ChatViewModel { get; set; }
    private string userId;
    private Func<int, List<Message>> getMessage;
    private Func<string,  Task<string>> getChatId;

    public ChatListViewModel(ChatViewModel chatViewModel)
    {
        List = new ObservableCollection<ChatListItemViewModel>();
        ChatViewModel = chatViewModel;
    }

    public ChatListViewModel(ChatViewModel chatViewModel, string userId, Func<int, List<Message>> getMessage
    , Func<string, Task<string>> getChatId)
    {
        this.getChatId = getChatId;
        this.getMessage = getMessage;
        List = new ObservableCollection<ChatListItemViewModel>();
        this.userId = userId;
        ChatViewModel = chatViewModel;
    }
    public async void Click(int index)
    {
        var converted =  HttpUtility.UrlEncode(List[index].ContactId);
        var accuser = await Api.GetIn<Tuple<string,string,string>>($"Sql/GetNamesAndPb?userId={converted}");
        ChatViewModel.HeaderViewModel.Name = accuser.Item1;
        var chatId = await getChatId(List[index].ContactId);
        ChatViewModel.ChatId = chatId;
        var messages = getMessage(index);
        ChatViewModel.Messages = new ObservableCollection<MessageViewModel>();
        ChatViewModel.HeaderViewModel.CreatePb(accuser.Item2);
        if (messages == null)
        {
            return;
        }

        ChatViewModel.TextBoxViewModel.Message = string.Empty;
        foreach (var message in messages)
        {
            ChatViewModel.Messages.Add(new MessageViewModel(message.Content, userId == message.UserId));
        }
    }
    public void RemoveContact(EditContactViewModel contact)
    {
        for (var i = 0; i < List.Count; i++)
        {
            if (List[i].ContactId == contact.ContactUserId)
            {
                List.RemoveAt(i);
            }
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