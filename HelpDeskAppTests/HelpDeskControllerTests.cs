using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace HelpDeskAppTests
{
    public class HelpDeskControllerTests
    {
        private readonly HelpDeskController _helpDeskController;

        public HelpDeskControllerTests()
        {
            var chatService = new TestChatService();
            var accountService = new TestAccountService();
            var helpDeskService = new TestHelpDeskService();
            var departmentService = new TestDepartmentService();

            _helpDeskController = new HelpDeskController(chatService, accountService, helpDeskService, departmentService);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "1"),
            ], "mock"));

            _helpDeskController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _helpDeskController.TempData = tempData;
        }

        [Fact]
        public async Task Panel_ReturnsViewResult_WithModel()
        {
            // Act
            var result = await _helpDeskController.Panel();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<List<JoinChatItemViewModel>>();
        }

        [Fact]
        public async Task JoinChat_ReturnsRedirectToActionResult_OnValidChat()
        {
            // Arrange
            var chatId = 1;

            // Act
            var result = await _helpDeskController.JoinChat(chatId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Chat");
        }

        [Fact]
        public async Task JoinChat_ReturnsRedirectToActionResult_OnInvalidChat()
        {
            // Arrange
            var chatId = -1;

            // Act
            var result = await _helpDeskController.JoinChat(chatId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Panel");

            _helpDeskController.TempData["ErrorMessage"].Should().Be("Chat is not available");
        }
    }
}
