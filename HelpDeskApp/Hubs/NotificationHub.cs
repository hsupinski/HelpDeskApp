using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.SignalR;


namespace HelpDeskApp.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ITopicService _topicService;
        private readonly IDepartmentService _departmentService;
        public NotificationHub(IDepartmentService departmentService, ITopicService topicService)
        {
            _departmentService = departmentService;
            _topicService = topicService;
        }
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Consultant") || Context.User.IsInRole("Department Head") || Context.User.IsInRole("Admin"))
            {
                var userId = Context.UserIdentifier;

                var departmentList = await _departmentService.GetUserDepartments(userId);
                var topicList = new List<Topic>();

                foreach (var department in departmentList)
                {
                    var topics = await _topicService.GetTopicsByDepartmentId(department.Id);
                    topicList.AddRange(topics);
                }

                foreach (var topic in topicList)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Topic-{topic.Id}");
                    //Console.WriteLine("User connected to topic: " + topic.Name);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.IsInRole("Consultant") || Context.User.IsInRole("Department Head") || Context.User.IsInRole("Admin"))
            {
                var userId = Context.UserIdentifier;
                var departmentList = await _departmentService.GetUserDepartments(userId);
                var topicList = new List<Topic>();

                foreach (var department in departmentList)
                {
                    var topics = await _topicService.GetTopicsByDepartmentId(department.Id);
                    topicList.AddRange(topics);
                }

                foreach (var topic in topicList)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Topic-{topic.Id}");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
