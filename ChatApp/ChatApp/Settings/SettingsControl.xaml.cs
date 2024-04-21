using System.Windows;
using System.Windows.Controls;

namespace ChatApp.Settings;

public partial class SettingsControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(SettingsViewModel), typeof(SettingsControl), new PropertyMetadata(default(SettingsViewModel)));

    public SettingsControl()
    {
        InitializeComponent();
    }

    public SettingsViewModel ViewModel
    {
        get => (SettingsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}