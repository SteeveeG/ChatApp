using System.Windows;
using System.Windows.Controls;

namespace ChatApp.Chat;

public partial class ChatControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ChatViewModel), typeof(ChatControl), new PropertyMetadata(default(ChatViewModel)));

    public ChatControl()
    {
        InitializeComponent();
    }

    public ChatViewModel ViewModel
    {
        get => (ChatViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}