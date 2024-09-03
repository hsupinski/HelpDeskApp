using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;

namespace HelpDeskAppTests.TestServices
{
    internal class TestChatService : IChatService
    {
        public async Task<bool> CheckChatValidity(int chatId, string userId)
        {
            if(chatId < 0)
            {
                return false;
            }

            return true;
        }

        public async Task<Chat> CreateChatAsync(string userId, int topicId)
        {
            var chat = new Chat();
            return chat;
        }

        public async Task<ChatViewModel> CreateChatViewModel(Chat chat, string userId)
        {
            var model = new ChatViewModel();
            return model;
        }

        public Task FinishChatAsync(string userId, bool isSaved)
        {
            throw new NotImplementedException();
        }

        public async Task<Chat> GetActiveChatByUserId(string userId)
        {
            var chat = new Chat();

            if(userId.StartsWith("1"))
            {
                return chat;
            }

            return null;
        }

        public Task<List<Chat>> GetAllChatsByTopicName(string topic)
        {
            throw new NotImplementedException();
        }

        public Task<List<Chat>> GetAllOpenChats(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Chat>> GetAvailableConsultantChats(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Chat> GetChatById(int chatId)
        {
            var chat = new Chat();
            chat.Id = chatId;
            return chat;
        }

        public Task<string> GetChatTopic(int chatId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetUsersInChat(int chatId)
        {
            throw new NotImplementedException();
        }

        public async Task JoinChatAsConsultant(int chatId, string userId)
        {
            return;
        }

        public Task LeaveChatAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task RedirectToDifferentTopic(int chatId, string newTopic, string moreInfo)
        {
            throw new NotImplementedException();
        }

        public Task SetChatSaved(int chatId, bool isSaved)
        {
            throw new NotImplementedException();
        }
    }
}
