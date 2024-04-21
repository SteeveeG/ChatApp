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

    public SettingsViewModel(string userId = "")
    {
        SaveCommand = new DelegateCommand(Save);
        NewNameSettingsInputViewModel = new SettingsInputViewModel("New Name:" ,"New Name" );
        PbSettingsInputViewModel= new SettingsInputViewModel("New Profile Picture:" , "Locate Picture" , new DelegateCommand(ChangeProfilePic));
        AccSettingsInputViewModel= new SettingsInputViewModel("Account:" , "Delete Account",new DelegateCommand(DeleteAccount));
        NewPasswordSettingsInputViewModel= new SettingsInputViewModel("New Password:" ,  "New Password");
        LogOutPasswordSettingsInputViewModel= new SettingsInputViewModel("Log Out:" ,"Log Out" , new DelegateCommand(LogOut));
    }
    
    private void Save()
    {
        
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