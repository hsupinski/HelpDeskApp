using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;

namespace HelpDeskAppTests.TestServices
{
    internal class TestLogService : ILogService
    {
        public async Task<List<ChatLogsInfoViewModel>> CreateChatLogsInfoViewModel(string userId)
        {
            var model = new List<ChatLogsInfoViewModel>
            {
                new ChatLogsInfoViewModel
                {
                    TopicName = "Test",
                    ChatId = 1,
                    StartTime = DateTime.Now,
                    EndTime = null,
                    IsServiced = false
                }
            };

            return model;
        }

        public async Task<List<LogDetailsViewModel>> GetChatLogsByChatId(int chatId)
        {
            var model = new List<LogDetailsViewModel>
            {
                new LogDetailsViewModel
                {
                    EventTime = DateTime.Now,
                    EventType = "Test",
                    Username = "Test",
                    Content = "Test",
                    TopicName = "Test"
                }
            };

            return model;
        }

        public Task RemoveUserLogs(string userId, int chatId)
        {
            throw new NotImplementedException();
        }
    }
}