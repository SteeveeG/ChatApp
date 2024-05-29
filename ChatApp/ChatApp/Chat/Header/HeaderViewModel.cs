using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatApp.Chat.Header;

public class HeaderViewModel : ViewModelBase
{
    private string name;
    private ImageSource imageSource;
    private ChatViewModel ChatViewModel { get; set; }
    public HeaderViewModel(ChatViewModel chatViewModel, string header)
    {
        ChatViewModel = chatViewModel;
        Name = header;
    }
    
    public void CreatePb(string byteString)
    {
        if (string.IsNullOrEmpty(byteString))
        {
            ImageSource = null;
            return;
        }
        var list =new List<byte>();
        var str = string.Empty;
        foreach (var bytechar in byteString)
        {
            if (bytechar != '{' && bytechar != ',')
            {
                str += bytechar;
            }
            else if (bytechar == ',')
            {
                list.Add(Convert.ToByte(str));
                str = string.Empty;
            }
            
        }
        using var stream = new MemoryStream(list.ToArray());
        ImageSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
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

    public ImageSource ImageSource
    {
        get => imageSource;
        set
        {
            if (Equals(value, imageSource)) return;
            imageSource = value;
            OnPropertyChanged();
        }
    }
}