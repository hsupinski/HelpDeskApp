using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace HelpDeskApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HelpDeskDbContext _context;
        private readonly IAccountService _accountService;

        public ChatHub(HelpDeskDbContext helpDeskDbContext, IAccountService accountService)
        {
            _context = helpDeskDbContext;
            _accountService = accountService;
        }

        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync("UserJoined", Context.UserIdentifier);
        }

        public async Task SendMessage(string chatId, string message, string userId)
        {
            Console.WriteLine($"SendMessage called with chatId: {chatId}, message: {message}, userId: {userId}");

            try
            {
                Console.WriteLine($"SendMessage called with chatId: {chatId}, message: {message}, userId: {userId}");
                int chatIdAsInt = Int32.Parse(chatId);

                var chat = await _context.Chats.FindAsync(chatIdAsInt);

                if (chat == null)
                {
                    Console.WriteLine($"Chat with ID {chatId} not found.");
                    return;
                }

                var username = await _accountService.GetUsernameById(userId);

                var newMessage = new Message
                {
                    Content = message,
                    TimeSent = DateTime.Now,
                    SenderId = userId,
                    SenderUsername = username,
                    Chat = chat
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                Console.WriteLine("Message saved to database successfully.");

                await Clients.Group(chatIdAsInt.ToString()).SendAsync("ReceiveMessage", userId, message, username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw new HubException($"Error sending message: {ex.Message}");
            }

        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync("UserLeft", Context.UserIdentifier);
        }
    }
}
