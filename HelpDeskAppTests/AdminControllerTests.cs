using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAppTests
{
    public class AdminControllerTests
    {
        private readonly AdminController _adminController;

        public AdminControllerTests()
        {
            var accountService = new TestAccountService();
            _adminController = new AdminController(accountService);
        }

        [Fact]
        public async Task ManageUserRoles_ReturnsViewResult()
        {
            // Act
            var result = await _adminController.ManageUserRoles();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ManageUserRoles_Post_RedirectsToManageUserRoles()
        {
            // Arrange
            var model = new List<UserRoleViewModel>
            {
                new UserRoleViewModel
                {
                    UserId = "1",
                    UserName = "test",
                    Roles = new List<string> { "User" }
                }
            };

            // Act
            var result = await _adminController.ManageUserRoles(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("ManageUserRoles");
        }
    }
}
