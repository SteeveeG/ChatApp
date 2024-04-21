using System.Windows;
using System.Windows.Controls;

namespace ChatApp.Chat.Header;

public partial class HeaderControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel),
            typeof(HeaderViewModel), 
            typeof(HeaderControl), 
            new PropertyMetadata(default(HeaderViewModel)));

    public HeaderControl()
    {
        InitializeComponent();
    }

    public HeaderViewModel ViewModel
    {
        get => (HeaderViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}