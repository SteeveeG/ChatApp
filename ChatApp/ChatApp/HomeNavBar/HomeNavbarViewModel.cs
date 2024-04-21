using System.Windows.Media;

namespace ChatApp.HomeNavBar;

public class  HomeNavbarViewModel : ViewModelBase
{
    private bool settingsIsChecked;
    private bool chatIsChecked;
    private bool contactsIsChecked;
    private SolidColorBrush chatForeground;
    private SolidColorBrush contactsForeground;
    private SolidColorBrush settingsForeground;

    private static readonly SolidColorBrush UnHover = new() { Color = Color.FromRgb(63, 89, 117) };
    private static readonly SolidColorBrush Hover = Brushes.GhostWhite;

    public HomeNavbarViewModel( )
    {
        ChatForeground = UnHover;
        ContactsForeground = UnHover;
        SettingsForeground = UnHover;

        ChatIsChecked = true;
    }
    
    
    public bool ContactsIsChecked
    {
        get => contactsIsChecked;
        set
        {
            if (value == contactsIsChecked) return;
            contactsIsChecked = value;
            ContactsForeground = !contactsIsChecked ? UnHover : Hover;
            OnPropertyChanged();
        }
    }

    public bool ChatIsChecked
    {
        get => chatIsChecked;
        set
        {
            if (value == chatIsChecked) return;
            chatIsChecked = value;
            ChatForeground = !chatIsChecked ? UnHover : Hover;
            OnPropertyChanged();
        }
    }

    public bool SettingsIsChecked
    {
        get => settingsIsChecked;
        set
        {
            if (value == settingsIsChecked) return;
            settingsIsChecked = value;
            SettingsForeground = !settingsIsChecked ? UnHover : Hover;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush SettingsForeground
    {
        get => settingsForeground;
        set
        {
            if (Equals(value, settingsForeground)) return;
            settingsForeground = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush ContactsForeground
    {
        get => contactsForeground;
        set
        {
            if (Equals(value, contactsForeground)) return;
            contactsForeground = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush ChatForeground
    {
        get => chatForeground;
        set
        {
            if (Equals(value, chatForeground)) return;
            chatForeground = value;
            OnPropertyChanged();
        }
    }
}