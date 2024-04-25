using System.Net.Http;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using ChatApp.ApiHandler;
using ChatApp.Settings.SettingsInput;

namespace ChatApp.Settings;

public class SettingsViewModel : ViewModelBase
{
    public SettingsInputViewModel NewNameSettingsInputViewModel { get; set; }
    public SettingsInputViewModel PbSettingsInputViewModel { get; set; }
    public SettingsInputViewModel AccSettingsInputViewModel { get; set; }
    public SettingsInputViewModel NewPasswordSettingsInputViewModel { get; set; }
    public SettingsInputViewModel LogOutPasswordSettingsInputViewModel { get; set; }
    public DelegateCommand SaveCommand { get; set; }
    private string userId;
    private MainViewModel mainViewModel;

    public SettingsViewModel(MainViewModel mainViewModel, string userId = "")
    {
        this.mainViewModel = mainViewModel;
        this.userId = userId;
        SaveCommand = new DelegateCommand(Save);
        NewNameSettingsInputViewModel = new SettingsInputViewModel("New Name:", "New Name");
        PbSettingsInputViewModel = new SettingsInputViewModel("New Profile Picture:", "Locate Picture",
            new DelegateCommand(ChangeProfilePic));
        AccSettingsInputViewModel =
            new SettingsInputViewModel("Account:", "Delete Account", new DelegateCommand(DeleteAccount));
        NewPasswordSettingsInputViewModel = new SettingsInputViewModel("New Password:", "New Password");
        LogOutPasswordSettingsInputViewModel =
            new SettingsInputViewModel("Log Out:", "Log Out", new DelegateCommand(LogOut));
    }


    private async void Save()
    {
        if (!string.IsNullOrEmpty(NewNameSettingsInputViewModel.NewInput?.Trim()))
        {
            var input = HttpUtility.UrlEncode(NewNameSettingsInputViewModel.NewInput);
            var result = await ApiPost.Post<bool>($"Sql/ChangeUsername?newusername={input}&userId={userId}",
                new StringContent(""));
            if (result)
            {
                mainViewModel.NewName(NewNameSettingsInputViewModel.NewInput);
                NewNameSettingsInputViewModel.NewInput = string.Empty;
            }
            else
            {
            }
        }

        if (NewPasswordSettingsInputViewModel.NewInput == null)
        {
            return;
        }

        var passwordResult = PasswordValidator.ValidatePassword(NewPasswordSettingsInputViewModel.NewInput.Trim());
        if (passwordResult.Item1)
        {
            if (string.IsNullOrEmpty(NewPasswordSettingsInputViewModel.NewInput.Trim()))
            {
                return;
            }
            var input = HttpUtility.UrlEncode(NewPasswordSettingsInputViewModel.NewInput);
            var result = await ApiPost.Post<bool>($"Sql/ChangePassword?userId={userId}&password={input}",
                new StringContent(""));
            if (result)
            {
                NewNameSettingsInputViewModel.NewInput = string.Empty;
            }
        }
        else
        {
            MessageBox.Show($"Password Rules Broken: {passwordResult.Item2}");
        }
    }


    private void LogOut()
    {
    }

    private void DeleteAccount()
    {
    }

    public void ChangeProfilePic()
    {
    }
}