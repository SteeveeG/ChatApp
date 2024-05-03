using System.Reflection.PortableExecutable;
using Type = Library.Enum.Type;

namespace Library.Model;

public class Subscriber
{
    public Type Type { get; set; }
    public AccUser? AccUser { get; set; }
    public Chat? Chat { get; set; }
    public Contact? Contact { get; set; }
    public Message? Message { get; set; }
}