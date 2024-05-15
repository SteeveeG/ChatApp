using System.Net.Http;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using ChatApp.ApiHandler;
using ChatApp.Settings.SettingsInput;
using Library.Model;

namespace ChatApp.Settings;

public class SettingsViewModel : ViewModelBase
{
    private AccUser accUser;
    private MainViewModel mainViewModel;
    public SettingsInputViewModel NewNameSettingsInputViewModel { get; set; }
    public SettingsInputViewModel PbSettingsInputViewModel { get; set; }
    public SettingsInputViewModel AccSettingsInputViewModel { get; set; }
    public SettingsInputViewModel NewPasswordSettingsInputViewModel { get; set; }
    public SettingsInputViewModel LogOutPasswordSettingsInputViewModel { get; set; }
    public DelegateCommand SaveCommand { get; set; }

    public SettingsViewModel(MainViewModel mainViewModel, AccUser acc = null)
    {
        this.mainViewModel = mainViewModel;
        accUser = acc;
        SaveCommand = new DelegateCommand(Save);
        NewNameSettingsInputViewModel = new SettingsInputViewModel("New Name:", "New Name");
        PbSettingsInputViewModel = new SettingsInputViewModel("New Profile Picture:", "Locate Picture",
            new DelegateCommand(ChangeProfilePic));
        AccSettingsInputViewModel =
            new SettingsInputViewModel("Account:", "Delete Account", Delete);
        NewPasswordSettingsInputViewModel = new SettingsInputViewModel("New Password:", "New Password");
        LogOutPasswordSettingsInputViewModel =
            new SettingsInputViewModel("Log Out:", "Log Out",  new DelegateCommand(() => Console.WriteLine()));
    }

    private async Task<bool> Delete()
    {
        var userId = HttpUtility.UrlEncode(accUser.UserId);
        return await Api.Delete<bool>($"Sql/OwnDeleteAcc?userId={userId}");
    }


    private async void Save()
    {
        if (!string.IsNullOrEmpty(NewNameSettingsInputViewModel.NewInput?.Trim()))
        {
            var input = HttpUtility.UrlEncode(NewNameSettingsInputViewModel.NewInput);
            var result = await Api.Post<bool>($"Sql/ChangeUsername?newusername={input}&userId={accUser.UserId}",
                new StringContent(""));
            if (result)
            {
                mainViewModel.NewName(NewNameSettingsInputViewModel.NewInput);
                NewNameSettingsInputViewModel.NewInput = string.Empty;
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
            var result = await Api.Post<bool>($"Sql/ChangePassword?userId={accUser.UserId}&password={input}",
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


    public void ChangeProfilePic()
    {
    }
}