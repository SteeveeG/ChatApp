using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using ChatApp.ApiHandler;
using ChatApp.CustomMessageBox;
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
        var result = await ControlCreateUsername(RegisterViewModel.Name);
        if (!result)
        {
            RegisterViewModel.RegisterFailed();
            return false;
        }

        var tupleResult = PasswordValidator.ValidatePassword(RegisterViewModel.Password);
        if (tupleResult.Item1)
        {
            RegisterViewModel.RegisterSucceeded();
            return true;
        }
        
        CustomMessageBoxHandler.Create($"{tupleResult.Item2}");
        return false;
    }

    public async Task<AccUser> ControlLogin()
    {
        var password = LoginViewModel.Password;
        using (var sha256 = SHA256.Create())
        {
            byte[] inputBytes;
            inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes;
            hashBytes = sha256.ComputeHash(inputBytes);
            password = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        
        var user = await Login(LoginViewModel.Name, password);
        if (user.UserId == null)
        {
            LoginViewModel.LoginFailed();
            return null;
        }

        LoginViewModel.LoginSucceeded();
        return user;
    }

    public async Task<AccUser> Create(string username, string password)
    {
        return await Api.Post<AccUser>($"Sql/CreateAcc?username={username}&password={password}",
            new StringContent(""));
    }

    private async Task<bool> ControlCreateUsername(string username)
    {
        return await Api.GetIn<bool>($"Sql/ControlCreateUsername?username={username}");
    }

    private async Task<AccUser> Login(string username, string password)
    {
        return await Api.GetIn<AccUser>($"Sql/GetAcc?username={username}&password={password}");
    }
}