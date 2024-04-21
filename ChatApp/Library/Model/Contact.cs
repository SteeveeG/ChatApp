namespace Library.Model;

public class Contact
{
    public string ContactId { get; set; }
    public string UserId { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public string ContactUsername { get; set; }
}