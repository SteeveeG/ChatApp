namespace ChatApp.Settings.SettingsInput;

public class SettingsInputViewModel : ViewModelBase
{
    private string label;
    private bool isTextInput;
    private string buttonContent;
    private string newInput;
    private string waterMark;
    private bool isChanged;
    public DelegateCommand DelegateCommand { get; set; }


    public SettingsInputViewModel(string label, string waterMark  )
    {
        Label = label;
        IsTextInput = true;
        WaterMark = waterMark;

    }

    public SettingsInputViewModel(string label, string buttonContent, DelegateCommand delegateCommand)
    {
        Label = label;
        IsTextInput = false;
        DelegateCommand = delegateCommand;
        ButtonContent = buttonContent;
    }

    public string Label
    {
        get => label;
        set
        {
            if (value == label) return;
            label = value;
            OnPropertyChanged();
        }
    }


    public bool IsTextInput
    {
        get => isTextInput;
        set
        {
            if (value == isTextInput) return;
            isTextInput = value;
            OnPropertyChanged();
        }
    }

    public bool IsButtonInput => !IsTextInput;


    public string ButtonContent
    {
        get => buttonContent;
        set
        {
            if (value == buttonContent) return;
            buttonContent = value;
            OnPropertyChanged();
        }
    }

    public string? NewInput
    {
        get => newInput;
        set
        {
            if (value == newInput) return;
            newInput = value;
            OnPropertyChanged();
        }
    }

    public string WaterMark
    {
        get => waterMark;
        set
        {
            if (value == waterMark) return;

            waterMark = $" {value}";
            OnPropertyChanged();
        }
    }
    public bool IsChanged
    {
        get => isChanged;
        set
        {
            if (value == isChanged) return;
            isChanged = value;
            OnPropertyChanged();
        }
    }
}