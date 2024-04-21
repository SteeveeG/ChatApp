using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ChatApp.LoginRegisterMenu.RegisterControl;

public partial class RegisterControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
        typeof(RegisterViewModel), typeof(RegisterControl), new PropertyMetadata(default(RegisterViewModel)));

    public RegisterControl()
    {
        InitializeComponent();
    }

    public RegisterViewModel ViewModel
    {
        get => (RegisterViewModel)GetValue(ViewModelProperty);
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