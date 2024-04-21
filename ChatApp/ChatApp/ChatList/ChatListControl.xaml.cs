using System.Windows;
using System.Windows.Input;
using ChatApp.ChatList.ChatListItem;

namespace ChatApp.ChatList;

public partial class ChatListControl
{   
    public static readonly DependencyProperty
        ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(ChatListViewModel),
            typeof(ChatListControl),
            new PropertyMetadata(default(ChatListViewModel)));

    public ChatListControl()
    {
        InitializeComponent();
    }

    public ChatListViewModel ViewModel
    {
        get => (ChatListViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void Click(object sender, MouseButtonEventArgs e)
    {
        var item = (ChatListItemControl)sender;
        var index = ViewModel.List.IndexOf(item.ViewModel);
        ViewModel.Click(index);
    }
}