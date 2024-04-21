using System.Data.SqlClient;
using Dapper;
using Library.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SqlController : ControllerBase
{
    private const string ConnectionString = "Server=DESKTOP-L7HGMAF;Database=ChatApp;Trusted_Connection=True;";

    [HttpPost("AddUser")]
    public void AddUser(AccUser accUser)
    {
        InsertSql(
            $"insert into AccUser (UserId, Username, Password) values ('{accUser.UserId}','{accUser.Username}','{accUser.Password}')");
    }

    [HttpPost("AddContact")]
    public void AddContact(Contact contact)
    {
        InsertSql($"insert into Contact" +
                  $" (UserId, ContactId, LastMessage, LastMessageTime, ContactUsername) values " +
                  $"('{contact.UserId}','{contact.ContactId}','{contact.LastMessage}','{contact.LastMessageTime}','{contact.ContactUsername}')");
    }

    [HttpPost("AddMessage")]
    public void AddMessage(Message message)
    {
        InsertSql($"insert into Message" +
                  $" (UserId, ChatId, Content, Time) values " +
                  $"('{message.UserId}','{message.ChatId}','{message.Content}','{message.Time}')");
    }

    [HttpPost("CreateChat")]
    public void CreateChat(Chat chat)
    {
        InsertSql(
            $"insert into Chat (ChatId , UserId, ContactId) values ('{chat.ChatId}' ,'{chat.UserId}' , '{chat.ContactId}')");
    }

    [HttpGet("GetAcc")]
    public async Task<AccUser> GetAcc(string username, string password)
    {
        IEnumerable<AccUser> user = new List<AccUser>();
        try
        {
            var con = new SqlConnection(ConnectionString);
            con.Open();
            user = con.Query<AccUser>($"Select * from AccUser where Username = '{username}' and Password = '{password}' ");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return user.ToList()[0];
    }

    [HttpGet("GetContacts")]
    public async Task<List<Contact>> GetContacts(string userId)
    {
        IEnumerable<Contact> contacts = new List<Contact>();
        try
        {
            var con = new SqlConnection(ConnectionString);
            con.Open();
            contacts = con.Query<Contact>($"Select * from Contact where UserId = '{userId}'");
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return contacts.ToList();
    }

    [HttpGet("GetChat")]
    public async Task<Chat> GetChat(string userId, string contactId)
    {
        IEnumerable<Chat> chat = new List<Chat>();
        try
        {
            var con = new SqlConnection(ConnectionString);
            con.Open();
            chat = con.Query<Chat>($"Select * from Chat where UserId = '{userId}' and ContactId = '{contactId}' ");
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
            var con = new SqlConnection(ConnectionString);
            con.Open();
            messages = con.Query<Message>($"Select * from Message where ChatId = '{chatId}' ");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return messages.ToList();
    }

    [HttpDelete("DeleteContact")]
    public void DeleteContact(string contactId)
    {
        InsertSql($"Delete Contact where ContactId = '{contactId}'");
    }

    [HttpDelete("DeleteMessages")]
    public void DeleteMessages(string chatId, string userId)
    {
        InsertSql($"Delete Message where ChatId = '{chatId}' and UserId = '{userId}'");
        InsertSql($"Delete Chat where ChatId = '{chatId}'");
    }

    [HttpDelete("DeleteAcc")]
    public void DeleteAcc(string username, string password)
    {
        InsertSql($"Delete UserAcc where Username = '{username}' and Password = '{password}'");
    }

    [HttpPost("CreateAcc")]
    public async Task<AccUser> CreateAcc(string username, string password)
    {
        try
        {
            string userId;
            IEnumerable<int> count;
            var con = new SqlConnection(ConnectionString);
            con.Open();
            do
            {
                userId = RandomString();
                count = con.Query<int>($"Select COUNT(*) from AccUser where UserId = '{userId}'");
            } while (count.ToArray()[0] > 0);

            con.Query("Insert Into AccUser (UserId ,Username ,Password)" +
                      $"VALUES ('{userId}','{username}','{password}')");
            var account = con.Query<AccUser>($"Select * from AccUser where UserId = '{userId}'");
            con.Close();
            return account.ToList()[0];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AccUser();
        }
        
    }

    [HttpGet("ControlCreate")]
    public async Task<bool> ControlCreate(string username)
    {
        try
        {
            IEnumerable<int> count;
            var con = new SqlConnection(ConnectionString);
            con.Open();
            count = con.Query<int>(
                $"Select COUNT(*) from AccUser where Username = '{username}'");
            
            con.Close();
            return count.ToArray()[0] == 0;

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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            userId = new string(Enumerable.Repeat(chars, 45)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } while (false);

        return userId;
        //check if UserId Already in use 
    }


    private void InsertSql(string sql)
    {
        try
        {
            var con = new SqlConnection(ConnectionString);
            con.Open();
            con.Query(sql);
            con.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}