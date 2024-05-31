using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatApp.Settings.SettingsInput;

public partial class SettingsInputControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
        typeof(SettingsInputViewModel), typeof(SettingsInputControl),
        new PropertyMetadata(default(SettingsInputViewModel)));

    public SettingsInputControl()
    {
        InitializeComponent();
    }

    public SettingsInputViewModel ViewModel
    {
        get => (SettingsInputViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void RemoveAddWatermark(object sender, TextChangedEventArgs e)
    {
        SetWatermark(Watermark, NewName,
            string.IsNullOrEmpty(NewName.Text) ? Visibility.Visible : Visibility.Hidden,
            new SolidColorBrush
                { Color = string.IsNullOrEmpty(NewName.Text) ? Colors.Transparent : Color.FromRgb(36, 51, 66) });
        ViewModel.IsChanged = !string.IsNullOrEmpty(NewName.Text);
    }

    private void SetWatermark(TextBlock watermark, TextBox textBox
        , Visibility visibility, SolidColorBrush color)
    {
        watermark.Visibility = visibility;
        textBox.Background = color;
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Label.Contains("Log Out"))
        {
            var loginregister = new LoginRegisterMenu.LoginRegisterMenu();
            loginregister.Show();
            var parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }
        else if (ViewModel.Label.Contains("Account"))
        {
            if (await ViewModel.DeletFunc())
            {
                var parentWindow = Window.GetWindow(this);
                parentWindow.Close();
            }
        }
    }
}