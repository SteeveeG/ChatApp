using System.Windows.Media;

namespace ChatApp.LoginRegisterMenu.LoginControl;

public class LoginViewModel : ViewModelBase
{
 
    private string name;
    private string password;
    private bool nameFocused;
    private bool passwordFocused;
    private SolidColorBrush textboxBorderColor;
    private SolidColorBrush foreGroundColor;
    private SolidColorBrush borderColor = new() { Color = (Color)ColorConverter.ConvertFromString("#19232e") };
    public LoginViewModel()
    {
        Name = "";
        Password = "";
        TextboxBorderColor = borderColor;
        ForeGroundColor = Brushes.GhostWhite;
    }
    public void LoginFailed()
    {
        TextboxBorderColor = Brushes.Red;
        ForeGroundColor =Brushes.Red;
    }
    public void LoginSucceeded()
    {
        TextboxBorderColor = borderColor;
        ForeGroundColor = Brushes.GhostWhite;
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