using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Protos;

var set = new Action<msg>(s);
var channel = GrpcChannel.ForAddress("https://localhost:7277/");
var sendReceiveClient = new ChatService.ChatServiceClient(channel);

string? Message;
var allMessages = new List<string>();

Task.Run(async () => { await Task.Run(() => OpenStream(set)); });


while (true)
{
    do
    {
        Message = Console.ReadLine();
    } while (string.IsNullOrEmpty(Message));
    allMessages.Add(Message);
    WriteInStream(Message  , "Console 2");
}


void s(msg msg)
{
    Console.WriteLine($"{msg.Id}: {msg.Message}");
}

async Task OpenStream(Action<msg> update)
{
    Console.Write("Connection Building...");
    var channel = GrpcChannel.ForAddress("https://localhost:7049/");
    sendReceiveClient = new ChatService.ChatServiceClient(channel);
    var result = sendReceiveClient.ServerSendMessage(
        new msg{Id = "Console 2"});
    do
    {
        Console.Write("Connection Builded\n");
        while (await result.ResponseStream.MoveNext())
        {
            var currentData = result.ResponseStream.Current;
            update(currentData);
        }
    } while (true);
}

void WriteInStream(string message , string id)
{
    sendReceiveClient = new ChatService.ChatServiceClient(GrpcChannel.ForAddress("https://localhost:7049/"));
    sendReceiveClient.ServerReceiveMessage(new msg()
    {
        Message = message,
        Id = id
    });
}