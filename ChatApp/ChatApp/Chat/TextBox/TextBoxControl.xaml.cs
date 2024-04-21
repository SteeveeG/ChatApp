using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatApp.Chat.TextBox;

public partial class TextBoxControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
        typeof(TextBoxViewModel), typeof(TextBoxControl), new PropertyMetadata(default(TextBoxViewModel)));

    public TextBoxControl()
    {
        InitializeComponent();
    }

    public TextBoxViewModel ViewModel
    {
        get => (TextBoxViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void btnMouse(object sender, MouseEventArgs e)
    {
       ViewModel.HoverForeground(((Button)sender).IsMouseOver);
    }

    private void OnKeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            ViewModel.SendMessage();
        }
    }

    private void Textchanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.Message = ((System.Windows.Controls.TextBox)sender).Text;
    }
}