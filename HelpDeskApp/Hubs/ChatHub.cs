using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;


namespace HelpDeskApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HelpDeskDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IChatService _chatService;
        

        public ChatHub(HelpDeskDbContext helpDeskDbContext, IAccountService accountService, IChatService chatService)
        {
            _context = helpDeskDbContext;
            _accountService = accountService;
            _chatService = chatService;
        }

        public async Task JoinChat(string chatId)
        {
            var user = await _accountService.GetUserByIdAsync(Context.UserIdentifier);
            var userRoles = await _accountService.GetUserRolesAsync(user);

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            var username = await _accountService.GetUsernameById(Context.UserIdentifier);


            if (!userRoles.Contains("Admin"))
            {
                var chat = await _chatService.GetChatById(Int32.Parse(chatId));

                if (chat.Participants.Count == 0)
                {
                    // new chat, send message to consultant panel

                    var model = new JoinChatItemViewModel
                    {
                        chatId = chat.Id,
                        topicName = chat.Topic,
                        usernamesInChat = new List<string>(),
                        isServiced = chat.IsServiced
                    };

                    foreach (var userId in await _chatService.GetUsersInChat(Int32.Parse(chatId)))
                    {
                        var _userRoles = await _accountService.GetUserRolesAsync(await _accountService.GetUserByIdAsync(userId));
                        if (!_userRoles.Contains("Admin"))
                            model.usernamesInChat.Add(await _accountService.GetUsernameById(userId));
                    }

                    await Clients.Group("ConsultantPanel").SendAsync("NewChatCreated", model);
                    Console.WriteLine("New chat created, sent to consultant panel.");
                }

                using (LogContext.PushProperty("UserId", Context.UserIdentifier))
                using (LogContext.PushProperty("ChatId", chatId))
                using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(Int32.Parse(chatId))))
                {
                    Log.Information("User joined chat");
                }

                await Clients.Group(chatId).SendAsync("UserJoined", Context.UserIdentifier, username);
                await BroadcastUserList(chatId);
            }
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

                var user = await _accountService.GetUserByIdAsync(userId);
                var userRoles = await _accountService.GetUserRolesAsync(user);
                var username = await _accountService.GetUsernameById(userId);

                var newMessage = new Message
                {
                    Content = message,
                    TimeSent = DateTime.Now,
                    SenderId = userId,
                    SenderUsername = username,
                    Chat = chat
                };

                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("ChatId", chatId))
                using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(chatIdAsInt)))
                using (LogContext.PushProperty("Content", message))
                {
                    Log.Information("Message sent");
                }

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();

                await Clients.Group(chatIdAsInt.ToString()).SendAsync("ReceiveMessage", userId, message, username);
                await BroadcastUserList(chatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw new HubException($"Error sending message: {ex.Message}");
            }

        }

        public async Task LeaveChat(string chatId, bool isChatSaved)
        {
            var user = await _accountService.GetUserByIdAsync(Context.UserIdentifier);
            var userRoles = await _accountService.GetUserRolesAsync(user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            var username = await _accountService.GetUsernameById(Context.UserIdentifier);

            var chat = await _chatService.GetChatById(Int32.Parse(chatId));

            if (chat.Participants.Count == 0)
            {
                // chat is being removed, send message to consultant panel

                await _chatService.SetChatSaved(Int32.Parse(chatId), isChatSaved);

                var model = new JoinChatItemViewModel
                {
                    chatId = chat.Id,
                    topicName = chat.Topic,
                    usernamesInChat = new List<string>(),
                    isServiced = chat.IsServiced
                };

                model.usernamesInChat.Add(username);

                await Clients.Group("ConsultantPanel").SendAsync("ChatRemoved", model);
                Console.WriteLine("Chat removed, sent to consultant panel.");
            }

            if (!userRoles.Contains("Admin"))
            {
                using (LogContext.PushProperty("UserId", Context.UserIdentifier))
                using (LogContext.PushProperty("ChatId", chatId))
                using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(Int32.Parse(chatId))))
                {
                    Log.Information("User left chat");
                }

                await Clients.Group(chatId).SendAsync("UserLeft", Context.UserIdentifier, username);
                await BroadcastUserList(chatId);
            }

        }

        public async Task IsIssueSolved(string chatId)
        {
            using (LogContext.PushProperty("UserId", Context.UserIdentifier))
            using (LogContext.PushProperty("ChatId", chatId))
            using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(Int32.Parse(chatId))))
            {
                Log.Information("Asked if issue is solved");
            }

            var userList = await _chatService.GetUsersInChat(Int32.Parse(chatId));
            var userId = userList.FirstOrDefault(user => _accountService.GetUserRolesAsync(
            _accountService.GetUserByIdAsync(user).Result).Result.Contains("User"));

            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "", "No user found in chat, cannot ask if issue is solved.", "System");
                return;
            }

            await Clients.Group(chatId).SendAsync("IssueSolvedQuestion", userId);

        }

        public async Task RespondToIssueSolved(string chatId, bool isSolved)
        {        
            if(isSolved)
            {
                using (LogContext.PushProperty("UserId", Context.UserIdentifier))
                using (LogContext.PushProperty("ChatId", chatId))
                using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(Int32.Parse(chatId))))
                {
                    Log.Information("Issue solved");
                }

                await Clients.Group(chatId).SendAsync("ReceiveMessage", "", "The issue has been solved. The chat will now close.", "System");
                await Clients.Group(chatId).SendAsync("CloseChat");
            }
            else
            {
                using (LogContext.PushProperty("UserId", Context.UserIdentifier))
                using (LogContext.PushProperty("ChatId", chatId))
                using (LogContext.PushProperty("Topic", await _chatService.GetChatTopic(Int32.Parse(chatId))))
                {
                    Log.Information("Issue not solved");
                }

                await Clients.Group(chatId).SendAsync("ReceiveMessage", "", "The issue has not been solved. The chat will continue.", "System");    
            }
        }

        public async Task ChangeChatTopic(string chatId, string topicId, string moreInfo)
        {
            await _chatService.RedirectToDifferentTopic(Int32.Parse(chatId), topicId, moreInfo);

            var topicName = await _chatService.GetChatTopic(Int32.Parse(chatId));

            using (LogContext.PushProperty("UserId", Context.UserIdentifier))
            using (LogContext.PushProperty("ChatId", chatId))
            using (LogContext.PushProperty("Topic", topicName))
            {
                Log.Information("Chat topic changed");
            }


            var model = new JoinChatItemViewModel
            {
                chatId = Int32.Parse(chatId),
                topicName = topicName,
                usernamesInChat = new List<string>()
            };

            foreach (var userId in await _chatService.GetUsersInChat(Int32.Parse(chatId)))
            {
                model.usernamesInChat.Add(await _accountService.GetUsernameById(userId));
            }

            await Clients.Group("ConsultantPanel").SendAsync("NewChatCreated", model);
            Console.WriteLine("Chat topic changed, sent new chat created notification to consultant panel.");


            var chat = await _chatService.GetChatById(Int32.Parse(chatId));

            await Clients.Group(chatId).SendAsync("TopicChanged", topicId);
            await Clients.Group("ConsultantPanel").SendAsync("TopicChanged", chat);
        }

        public async Task BroadcastUserList(string chatId)
        {
            var usersInChat = await _chatService.GetUsersInChat(Int32.Parse(chatId));

            var idWithUsername = new List<IdWithUsernameViewModel>();

            foreach (var userId in usersInChat)
            {
                var username = await _accountService.GetUsernameById(userId);
                idWithUsername.Add(new IdWithUsernameViewModel { id = userId, username = username });
            }
            await Clients.Group(chatId).SendAsync("UpdateUserList", idWithUsername);
        }

        public async Task JoinConsultantPanel()
        {
            try
            {
                Console.WriteLine($"Consultant with ID {Context.UserIdentifier} joined the consultant panel.");
                await Groups.AddToGroupAsync(Context.ConnectionId, "ConsultantPanel");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in JoinConsultantPanel: {ex.Message}");
                await Clients.Caller.SendAsync("ErrorOccurred", "An error occurred while fetching chats.");
            }
        }

        public async Task LeaveConsultantPanel()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ConsultantPanel");
        }
    }
}
