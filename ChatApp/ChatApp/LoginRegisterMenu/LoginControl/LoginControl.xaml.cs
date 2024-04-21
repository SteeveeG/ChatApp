using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatApp.LoginRegisterMenu.LoginControl;

public partial class LoginControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
        typeof(LoginViewModel), typeof(LoginControl), new PropertyMetadata(default(LoginViewModel)));

    public LoginControl()
    {
        InitializeComponent();
    }

    public LoginViewModel ViewModel
    {
        get => (LoginViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
    
    private void RemoveAddWatermark(object sender, TextChangedEventArgs e)
    {
        if (ViewModel.NameFocused)
        {
            SetWatermark(NameWatermark, Name,
                string.IsNullOrEmpty(Name.Text) ? Visibility.Visible : Visibility.Hidden,
                new SolidColorBrush
                    { Color = string.IsNullOrEmpty(Name.Text) ? Colors.Transparent : Color.FromRgb(36, 51, 66) });
        }
        else if (ViewModel.PasswordFocused)
        {
            SetWatermark(PasswordWatermark, Password,
                string.IsNullOrEmpty(Password.Text) ? Visibility.Visible : Visibility.Hidden,
                new SolidColorBrush
                    { Color = string.IsNullOrEmpty(Password.Text) ? Colors.Transparent : Color.FromRgb(36, 51, 66) });
        }
    }

    private void SetWatermark(TextBlock watermark, TextBox textBox
        , Visibility visibility, SolidColorBrush color)
    {
        watermark.Visibility = visibility;
        textBox.Background = color;
    }


    private void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        SetFocus(Password.IsFocused, Name.IsFocused);
    }

    private void SetFocus(bool passwordValue, bool nameValue)
    {
        ViewModel.PasswordFocused = passwordValue;
        ViewModel.NameFocused = nameValue;
    }
}