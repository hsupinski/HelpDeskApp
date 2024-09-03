using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;

namespace HelpDeskAppTests.TestServices
{
    internal class TestHelpDeskService : IHelpDeskService
    {
        public async Task<List<JoinChatItemViewModel>> CreateChatDisplayViewModel(List<Chat> availableChats)
        {
            var model = new List<JoinChatItemViewModel>();
            return model;
        }

        public async Task<List<Chat>> GetAvailableChats(string userId)
        {
            var model = new List<Chat>();
            return model;
        }
    }
}
