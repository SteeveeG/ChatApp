namespace Library.Model;

public class Message
{
    public string UserId { get; set; }
    public string ChatId { get; set; }
    public string Content { get; set; }
    public DateTime Time { get; set; }
}