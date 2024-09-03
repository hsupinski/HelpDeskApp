using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAppTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;

        public HomeControllerTests()
        {
            var chatService = new TestChatService();
            var topicService = new TestTopicService();
            var accountService = new TestAccountService();
            _homeController = new HomeController(chatService, topicService, accountService);
        }

        [Fact]
        public async Task Index_ReturnsRedirectToActionResult_WhenActiveChatExistsAndUserRoleIsUser()
        {
            // Arrange
            var userId = "10"; // Ids that start with 1 are considered users with active chats
            var userRole = "User";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.Index();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Chat");
        }

        [Fact]
        public async Task Index_ReturnsRedirectsToActionResult_WhenActiveChatDoesNotExistAndUserRoleIsUser()
        {
            // Arrange
            var userId = "2";
            var userRole = "User";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.Index();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("ChooseTopic");
        }

        [Fact]
        public async Task Index_ReturnsRedirectToActionResult_WhenActiveChatExistsAndUserRoleIsNotUser()
        {
            // Arrange
            var userId = "11";
            var userRole = "Consultant";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.Index();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Chat");
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenActiveChatDoesNotExistAndUserRoleIsNotUser()
        {
            // Arrange
            var userId = "2";
            var userRole = "Consultant";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ChooseTopic_ReturnsRedirectToActionResult_WhenUserRoleIsNotUser()
        {
            // Arrange
            var userId = "3";
            var userRole = "Consultant";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.ChooseTopic();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ChooseTopic_ReturnsViewResultWithTopics_WhenUserRoleIsUser()
        {
            // Arrange
            var userId = "12";
            var userRole = "User";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.ChooseTopic();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<List<Topic>>();
        }

        [Fact]
        public async Task CreateChat_ReturnsRedirectToActionResult_WhenUserRoleIsUser()
        {
            // Arrange
            var userId = "13";
            var userRole = "User";
            var topicId = 1;
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.CreateChat(topicId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Chat");
        }

        [Fact]
        public async Task CreateChat_ReturnsRedirectToActionResult_WhenUserRoleIsNotUser()
        {
            // Arrange
            var userId = "4";
            var userRole = "Consultant";
            var topicId = 1;
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.CreateChat(topicId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task JoinChat_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userId = "5";
            var userRole = "Consultant";
            var chatId = 1;
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.JoinChat(chatId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Chat");
        }

        [Fact]
        public async Task Chat_ReturnsViewResult_WithModel()
        {
            // Arrange
            var userId = "6";
            var userRole = "User";
            var chatId = 1;
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.Chat(chatId);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<ChatViewModel>();
        }

        [Fact]
        public async Task LeaveChat_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userId = "7";
            var userRole = "User";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = TestHttpContextFactory.CreateHttpContext(userId, userRole)
            };

            // Act
            var result = await _homeController.LeaveChat();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _homeController.Privacy();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}