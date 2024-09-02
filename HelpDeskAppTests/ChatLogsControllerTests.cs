using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskAppTests
{
    public class ChatLogsControllerTests
    {
        private readonly ChatLogsController _chatLogsController;

        public ChatLogsControllerTests()
        {
            var logService = new TestLogService();
            _chatLogsController = new ChatLogsController(logService);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "test")
            }, "mock"));

            _chatLogsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            // Act
            var result = await _chatLogsController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.Should().NotBeNull();
            var viewResult = result.As<ViewResult>();
            viewResult.Model.Should().BeOfType<List<ChatLogsInfoViewModel>>();

        }

        [Fact]
        public async Task Details_ReturnsViewWithModel()
        {
            // Act
            var result = await _chatLogsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
            result.Should().NotBeNull();
            var viewResult = result.As<ViewResult>();
            viewResult.Model.Should().BeOfType<List<LogDetailsViewModel>>();
        }
    }
}
