using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp.ChatList.ChatListItem;

public partial class ChatListItemControl  
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), 
            typeof(ChatListItemViewModel), 
            typeof(ChatListItemControl),
            new PropertyMetadata(default(ChatListItemViewModel)));

    public ChatListItemControl()
    {
        InitializeComponent();
    }

    public ChatListItemViewModel ViewModel
    {
        get => (ChatListItemViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
    private void Mouse(object sender, MouseEventArgs e)
    {
        var variable = (Grid)sender;
        Background = variable.IsMouseOver ? new SolidColorBrush(){ Opacity = 1 , Color = new Color(){ A = 1 , R = 57 , B= 77 , G = 99}} : new SolidColorBrush(){ Opacity = 1 , Color = new Color(){ A = 1 , R = 45 , B= 62 , G = 80}};
    }
}