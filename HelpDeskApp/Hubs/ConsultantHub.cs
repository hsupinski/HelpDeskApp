using HelpDeskApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace HelpDeskApp.Hubs
{
    public class ConsultantHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IAccountService _accountService;
        private readonly IDepartmentService _departmentService;
        private readonly ITopicService _topicService;

        public ConsultantHub(IChatService chatService, IAccountService accountService, 
            IDepartmentService departmentService, ITopicService topicService)
        {
            _chatService = chatService;
            _accountService = accountService;
            _departmentService = departmentService;
            _topicService = topicService;
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
