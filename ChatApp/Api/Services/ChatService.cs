using System.Diagnostics;
using System.Text.Json;
using Api.Controllers;
using Grpc.Core;
using Library.Model;
using Request = Grpc.Protos.Request;
using Response = Grpc.Protos.Response;
using Type = Library.Enum.Type;

namespace Api.Services;

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
            Debug.Write("Dispose");
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
                    if (await ControlId(request))
                    {
                        var subscriberData = JsonSerializer.Serialize(subscriber);
                        await responseStream.WriteAsync(new Response
                        {
                            Data = subscriberData
                        });
                    }
                    isChanged = false;
                }

                Thread.Sleep(50);
            }

            if (context.CancellationToken.IsCancellationRequested)
            {
                Unsubscribe();
            }
        }

        private async Task<bool> ControlId(Request request)
        {
            switch (subscriber.Type)
            {
                case Type.Message:
                    var result = await sqlController.GetChatIds(request.UserId);
                     if (!result.Exists(s => s == subscriber.Message.ChatId))
                     {
                         return false;
                     }
                    return request.UserId != subscriber.Message.UserId;
                case Type.CreatedContact:
                    return request.UserId == subscriber.Contact.UserId;
                case Type.Delete:
                    var contacts = await sqlController.GetUserContacts(request.UserId);
                    if ( contacts.Exists(t => t.UserId == subscriber.AccUser.UserId) ||
                         contacts.Exists(t => t.CreatedContactUserId == subscriber.AccUser.UserId))
                    {
                        return true;
                    }
                    return false;
                
            }

            return true;
        }
    }
