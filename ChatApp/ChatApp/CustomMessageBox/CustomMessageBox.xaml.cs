using System.Windows;
using System.Windows.Input;

namespace ChatApp.CustomMessageBox;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}