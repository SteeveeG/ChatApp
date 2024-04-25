namespace ChatApp.CustomMessageBox;

public class CustomMessageBoxHandler
{
    public static void Create(string message)
    {
        var customMessageBox = new CustomMessageBox();
        customMessageBox.DataContext = new CustomMessageBoxViewModel(message);
        customMessageBox.ShowDialog();
    }
}