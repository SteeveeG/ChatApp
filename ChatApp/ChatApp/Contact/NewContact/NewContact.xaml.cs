using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp.Contact.NewContact;

public partial class NewContact : Window
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(NewContactViewModel), typeof(NewContact), new PropertyMetadata(default(NewContactViewModel)));

    public NewContact()
    {
        InitializeComponent();
    }

    public NewContactViewModel ViewModel
    {
        get => (NewContactViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void RemoveAddWatermark(object sender, TextChangedEventArgs e)
    {
        SetWatermark(UserIdWatermark, UserId,
            string.IsNullOrEmpty(UserId.Text) ? Visibility.Visible : Visibility.Hidden,
            new SolidColorBrush
                { Color = string.IsNullOrEmpty(UserId.Text) ? Colors.Transparent :
                    Color.FromRgb(36, 51, 66) });
    }
    private void SetWatermark(TextBlock watermark, TextBox textBox
        , Visibility visibility, SolidColorBrush color)
    {
        watermark.Visibility = visibility;
        textBox.Background = color;
    }

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Move(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}