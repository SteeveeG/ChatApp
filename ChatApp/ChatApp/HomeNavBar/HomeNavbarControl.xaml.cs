using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp.HomeNavBar;

public partial class HomeNavbarControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = 
        DependencyProperty.Register(nameof(ViewModel), typeof(HomeNavbarViewModel),
            typeof(HomeNavbarControl), new PropertyMetadata(default(HomeNavbarViewModel)));

    public HomeNavbarControl()
    {
        InitializeComponent();
    }

    public HomeNavbarViewModel ViewModel
    {
        get => (HomeNavbarViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private static readonly SolidColorBrush unHoverColor = new() { Color = Color.FromRgb(63, 89, 117)};
   
     private void Mouse(object sender, MouseEventArgs e)
     {
         var radioButton = (RadioButton)sender;
         SetRadiobutton(radioButton.Name, radioButton.IsMouseOver);
     }
     private void SetRadiobutton(string name , bool ishover)
     {
         switch (name)
         {
             case "Chat":
                 if (ViewModel.ChatIsChecked && !ishover) { return; }
                 ViewModel.ChatForeground = ishover ? Brushes.GhostWhite : unHoverColor ;
                 break;
             case "Contacts":
                 if (ViewModel.ContactsIsChecked && !ishover) { return; }
                 ViewModel.ContactsForeground = ishover ? Brushes.GhostWhite : unHoverColor ;
                 break;
             case "Settings":
                 if (ViewModel.SettingsIsChecked && !ishover) { return; }
                 ViewModel.SettingsForeground = ishover ? Brushes.GhostWhite : unHoverColor ;
                 break;
         }
     }
}