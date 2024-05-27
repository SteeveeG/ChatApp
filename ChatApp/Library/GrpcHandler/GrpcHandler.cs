using System.Diagnostics;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Protos;

namespace Library.GrpcHandler;

public class GrpcHandler
{
    public static bool Unsubscribe = false;
    public static async Task OpenStream(Action<string> update ,string userId)
    {
        string currentData;
        var channel = GrpcChannel.ForAddress("https://localhost:7049");
        var client = new ChatService.ChatServiceClient(channel);
        var result = client.OpenStream(new Request
        {
            UserId = userId
        });
        do
        {
            while (await result.ResponseStream.MoveNext())
            {
                Thread.Sleep(50);
                currentData = result.ResponseStream.Current.Data;
                Debug.WriteLine(currentData);
                update(currentData);
            }
            if (Unsubscribe)
            {
                result.Dispose();
                break;
            }
        } while (true);
    }
}