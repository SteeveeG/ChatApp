using Grpc.Core;
using Grpc.Protos;
using Newtonsoft.Json;


namespace Grpc.Services
{
    public class ChatService : Protos.ChatService.ChatServiceBase
    {
        private bool newMessage = false;
        private msg Message = new msg();

        public override async Task<msg> ServerReceiveMessage(msg request, ServerCallContext context)
        {
            Message = request;
            newMessage = true;

            return new msg();
        }


        public override async Task ServerSendMessage(msg request, IServerStreamWriter<msg> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                if (!newMessage || Message.Id == request.Id)
                {
                    continue;
                }

                await responseStream.WriteAsync(Message);
                newMessage = false;
            }
        }
    }
}