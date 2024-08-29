﻿using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using System.Security.Claims;

namespace HelpDeskApp.Services
{
    public class HelpDeskService : IHelpDeskService
    {
        private readonly IChatService _chatService;
        private readonly IAccountService _accountService;

        public HelpDeskService(IChatService chatService, IAccountService accountService)
        {
            _accountService = accountService;
            _chatService = chatService;
        }
        public async Task<List<ChatDisplayInHelpDeskViewModel>> CreateChatDisplayViewModel(List<Chat> availableChats)
        {
            var model = new List<ChatDisplayInHelpDeskViewModel>();

            foreach (var chat in availableChats)
            {
                var usernames = new List<string>();
                var userIds = await _chatService.GetUsersInChat(chat.Id);

                foreach (var id in userIds)
                {
                    var username = await _accountService.GetUsernameById(id);
                    usernames.Add(username + " ");
                }

                model.Add(new ChatDisplayInHelpDeskViewModel
                {
                    chatId = chat.Id,
                    topicName = chat.Topic,
                    usernamesInChat = usernames
                });
            }

            return model;
        }

        public async Task<List<Chat>> GetAvailableChats(string userId)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            var userRoles = await _accountService.GetUserRolesAsync(user);
            var availableChats = new List<Chat>();

            if (userRoles.Contains("Admin"))
            {
                availableChats = await _chatService.GetAllOpenChats(userId);
            }
            else
            {
                availableChats = await _chatService.GetAvailableConsultantChats(userId);
            }

            return availableChats;
        }
    }
}
