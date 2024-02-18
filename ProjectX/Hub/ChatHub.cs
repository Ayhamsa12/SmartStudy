using Microsoft.AspNetCore.SignalR;

namespace ProjectX.Hup
{
    public class ChatHub : Hub
    {
        public async Task SendQuestion(string title, string content)
        {
            // Send the new question to all connected clients
            await Clients.All.SendAsync("ReceiveQuestion", title, content);
        }

        public async Task SendAnswer(int questionId, string replyContent, string userName)
        {
            await Clients.All.SendAsync("ReceiveAnswer", questionId, replyContent, userName);
        }

    }
}
