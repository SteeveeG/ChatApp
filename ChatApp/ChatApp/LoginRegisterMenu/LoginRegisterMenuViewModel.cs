using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChatApp.ApiHandler;
using ChatApp.LoginRegisterMenu.LoginControl;
using ChatApp.LoginRegisterMenu.RegisterControl;
using Library.Model;

namespace ChatApp.LoginRegisterMenu;

public class LoginRegisterMenuViewModel : ViewModelBase
{
    public LoginViewModel LoginViewModel { get; set; }
    public RegisterViewModel RegisterViewModel { get; set; }

    public LoginRegisterMenuViewModel()
    {
        LoginViewModel = new LoginViewModel();
        RegisterViewModel = new RegisterViewModel();
    }
    
    public async Task<bool> ControlRegister()
    {
        var result = await ControlCreate(RegisterViewModel.Name);
        if (!result)
        {
            RegisterViewModel.RegisterFailed();
            return false;
        }
        RegisterViewModel.RegisterSucceeded();
        return true;
    }
    public async Task<AccUser> ControlLogin()
    {
        var user = await Login(LoginViewModel.Name, LoginViewModel.Password);
        if (user == null)
        {
            LoginViewModel.LoginFailed();
            return null;
        }

        LoginViewModel.LoginSucceeded();
        return user;
    }
    
    public async Task<AccUser> Create(string username, string password)
    {
        return await ApiPost.Post<AccUser>($"Sql/CreateAcc?username={username}&password={password}",new StringContent(""));
    }
    private async Task<bool> ControlCreate(string username)
    {
        return await ApiGet.GetApiIn<bool>($"Sql/ControlCreateUsername?username={username}");
    }

    private async Task<AccUser> Login(string username, string password)
    {
        return await ApiGet.GetApiIn<AccUser>($"Sql/GetAcc?username={username}&password={password}");
    }
}