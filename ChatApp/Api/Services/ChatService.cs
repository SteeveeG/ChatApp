using System.Diagnostics;
using System.Text.Json;
using Api.Controllers;
using Azure;
using Azure.Core;
using Grpc.Core;
using Library.Model;
using Library;
using Request = Grpc.Protos.Request;
using Response = Grpc.Protos.Response;
using Type = Library.Enum.Type;

namespace Api.Services
{
    public class ChatService : Grpc.Protos.ChatService.ChatServiceBase, IObserver<Subscriber>

    {
        private IDisposable unsubscriber;
        private SqlController sqlController;
        private bool isChanged;
        private Subscriber subscriber;

        public ChatService(SqlController sqlController)
        {
            this.sqlController = sqlController;
        }

        public void OnCompleted()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public void OnError(Exception error)
        {
            Debug.Write(error);
        }

        public void OnNext(Subscriber value)
        {
            subscriber = value;
            isChanged = true;
        }

        private void Subscribe(IObservable<Subscriber> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }

        public override async Task OpenStream(Request request, IServerStreamWriter<Response> responseStream,
            ServerCallContext context)
        {
            Subscribe(sqlController);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                if (isChanged)
                {
                    switch (subscriber.Type)
                    {
                        case Type.Message:
                            var result = await sqlController.GetChatIds(request.UserId);
                            var id = result.Find(ids => ids == subscriber.Chat.ChatId);
                            if (id == null)
                            {
                                return;
                            }
                            break;
                        case Type.CreatedChat:
                            if (request.UserId != subscriber.Chat.UserId)
                            {
                                return;
                            }
                            break;
                        case Type.CreatedContact:
                            if (request.UserId != subscriber.Contact.UserId)
                            {
                                return;
                            }
                            break;
                        case Type.DeleteContact:
                            //not implented
                            break;
                        case Type.DeleteAccount:
                            //not implented
                            break;
                    }
                    var subscriberData = JsonSerializer.Serialize(subscriber);
                    await responseStream.WriteAsync(new Response
                    {
                        Data = subscriberData
                    });
                    isChanged = false;
                }

                Thread.Sleep(50);
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                Unsubscribe();
            }
        }
        
        // public override async Task<msg> ServerReceiveMessage(msg request, ServerCallContext context)
        // {
        //     Message = request;
        //     newMessage = true;
        //
        //     return new msg();
        // }
        //
        //
        // public override async Task ServerSendMessage(msg request, IServerStreamWriter<msg> responseStream, ServerCallContext context)
        // {
        //     while (!context.CancellationToken.IsCancellationRequested)
        //     {
        //         if (!newMessage || Message.Id == request.Id)
        //         {
        //             continue;
        //         }
        //
        //         await responseStream.WriteAsync(Message);
        //         newMessage = false;
        //     }
        // }
    }
}