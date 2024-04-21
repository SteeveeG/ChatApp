using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatApp.LoginRegisterMenu;

public partial class LoginRegisterMenu : Window
{
    public LoginRegisterMenu()
    {
        InitializeComponent();
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void LogIn(object sender, RoutedEventArgs e)
    {
        var viewModel = (LoginRegisterMenuViewModel)((Button)sender).DataContext;
        var accUser = await viewModel.ControlLogin();
        if (accUser == null) 
        {
            return;
        }
        var mainWindow = new MainWindow();
        mainWindow.Init();
        mainWindow.DataContext = new MainViewModel(accUser);
        mainWindow.Show();
        Close();
    }

    private async void Register(object sender, RoutedEventArgs e)
    {
        var viewModel = (LoginRegisterMenuViewModel)((Button)sender).DataContext;
        if (!await viewModel.ControlRegister())
        {
            return;
        }
        var accUser = await viewModel.Create(viewModel.RegisterViewModel.Name, viewModel.RegisterViewModel.Password);
        var mainWindow = new MainWindow();
        mainWindow.Init();
        mainWindow.DataContext = new MainViewModel(accUser);
        mainWindow.Show();
        Close();
    }
}