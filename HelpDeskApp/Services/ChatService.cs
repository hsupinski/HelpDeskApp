using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;

namespace HelpDeskApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IAccountService _accountService;
        private readonly IChatRepository _chatRepository;
        private readonly ITopicService _topicService;
        private readonly IDepartmentService _departmentService;
        public ChatService(IChatRepository chatRepository, IAccountService accountService, ITopicService topicService, 
            IDepartmentService departmentService)
        {
            _chatRepository = chatRepository;
            _accountService = accountService;
            _topicService = topicService;
            _departmentService = departmentService;
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
                Participants = new List<ChatParticipation>
                {
                    new ChatParticipation
                    {
                        ParticipantId = userId,
                        IsHidden = false // By default chats are created by non-admin users
                    }
                }
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

        public async Task<Chat> GetActiveChatByUserId(string userId)
        {
            return await _chatRepository.GetActiveChatByUserId(userId);
        }

        public async Task<Chat> GetActiveConsultantChats(string userId)
        {
            /*
             * Active consultant chat:
             * - Is not closed (EndTime is null)
             * - IsServiced is true
             * - Participant list contains the consultant
             * - Chat topic belongs to the consultant's department
             */

            var departmentList = await _departmentService.GetUserDepartments(userId);
            var topicList = new List<Topic>();

            foreach (var department in departmentList)
            {
                var topics = await _topicService.GetTopicsByDepartmentId(department.Id);
                topicList.AddRange(topics);
            }

            return await _chatRepository.GetActiveConsultantChats(userId, topicList);
        }

        public async Task<Chat> GetAvailableConsultantChats(string userId)
        {
            return await _chatRepository.GetAvailableConsultantChats(userId);
        }

        public async Task LeaveChatAsync(string userId)
        {
            await _chatRepository.LeaveChatAsync(userId);
        }
    }
}
