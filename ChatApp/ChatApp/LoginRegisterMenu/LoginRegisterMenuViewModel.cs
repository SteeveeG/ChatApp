using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ChatApp.LoginRegisterMenu.LoginControl;
using ChatApp.LoginRegisterMenu.RegisterControl;
using Library.Model;

namespace ChatApp.LoginRegisterMenu;

public class LoginRegisterMenuViewModel : ViewModelBase
{
    private Random random;
    public LoginViewModel LoginViewModel { get; set; }
    public RegisterViewModel RegisterViewModel { get; set; }

    public LoginRegisterMenuViewModel()
    {
        random = new Random();
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
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.PostAsync($"Sql/CreateAcc?username={username}&password={password}" ,null);
        if (!response.IsSuccessStatusCode)
        {
            return new AccUser();
        }
        var accjson = await response.Content.ReadAsStringAsync();
        var accUser = JsonSerializer.Deserialize<AccUser>(accjson);
        return accUser;
    }
    private async Task<bool> ControlCreate(string username)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"Sql/ControlCreate?username={username}");
        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
        }

        Console.WriteLine("Internal server Error");
        return false;
    }

    private async Task<AccUser> Login(string username, string password)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7261");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"Sql/GetAcc?username={username}&password={password}");
        if (response.IsSuccessStatusCode)
        {
            var accUser = await response.Content.ReadFromJsonAsync<AccUser>();
            return accUser;
        }

        Console.WriteLine("Internal server Error");
        return new AccUser();
    }
}