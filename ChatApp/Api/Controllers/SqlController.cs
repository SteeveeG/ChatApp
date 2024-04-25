using System.Data.SqlClient;
 using Dapper;
 using Library.Model;
 using Microsoft.AspNetCore.Mvc;

 namespace Api.Controllers;
 
 [ApiController]
 [Route("[controller]")]
 public class SqlController : ControllerBase
 {
     private string connectionString;
     public SqlController()
     {
         GetLocalHost();
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
                         $" (UserId, ContactId, LastMessage, LastMessageTime, ContactUsername) values " +
                         $"('{contact.UserId}','{contact.ContactId}','{contact.LastMessage}',convert(datetime, '{contact.LastMessageTime}','{contact.ContactUsername}')");
     }
 
     [HttpPost("AddMessage")]
     public async void AddMessage(Message message)
     {
         await InsertSql($"insert into Message" +
                         $" (UserId, ChatId, Content, Time) values " +
                         $"('{message.UserId}','{message.ChatId}','{message.Content}',convert(datetime, '{message.Time}', 104))");
     }
 
     [HttpPost("CreateContact")]
     public async Task<Contact> CreateContact(string userId, string contactId)
     {
         string username;
         Contact contact;
         try
         {
             var con = new SqlConnection(connectionString);
             con.Open();
             username = con.Query<string>($"Select Username from AccUser where UserId = '{contactId}'").ToArray()[0];
             con.Query(
                 $"Insert into Contact (ContactId, UserId , LastMessage , LastMessageTime ,ContactUsername) values " +
                 $"('{contactId}', '{userId}', ' ' ,convert(datetime, '{DateTime.Now}', 104) ,'{username}')");
             contact = con
                 .Query<Contact>($"Select * from Contact Where ContactId = '{contactId}' and UserId = '{userId}'")
                 .ToArray()[0];
             con.Close();
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

     [HttpPost("ChangeUsername")]
     public async Task<bool> ChangeUsername(string newusername, string userId)
     {
         int count;
         try
         {
             var con = new SqlConnection(connectionString);
             con.Open();
             count = con.Query<int>($"Select Count(*) from AccUser where Username = '{newusername}'").ToArray()[0];
             if (count != 0)
             {
                 return false;
             }
             con.Query($"UPDATE AccUser SET Username = '{newusername}' WHERE UserId = '{userId}'");
             con.Query($"UPDATE Contact SET ContactUsername = '{newusername}' WHERE ContactId = '{userId}'");
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
     public async Task<bool> ChangePassword(string userId,string password)
     {
         try
         {
             var con = new SqlConnection(connectionString);
             con.Open();
             con.Query($"UPDATE AccUser SET Password = '{password}' WHERE UserId = '{userId}'");
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
             count = con.Query<int>($"Select Count(*) from Chat where ChatId = '{chatId}'").ToArray()[0];
             con.Close();
         } while (count != 0);
 
         var chat = new Chat
         {
             UserId = userId,
             ContactId = contactId,
             ChatId = chatId
         };
         await InsertSql(
             $"insert into Chat (ChatId , UserId, ContactId) values ('{chat.ChatId}' ,'{chat.UserId}' , '{chat.ContactId}')");
 
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
 
     [HttpGet("GetAcc")]
     public async Task<AccUser> GetAcc(string username, string password)
     {
         IEnumerable<AccUser> user = new List<AccUser>();
         try
         {
             var con = new SqlConnection(connectionString);
             con.Open();
             user = con.Query<AccUser>(
                 $"Select * from AccUser where Username = '{username}' and Password = '{password}' ");
             con.Close();
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
         }
 
         return user.ToList()[0];
     }
 
     [HttpGet("GetUserContacts")]
     public async Task<List<Contact>> GetUserContacts(string userId)
     {
         IEnumerable<Contact> contacts = new List<Contact>();
         try
         {
             var con = new SqlConnection(connectionString);
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
             var con = new SqlConnection(connectionString);
             con.Open();
             chat = con.Query<Chat>($"Select * from Chat where UserId = '{userId}' and ContactId = '{contactId}' ");
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
             messages = con.Query<Message>($"Select * from Message where ChatId = '{chatId}' ");
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
         if (!await CheckIfContactUsed(contactId, userId))
         {
             var chat = await GetChat(userId, contactId);
             DeleteMessages(chat.ChatId , userId);
             DeleteMessages(chat.ChatId , contactId);
             OwnDeleteUserAcc(contactId);
         }
         await InsertSql($"Delete Contact where ContactId = '{contactId}' and UserId='{userId}'");
 
     }

     


     [HttpDelete("DeleteMessages")]
     public async void DeleteMessages(string chatId, string userId)
     {
         await InsertSql($"Delete Message where ChatId = '{chatId}' and UserId = '{userId}'");
         await InsertSql($"Delete Chat where ChatId = '{chatId}'");
     }


     [HttpDelete("DeleteAllContactsFromUser")]
     public async Task<bool> DeleteAllContactsFromUser(string userId)
     {
        return await InsertSql($"Delete Contact where UserId='{userId}'");
     }
     
     [HttpDelete("OwnDeleteAcc")]
     public async Task<bool> OwnDeleteAcc(string userId)
     {
         //todo : Clean This Mess :( 
       // var result1 = await DeleteAllContactsFromUser(userId);
        var result2 = await InsertSql($"UPDATE AccUser SET Username = 'Account Deleted', Password='Account Deleted' WHERE UserId = '{userId}'");
        var contacts = await GetUserContacts(userId);
        foreach (var contact in contacts)
        {
             DeleteContact(contact.ContactId , userId);
        }
        return result2; 
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
     private async void OwnDeleteUserAcc(string contactId)
     {
         await InsertSql($"Delete UserAcc where UserId = '{contactId}'");
     }
     private async Task<bool> CheckIfContactUsed(string userId, string contactId)
     {
         int count;
         try
         {
             var con = new SqlConnection(connectionString);
             con.Open();
             count = con.Query<int>(
                 $"Select Count(*) from Contact where ContactId = '{userId}' and UserId = '{contactId}'").ToArray()[0];
             con.Close();
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
             throw;
         }
 
         return count > 1;
     }
     private void GetLocalHost()
     {
         var myServer = Environment.MachineName;
         connectionString = @$"Server={myServer}\MSSQL2022;Database=ChatApp;Trusted_Connection=True;MultipleActiveResultSets=True";
     }


 }