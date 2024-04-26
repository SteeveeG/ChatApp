using System.Reflection.PortableExecutable;

namespace Library.Model;

public class Subscriber
{
    public AccUser AccUser { get; set; }
    public Chat Chat { get; set; }
    public Contact Contact { get; set; }
    public Message Message { get; set; }
}