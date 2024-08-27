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
        private readonly ITopicService _topicService;
        public ChatService(IChatRepository chatRepository, IAccountService accountService, ITopicService topicService)
        {
            _chatRepository = chatRepository;
            _accountService = accountService;
            _topicService = topicService;
        }
        public async Task<Chat> CreateChatAsync(string userId, int topicId)
        {
            var topic = await _topicService.GetByIdAsync(topicId);

            var chat = new Chat
            {
                Topic = topic.Name,
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
