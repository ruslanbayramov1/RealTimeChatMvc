using ChatSignalR.Contexts;
using ChatSignalR.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatSignalR.Hubs
{
    public class ChatHub : Hub
    {
        private static List<ChatMessage> Messages => SharedDb.Messages;
        private static List<ChatUser> Users => SharedDb.Users;

        public override async Task OnConnectedAsync()
        {
            ChatUser? user = new ChatUser();
            if (Context.User != null)
            {
                user = Users.FirstOrDefault(x => x.UserName == Context.User?.FindFirst(ClaimTypes.Name)?.Value);

                if (user != null)
                { 
                    user.ConnectionId = Context.ConnectionId;
                }
            }

            var sendedUsers = Users.Where(x => x.Id != user.Id && x.ConnectionId != null);

            await Clients.Caller.SendAsync("RecivedAllMessagesByCaller", Messages);
            await Clients.Caller.SendAsync("ReceivedAllUsersByCaller", sendedUsers);

            await Clients.All.SendAsync("JoinedUser", user);
        }

        public async Task SendMessage(string msgFromClient)
        {
            #region Notes
            // 1. In client side we will invoke the SendMessage method and we will pass the input value like this: connection.invoke("SendMessage", inpVal);
            // 2. After that we need to send back the message to client, SendAsync("ReceivedMessage", message) means we sends message back to client with "ReceivedMessage" method.
            // 3. With connection.on("ReceivedMessage", () => {}) we will accept the message from hub
            #endregion
            ChatMessage message = new ChatMessage();
            message.Message = msgFromClient;
            message.User = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            message.ProfileImageUrl = Context.User.FindFirst("imageurl").Value;
            message.SendedAt = DateTime.Now;

            SharedDb.Messages.Add(message);

            await Clients.All.SendAsync("ReceivedMessage", message);
        }
    }
}
