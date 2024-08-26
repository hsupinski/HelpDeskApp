using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;

namespace HelpDeskApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
        public async Task<ChatViewModel> CreateChatAsync(string userId)
        {
            var chat = new Chat
            {
                Topic = "New Chat", // TODO: Implement topic
                StartTime = DateTime.Now,
                EndTime = null,
                Messages = new List<Message>(),
            };

            chat.Participants = new List<ChatParticipation>
            {
                new ChatParticipation
                {
                    ChatId = chat.Id,
                    ParticipantId = userId,
                }
            };

            await _chatRepository.CreateChatAsync(chat);

            var chatViewModel = new ChatViewModel
            {
                ChatId = chat.Id,
                UserId = userId,
                Messages = chat.Messages.ToList(),
            };

            return chatViewModel;
        }
    }
}
