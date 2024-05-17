using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using ChatApp.ApiHandler;
using ChatApp.CustomMessageBox;
using ChatApp.Settings.SettingsInput;
using Library.Enum;
using Library.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Win32;
using File = System.IO.File;

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
            new SettingsInputViewModel("Log Out:", "Log Out", new DelegateCommand(() => Console.WriteLine()));
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
            if (NewNameSettingsInputViewModel.NewInput == accUser.Username)
            {
                CustomMessageBoxHandler.Create("you already have this username");
                return;
            }

            var input = HttpUtility.UrlEncode(NewNameSettingsInputViewModel.NewInput);
            var result = await Api.Post<bool>($"Sql/ChangeUsername?newusername={input}&userId={accUser.UserId}",
                new StringContent(""));
            if (result)
            {
                mainViewModel.NewName(NewNameSettingsInputViewModel.NewInput);
                NewNameSettingsInputViewModel.NewInput = string.Empty;
            }
            else
            {
                CustomMessageBoxHandler.Create("this name is already taken :(");
                return;
            }
        }

        if (NewPasswordSettingsInputViewModel.NewInput?.Trim() == null)
        {
            return;
        }

        var passwordResult = PasswordValidator.ValidatePassword(NewPasswordSettingsInputViewModel.NewInput.Trim());
        if (passwordResult.Item1)
        {
            var input = HttpUtility.UrlEncode(NewPasswordSettingsInputViewModel.NewInput);
            var result = await Api.Post<bool>($"Sql/ChangePassword?userId={accUser.UserId}&password={input}",
                new StringContent(""));
            if (result)
            {
                NewPasswordSettingsInputViewModel.NewInput = string.Empty;
            }
        }
        else
        {
            CustomMessageBoxHandler.Create($"Password Rules Broken\n{passwordResult.Item2}");
        }
    }


    public async void ChangeProfilePic()
    {
        try
        {
            string mediaType;
            var path = s("Profile Picture", FileFormat.PNG, FileFormat.JPG);
            if (path == null)
            {
                return;
            }
            var fi = new FileInfo(path);
            if (path.Contains(".jpg"))
            {
                fi.CopyTo(@"Pb\pb.jpg", true);
                mediaType = "image/jpeg";
            }
            else if (path.Contains(".png"))
            {
                fi.CopyTo(@"Pb\pb.png", true);
                mediaType = "image/png";
            }
            else
            {
                CustomMessageBoxHandler.Create("Error");
                return;
            }
            using (var content = new MultipartFormDataContent())
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7049");
                var filearray = File.ReadAllBytes(path);
                var filecontent = new ByteArrayContent(filearray);
                filecontent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                content.Add(filecontent, "file", Path.GetFileName(path));
                HttpResponseMessage response = await client.PostAsync("Sql/PostProfilePic", content);
            }
        }
        catch
        {
            CustomMessageBoxHandler.Create("Error");
        }
    }

    public string s(string header, params FileFormat[] formats)
    {
        var openFileDialog = new OpenFileDialog();

        var filter = "";

        if (formats.Contains(FileFormat.PNG))
        {
            filter += " (*.png)|*.png|";
        }

        if (formats.Contains(FileFormat.JPG))
        {
            filter += " (*.jpg)|*.jpg|";
        }


        openFileDialog.Filter = filter + " (*.*)|*.*";

        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        openFileDialog.Multiselect = false;

        openFileDialog.Title = header;

        if (openFileDialog.ShowDialog() == false)
        {
            return null;
        }

        return openFileDialog.FileName;
    }
}