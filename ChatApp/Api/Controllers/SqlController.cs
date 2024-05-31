using System.Data.SqlClient;
using Dapper;
using Library.Model;
using Microsoft.AspNetCore.Mvc;
using Type = Library.Enum.Type;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SqlController : ControllerBase, IObservable<Subscriber>
{
    private string connectionString;
    private readonly List<IObserver<Subscriber>> observers;
    
    public SqlController()
    {
        GetLocalHost();
        observers = new List<IObserver<Subscriber>>();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public IDisposable Subscribe(IObserver<Subscriber> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscribe(observers, observer);
    }


    [HttpPost("AddUser")]
    public async void AddUser(AccUser accUser)
    {
        await InsertSql(
            $"insert into AccUser (UserId, Username, Password) values ('{accUser.UserId}','{accUser.Username}','{accUser.Password}')");
    }

    [HttpPost("AddContact")]
    public async void AddContact(Contact contact)
    {
        await InsertSql($"insert into Contact" +
                        $" (UserId, CreatedContactUserId, LastMessage, LastMessageTime) values " +
                        $"('{contact.UserId}','{contact.CreatedContactUserId}','{contact.LastMessage}',convert(datetime, '{contact.LastMessageTime}'))");
    }

    [HttpPost("AddMessage")]
    public async void AddMessage(Message message)
    {
        await InsertSql($"insert into Message" +
                        $" (UserId, ChatId, Content, Time) values " +
                        $"('{message.UserId}','{message.ChatId}','{message.Content}',convert(datetime, '{message.Time}', 104))");

        var userId = await GetContactId(message);

        await InsertSql(
            $"UPDATE Contact SET LastMessage = '{message.Content}' , LastMessageTime = convert(datetime, '{DateTime.Now}', 104)  WHERE UserId Collate Latin1_General_CS_AS = '{userId}'  and CreatedContactUserId Collate Latin1_General_CS_AS = '{message.UserId}' or CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}' and UserId Collate Latin1_General_CS_AS = '{message.UserId}'");

        var chat = await GetChat(userId, message.UserId);
        NotifyObserver(new Subscriber
        {
            Type = Type.Message,
            Message = message,
            Chat = chat
        });
    }

    private async Task<string> GetContactId(Message message)
    {
        Chat chat;
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            chat = con.Query<Chat>($"Select * from Chat where ChatId Collate Latin1_General_CS_AS = '{message.ChatId}'")
                .ToArray()[0];
            con.Close();
            return chat.UserId == message.UserId ? chat.CreatedChatUserId : chat.UserId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }

    [HttpPost("PostProfilePic")]
    public async Task<byte[]> PostProfilePic(string userId, IFormFile file)
    {
        var file1 = file;
        var length = file1.Length;
        if (length < 0)
            return Array.Empty<byte>();
        await using var fileStream = file1.OpenReadStream();
        var bytes = new byte[length];
        await fileStream.ReadAsync(bytes, 0, (int)file1.Length);
        fileStream.Close();
        var type = file1.ContentType.Contains("png") ? "png" : "jpg";
        var con = new SqlConnection(connectionString);
        con.Open();
        var rs = bytes.Aggregate("{", (current, b) => current + "," + b);
        rs = rs.Remove(1, 1);
        rs += "}";
        con.Query(
            $"update AccUser Set ProfilePicByte = '{rs}', ProfilePicType = '{type}' where UserId Collate Latin1_General_CS_AS = '{userId}'");
        con.Close();
        return bytes;
    }

    [HttpPost("CreateContact")]
    public async Task<Contact> CreateContact(string userId, string createdContactUserId)
    {
        Contact contact;
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            con.Query(
                $"Insert into Contact (UserId, CreatedContactUserId , LastMessage , LastMessageTime) values " +
                $"('{userId}', '{createdContactUserId}', ' ' ,convert(datetime, '{DateTime.Now}', 104))");
            contact = con
                .Query<Contact>(
                    $"Select * from Contact Where UserId Collate Latin1_General_CS_AS = '{userId}' and CreatedContactUserId Collate Latin1_General_CS_AS = '{createdContactUserId}'")
                .ToArray()[0];
            con.Close();
            NotifyObserver(new Subscriber
            {
                Type = Type.CreatedContact,
                Contact = contact
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Contact();
        }

        return contact;
    }

    [HttpPost("CreateAcc")]
    public async Task<AccUser> CreateAcc(string username, string password)
    {
        try
        {
            string userId;
            IEnumerable<int> count;
            var con = new SqlConnection(connectionString);
            con.Open();
            do
            {
                userId = RandomString();
                count = con.Query<int>(
                    $"Select COUNT(*) from AccUser where UserId Collate Latin1_General_CS_AS = '{userId}'");
            } while (count.ToArray()[0] > 0);

            con.Query("Insert Into AccUser (UserId ,Username ,Password , ProfilePicByte , ProfilePicType)" +
                      $"VALUES ('{userId}','{username}','{password}', '','')");
            var account =
                con.Query<AccUser>($"Select * from AccUser where UserId Collate Latin1_General_CS_AS = '{userId}'");
            con.Close();
            return account.ToList()[0];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AccUser();
        }
    }

    [HttpPost("ChangeUsername")]
    public async Task<bool> ChangeUsername(string newusername, string userId)
    {
        int count;
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            count = con.Query<int>(
                    $"Select Count(*) from AccUser where Username Collate Latin1_General_CS_AS = '{newusername}'")
                .ToArray()[0];
            if (count != 0)
            {
                return false;
            }

            con.Query(
                $"UPDATE AccUser SET Username  = '{newusername}' WHERE UserId Collate Latin1_General_CS_AS = '{userId}'");
            con.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    [HttpPost("ChangePassword")]
    public async Task<bool> ChangePassword(string userId, string password)
    {
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            con.Query(
                $"UPDATE AccUser SET Password = '{password}' WHERE UserId Collate Latin1_General_CS_AS = '{userId}'");
            con.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }


    [HttpGet("CreateChat")]
    public async Task<Chat> CreateChat(string userId, string contactId)
    {
        var chatId = RandomString();
        int count;
        do
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            count = con.Query<int>($"Select Count(*) from Chat where ChatId Collate Latin1_General_CS_AS = '{chatId}'")
                .ToArray()[0];
            con.Close();
        } while (count != 0);

        var chat = new Chat
        {
            CreatedChatUserId = userId,
            UserId = contactId,
            ChatId = chatId
        };
        await InsertSql(
            $"insert into Chat (ChatId , UserId, CreatedChatUserId) values ('{chat.ChatId}' ,'{chat.UserId}' , '{chat.CreatedChatUserId}')");

        return chat;
    }

    [HttpGet("ControlCreateUsername")]
    public async Task<bool> ControlCreateUsername(string username)
    {
        try
        {
            IEnumerable<int> count;
            var con = new SqlConnection(connectionString);
            con.Open();
            count = con.Query<int>(
                $"Select COUNT(*) from AccUser where Username Collate Latin1_General_CS_AS = '{username}'");

            con.Close();
            return count.ToArray()[0] == 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    [HttpGet("GetAcc")]
    public async Task<AccUser> GetAcc(string username, string password)
    {
        IEnumerable<AccUser> user = new List<AccUser>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            user = con.Query<AccUser>(
                $"Select * from AccUser where Username Collate Latin1_General_CS_AS = '{username}' and Password Collate Latin1_General_CS_AS = '{password}' ");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return user.ToList().Count == 0 ? new AccUser() : user.ToList()[0];
    }

    [HttpGet("GetUserContacts")]
    public async Task<List<Contact>> GetUserContacts(string userId)
    {
        IEnumerable<Contact> contacts = new List<Contact>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            contacts = await con.QueryAsync<Contact>(
                $"Select * from Contact where UserId Collate Latin1_General_CS_AS = '{userId}'" +
                $"or CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}' order by UserId ASC ");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return contacts.ToList();
    }

    [HttpGet("GetContactNames")]
    public async Task<List<string>> GetContactNames(string userId)
    {
        IEnumerable<Contact> contacts = new List<Contact>();
        var contactsNames = new List<string>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            contacts = con.Query<Contact>(
                $"Select * from Contact where UserId Collate Latin1_General_CS_AS = '{userId}'or CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}'");
            foreach (var contact in contacts)
            {
                if (contact.CreatedContactUserId == userId)
                {
                    contactsNames.Add(con
                        .Query<string>(
                            $"Select Username from AccUser where UserId Collate Latin1_General_CS_AS = '{contact.UserId}' order by UserId ASC  ")
                        .ToArray()[0]);
                }
                else if (contact.UserId == userId)
                {
                    contactsNames.Add(con
                        .Query<string>(
                            $"Select Username from AccUser where UserId Collate Latin1_General_CS_AS = '{contact.CreatedContactUserId}' order by UserId ASC ")
                        .ToArray()[0]);
                }
            }

            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return contactsNames;
    }


    [HttpGet("GetContactNamesAndPb")]
    public async Task<List<Tuple<string, string, string,string>>> GetContactNamesAndPb(string userId)
    {
        IEnumerable<Contact> contacts = new List<Contact>();
        var contactsdata = new List<AccUser>();
        var contactsTuple = new List<Tuple<string, string, string,string>>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            contacts = con.Query<Contact>(
                $"Select * from Contact where UserId Collate Latin1_General_CS_AS = '{userId}'or CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}'");
            foreach (var contact in contacts)
            {
                if (contact.UserId.Length == 52 || contact.CreatedContactUserId.Length == 52)
                {
                    var id = contact.UserId.Length == 52 ? contact.UserId : contact.CreatedContactUserId;
                    contactsdata.Add(new AccUser
                    {
                        Username = "Deleted Contact",
                        ProfilePicByte = string.Empty,
                        ProfilePicType = string.Empty,
                        UserId = id
                    });
                continue;
                }

                if (contact.CreatedContactUserId == userId)
                {
                    contactsdata.Add(con
                        .Query<AccUser>(
                            $"Select Username,UserId, ProfilePicByte , ProfilePicType from AccUser where UserId Collate Latin1_General_CS_AS = '{contact.UserId}' order by UserId ASC  ")
                        .ToArray()[0]);
                }
                else if (contact.UserId == userId)
                {
                    contactsdata.Add(con
                        .Query<AccUser>(
                            $"Select Username, UserId ,ProfilePicByte , ProfilePicType from AccUser where UserId Collate Latin1_General_CS_AS = '{contact.CreatedContactUserId}' order by UserId ASC ")
                        .ToArray()[0]);
                }
            }

            con.Close();
            contactsTuple.AddRange(contactsdata.Select(contactdata =>
                new Tuple<string, string, string,string>(contactdata.Username, contactdata.ProfilePicByte,
                    contactdata.ProfilePicType , contactdata.UserId)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return contactsTuple;
    }


    [HttpGet("GetNamesAndPb")]
    public async Task<Tuple<string, string, string>> GetNamesAndPb(string userId)
    {
        var data = new AccUser();
        Tuple<string, string, string> contactsTuple;
        try
        {
            if (userId.Length == 52)
            {
                return new Tuple<string, string, string>("Deleted Contact", string.Empty, string.Empty);
            }
            var con = new SqlConnection(connectionString);
            con.Open();
            data = con
                .Query<AccUser>(
                    $"Select Username, ProfilePicByte , ProfilePicType from AccUser where UserId Collate Latin1_General_CS_AS = '{userId}' order by UserId ASC  ")
                .ToArray()[0];
            con.Close();
            contactsTuple = new Tuple<string, string, string>(data.Username, data.ProfilePicByte, data.ProfilePicType);
        }
        catch (Exception e)
        {
            contactsTuple = new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            Console.WriteLine(e);
        }

        return contactsTuple;
    }


    [HttpGet("GetUsername")]
    public async Task<string> GetUsername(string userId)
    {
        var username = string.Empty;
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            username = con
                .Query<string>($"Select Username from AccUser where UserId Collate Latin1_General_CS_AS = '{userId}'")
                .ToArray()[0];
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return username;
    }


    [HttpGet("GetChat")]
    public async Task<Chat> GetChat(string userId, string contactId)
    {
        IEnumerable<Chat> chat = new List<Chat>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            chat = con.Query<Chat>(
                $"Select * from Chat where CreatedChatUserId Collate Latin1_General_CS_AS = '{userId}'" +
                $" and UserId Collate Latin1_General_CS_AS = '{contactId}' or CreatedChatUserId Collate " +
                $"Latin1_General_CS_AS = '{contactId}' and UserId Collate Latin1_General_CS_AS = '{userId}'");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return chat.ToList().Count == 0 ? new Chat() : chat.ToList()[0];
    }

    [HttpGet("GetMessages")]
    public async Task<List<Message>> GetMessages(string chatId)
    {
        IEnumerable<Message> messages = new List<Message>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            messages = con.Query<Message>(
                $"Select * from Message where ChatId Collate Latin1_General_CS_AS = '{chatId}' order by  Time ASC");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return messages.ToList();
    }

    [HttpDelete("DeleteContact")]
    public async void DeleteContact(string contactId, string userId)
    {
        if (contactId.Length == 52)
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            var chatId = con
                .Query<string>(
                    $"Select ChatId from Chat where CreatedChatUserId Collate Latin1_General_CS_AS = '{userId}' and UserId Collate Latin1_General_CS_AS = '{contactId}' or UserId ='{userId}' and CreatedChatUserId = '{contactId}'")
                .ToArray()[0];
            con.Close();
            await InsertSql($"Delete Message where ChatId Collate Latin1_General_CS_AS = '{chatId}' ");
            await InsertSql($"Delete Chat where ChatId Collate Latin1_General_CS_AS = '{chatId}' ");
            await InsertSql(
                $"Delete Contact where UserId Collate Latin1_General_CS_AS = '{contactId}' and CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}' or " +
                $"CreatedContactUserId Collate Latin1_General_CS_AS = '{contactId}' and UserId Collate Latin1_General_CS_AS = '{userId}'");
            return;
        }

        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            var chatId = con
                .Query<string>(
                    $"Select ChatId from Chat where CreatedChatUserId Collate Latin1_General_CS_AS = '{userId}' and UserId Collate Latin1_General_CS_AS = '{contactId}' or UserId ='{userId}' and CreatedChatUserId = '{contactId}'")
                .ToArray()[0];
            con.Query(
                $"Update Contact set UserId = 'FFFFFFF{userId}' where UserId Collate Latin1_General_CS_AS  = '{userId}' and CreatedContactUserId Collate Latin1_General_CS_AS = '{contactId}'");
            con.Query(
                $"Update Contact set CreatedContactUserId = 'FFFFFFF{userId}' where CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}' and UserId Collate Latin1_General_CS_AS = '{contactId}'");
            con.Query(
                $"update Chat set UserId  = 'FFFFFFF{userId}' where UserId Collate Latin1_General_CS_AS = '{userId}' and CreatedChatUserId Collate Latin1_General_CS_AS = '{contactId}'");
            con.Query(
                $"update Chat set CreatedChatUserId  = 'FFFFFFF{userId}' where CreatedChatUserId Collate Latin1_General_CS_AS = '{userId}' and UserId Collate Latin1_General_CS_AS = '{contactId}'");
            con.Query(
                $"update Message set UserId  = 'FFFFFFF{userId}' where UserId Collate Latin1_General_CS_AS = '{userId}' and ChatId Collate Latin1_General_CS_AS = '{chatId}'");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        NotifyObserver(new Subscriber
        {
            Type = Type.Delete,
            AccUser = new AccUser
            {
                UserId = "FFFFFFF" + userId
            }
        });

    }

    [HttpDelete("OwnDeleteAcc")]
    public async Task<bool> OwnDeleteAcc(string userId)
    {
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            var iEnurmerableContacts = await con.QueryAsync<Contact>(
                $"select * from Contact where UserId Collate Latin1_General_CS_AS = '{userId}' or CreatedContactUserId Collate Latin1_General_CS_AS = '{userId}'");
            var contacts = iEnurmerableContacts.ToList();
            foreach (var contact in contacts)
            {
                DeleteContact(contact.CreatedContactUserId == userId ? contact.UserId : contact.CreatedContactUserId, userId);
            }

            await InsertSql($"Delete AccUser where UserId = '{userId}'");
            
            NotifyObserver(new Subscriber
            {
                Type = Type.Delete,
                AccUser = new AccUser
                {
                    UserId = "FFFFFFF" + userId
                }
            });
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private string RandomString()
    {
        var random = new Random();
        string userId;
        do
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            userId = new string(Enumerable.Repeat(chars, 45)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } while (false);

        return userId;
    }

    private async Task<bool> InsertSql(string sql)
    {
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            await con.QueryAsync(sql);
            con.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<List<string>> GetChatIds(string userId)
    {
        var list = new List<string>();
        try
        {
            var con = new SqlConnection(connectionString);
            con.Open();
            list = con.Query<string>(
                    $"Select ChatId from Chat where UserId Collate Latin1_General_CS_AS = '{userId}' or CreatedChatUserId Collate Latin1_General_CS_AS = '{userId}'")
                .ToList();
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<string>();
        }

        return list;
    }

   

    private void NotifyObserver(Subscriber subscriber)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(subscriber);
        }
    }

    private void GetLocalHost()
    {
        var myServer = Environment.MachineName;
        connectionString =
        // @$"Server={myServer}\MSSQL2022;Database=ChatApp;Trusted_Connection=True;MultipleActiveResultSets=True";
       @$"Server={myServer};Database=ChatApp;Trusted_Connection=True;MultipleActiveResultSets=True";
    }


    private class Unsubscribe : IDisposable
    {
        private List<IObserver<Subscriber>> observers;
        private IObserver<Subscriber>? observer;


        public Unsubscribe(List<IObserver<Subscriber>> observers, IObserver<Subscriber> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (observer != null && observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}