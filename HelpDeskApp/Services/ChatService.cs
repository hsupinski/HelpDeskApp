using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;
using System.Security.Claims;

namespace HelpDeskApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IAccountService _accountService;
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository, IAccountService accountService)
        {
            _chatRepository = chatRepository;
            _accountService = accountService;
        }
        public async Task<Chat> CreateChatAsync(string userId)
        {
            var chat = new Chat
            {
                Topic = "New Chat", // TODO: Implement topic
                StartTime = DateTime.Now,
                EndTime = null,
                Messages = new List<Message>(),
            };

            await _chatRepository.CreateChatAsync(chat);

            return chat;
        }

        public async Task<ChatViewModel> CreateChatViewModel(Chat chat, string userId)
        {
            var model = new ChatViewModel
            {
                ChatId = chat.Id,
                UserId = userId,
                UserName = await _accountService.GetUsernameById(userId),
                Messages = chat.Messages.ToList(),
            };

            return model;
        }

        // Get the chat if it's not closed (EndTime is null)
        public async Task<Chat> GetActiveChatByUserId(string userId)
        {
            return await _chatRepository.GetActiveChatByUserId(userId);
        }

        public async Task LeaveChatAsync(string userId)
        {
            await _chatRepository.LeaveChatAsync(userId);
        }
    }
}
