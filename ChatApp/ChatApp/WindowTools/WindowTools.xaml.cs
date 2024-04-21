using System.Windows;
using System.Windows.Controls;

namespace ChatApp.WindowTools;

public partial class WindowTools : UserControl
{

    public WindowTools()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        if (parentWindow.WindowState == WindowState.Maximized)
        {
            MaximizeNormalLabel.Content = "\u2750";
            MaximizeNormalLabel.FontSize = 20;
        }
    }


    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        parentWindow.Close();
    }

    private void Minimize(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        parentWindow.WindowState = WindowState.Minimized;
    }
    
    private void MaximizeNormal(object sender, RoutedEventArgs e)
    { /*   */
        
        var parentWindow = Window.GetWindow(this);
        if (parentWindow.WindowState == WindowState.Maximized)
        {
            MaximizeNormalLabel.Content = "\u25a1";
            parentWindow.WindowState = WindowState.Normal;
            MaximizeNormalLabel.FontSize = 25;
        }
        else
        {
            MaximizeNormalLabel.Content = "\u2750";
            parentWindow.WindowState = WindowState.Maximized;
            MaximizeNormalLabel.FontSize = 20;
        }
    }
}