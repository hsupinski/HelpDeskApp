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

        public async Task JoinChatAsConsultant(int chatId, string userId)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            var userRoles = await _accountService.GetUserRolesAsync(user);

            if(userRoles.Contains("Admin"))
            {
                // Don't add admin to the participant list
                return;
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            
            var chatParticipation = new ChatParticipation
            {
                ChatId = chatId,
                ParticipantId = userId,
                IsHidden = false
            };

            chat.Participants.Add(chatParticipation);

            await _chatRepository.UpdateChatAsync(chat);
        }

        public async Task<ChatViewModel> CreateChatViewModel(Chat chat, string userId)
        {
            var availableTopics = await _topicService.GetAllAsync();
            var currentTopic = chat.Topic;

            // Remove the current topic from the list of available topics

            var topics = availableTopics.Where(t => t.Name != currentTopic).ToList();

            var usersInChatroom = await _chatRepository.GetUserIdsInChat(chat.Id);

            var usernames = new List<string>();

            foreach(var _userId in usersInChatroom)
            {
                var username = await _accountService.GetUsernameById(_userId);
                var userRoles = await _accountService.GetUserRolesAsync(await _accountService.GetUserByIdAsync(_userId));

                if (userRoles.Contains("Admin"))
                {
                    // Don't show Admins in the chatroom
                    continue;
                }

                usernames.Add(username);
            }

            var model = new ChatViewModel
            {
                ChatId = chat.Id,
                UserId = userId,
                UserName = await _accountService.GetUsernameById(userId),
                Messages = chat.Messages.ToList(),
                AvailableTopics = topics,
                UsersInChatroom = usernames
            };

            return model;
        }

        public async Task<Chat> GetActiveChatByUserId(string userId)
        {
            return await _chatRepository.GetActiveChatByUserId(userId);
        }

        public async Task<List<Chat>> GetAvailableConsultantChats(string userId)
        {
            /*
             * Available consultant chat:
             * - Is not closed (EndTime is null)
             * - IsServiced is false
             * - Participant list does not contain the consultant
             * - Chat topic belongs to the consultant's department
             */

            var departmentList = await _departmentService.GetUserDepartments(userId);
            var topicList = new List<Topic>();

            foreach (var department in departmentList)
            {
                var topics = await _topicService.GetTopicsByDepartmentId(department.Id);
                topicList.AddRange(topics);
            }

            return await _chatRepository.GetAvailableConsultantChats(userId, topicList);
        }

        public async Task<List<string>> GetUsersInChat(int chatId)
        {
            return await _chatRepository.GetUserIdsInChat(chatId);
        }

        public async Task LeaveChatAsync(string userId)
        {
            await _chatRepository.LeaveChatAsync(userId);
        }
        public async Task FinishChatAsync(string userId, bool isSaved)
        {
            await _chatRepository.FinishChatAsync(userId, isSaved);
        }

        public async Task RedirectToDifferentTopic(int chatId, string topicId)
        {
            await _chatRepository.RedirectToDifferentTopic(chatId, topicId);
        }

        public async Task<Chat> GetChatById(int chatId)
        {
            return await _chatRepository.GetChatByIdAsync(chatId);
        }

        public async Task<List<Chat>> GetAllOpenChats(string userId)
        {
            return await _chatRepository.GetAllOpenChats(userId);
        }
    }
}
