using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HelpDeskDbContext _context;

        public ChatHub(HelpDeskDbContext helpDeskDbContext)
        {
            _context = helpDeskDbContext;
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task SendMessage(int chatId, string message, string userId)
        {
            Console.WriteLine($"SendMessage called with chatId: {chatId}, message: {message}, userId: {userId}");

            try
            {
                Console.WriteLine($"SendMessage called with chatId: {chatId}, message: {message}, userId: {userId}");
                var chat = await _context.Chats.FindAsync(chatId);

                if (chat == null)
                {
                    Console.WriteLine($"Chat with ID {chatId} not found.");
                    return;
                }

                var newMessage = new Message
                {
                    Content = message,
                    TimeSent = DateTime.Now,
                    SenderId = userId,
                    Chat = chat
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                Console.WriteLine("Message saved to database successfully.");

                await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", userId, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }

        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
