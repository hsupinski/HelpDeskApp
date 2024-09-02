using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;

namespace HelpDeskApp.Services
{
    public class LogService : ILogService
    {
        private readonly IAccountService _accountService;
        private readonly IDepartmentService _departmentService;
        private readonly IChatService _chatService;
        private readonly ILogRepository _logRepository;

        public LogService(IAccountService accountService, IDepartmentService departmentService, IChatService chatService,
            ILogRepository logRepository)
        {
            _accountService = accountService;
            _departmentService = departmentService;
            _chatService = chatService;
            _logRepository = logRepository;
        }
        public async Task<List<ChatLogsInfoViewModel>> CreateChatLogsInfoViewModel(string userId)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            var userRoles = await _accountService.GetUserRolesAsync(user);
            var departments = new List<Department>();

            if (userRoles.Contains("Admin"))
            {
                departments = await _departmentService.GetAllAsync();
            }

            else if (userRoles.Contains("Department Head"))
            {
                departments = await _departmentService.GetDepartmentsUserIsAHeadOfAsync(userId);
            }

            var topics = new List<Topic>();
            var chats = new List<Chat>();
            var model = new List<ChatLogsInfoViewModel>();

            foreach (var department in departments)
            {
                topics = await _departmentService.GetTopicsInDepartment(department.Id);
                foreach(var topic in topics)
                {
                    chats = await _chatService.GetAllChatsByTopicName(topic.Name);

                    foreach(var chat in chats)
                    {
                        model.Add(new ChatLogsInfoViewModel
                        {
                            TopicName = topic.Name,
                            ChatId = chat.Id,
                            StartTime = chat.StartTime,
                            EndTime = chat.EndTime,
                            IsServiced = chat.IsServiced,
                        });
                    }
                }
            }

            // Remove duplicates
            model = model.GroupBy(x => x.ChatId)
                .Select(x => x.First())
                .ToList();


            model = model.OrderByDescending(x => x.EndTime == null) // Put the chats that are not closed at the top
                .ThenByDescending(x => x.EndTime ?? DateTime.MaxValue)
                .ThenBy(x => x.StartTime)
                .ToList();

            return model;

        }

        public async Task<List<LogDetailsViewModel>> GetChatLogsByChatId(int chatId)
        {
            var logList = await _logRepository.GetChatLogsByChatId(chatId);
            var model = new List<LogDetailsViewModel>();

            foreach (var log in logList)
            {
                var user = await _accountService.GetUserByIdAsync(log.UserId);
                model.Add(new LogDetailsViewModel
                {
                    EventTime = log.EventTime,
                    EventType = log.EventType,
                    Username = user.UserName,
                    Content = log.Content,
                    TopicName = log.Topic
                });
            }

            return model;
        }

        public async Task RemoveUserLogs(string userId, int chatId)
        {
            await _logRepository.RemoveUserLogs(userId, chatId);
        }
    }
}
