using System.Windows;

namespace ChatApp.Chat.Messages;

public partial class MessageControl  
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), 
            typeof(MessageViewModel),
            typeof(MessageControl), 
            new PropertyMetadata(default(MessageViewModel)));

    public MessageControl()
    {
        InitializeComponent();
    }

    public MessageViewModel ViewModel
    {
        get => (MessageViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}