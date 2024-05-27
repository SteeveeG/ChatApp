using System.Windows.Media;
using ChatApp.CustomMessageBox;

namespace ChatApp.LoginRegisterMenu.RegisterControl;

public class RegisterViewModel : ViewModelBase
{
    private string password;
    private string name;
    private bool nameFocused;
    private bool passwordFocused;
    private SolidColorBrush textboxBorderColor;
    private SolidColorBrush foreGroundColor;
    private SolidColorBrush borderColor = new() { Color = (Color)ColorConverter.ConvertFromString("#19232e") };

    public RegisterViewModel()
    {
        Name = "";
        Password = "";
        TextboxBorderColor = borderColor;
        ForeGroundColor = Brushes.GhostWhite;
    }
    public void RegisterSucceeded()
    {      
        TextboxBorderColor =  borderColor;
        ForeGroundColor = Brushes.GhostWhite;
    }

    public void RegisterFailed()
    {
        CustomMessageBoxHandler.Create("Username Already in Use");
        TextboxBorderColor = Brushes.Red;
        ForeGroundColor = Brushes.Red;
    }
    
    
    public bool PasswordFocused
    {
        get => passwordFocused;
        set
        {
            if (value == passwordFocused) return;
            passwordFocused = value;
            OnPropertyChanged();
        }
    }

    public bool NameFocused
    {
        get => nameFocused;
        set
        {
            if (value == nameFocused) return;
            nameFocused = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => name;
        set
        {
            if (value == name) return;
            name = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => password;
        set
        {
            if (value == password) return;
            password = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush TextboxBorderColor
    {
        get => textboxBorderColor;
        set
        {
            if (Equals(value, textboxBorderColor)) return;
            textboxBorderColor = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush ForeGroundColor
    {
        get => foreGroundColor;
        set
        {
            if (Equals(value, foreGroundColor)) return;
            foreGroundColor = value;
            OnPropertyChanged();
        }
    }
}