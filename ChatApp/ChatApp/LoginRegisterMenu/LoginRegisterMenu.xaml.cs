using System.Security.Cryptography;
using System.Text;
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

        var password = viewModel.RegisterViewModel.Password;
        
        using (var sha256 = SHA256.Create())
        {
            byte[] inputBytes;
            inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes;
            hashBytes = sha256.ComputeHash(inputBytes);
            password = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        
        
        var accUser = await viewModel.Create(viewModel.RegisterViewModel.Name, password);
        var mainWindow = new MainWindow();
        mainWindow.Init();
        mainWindow.DataContext = new MainViewModel(accUser);
        mainWindow.Show();
        Close();
    }
}