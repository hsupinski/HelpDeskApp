using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAppTests
{
    public class DepartmentControllerTests
    {
        private readonly DepartmentController _departmentController;

        public DepartmentControllerTests()
        {
            var departmentService = new TestDepartmentService();
            var accountService = new TestAccountService();
            var topicService = new TestTopicService();
            _departmentController = new DepartmentController(departmentService, accountService, topicService);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithModel()
        {
            // Act
            var result = await _departmentController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<List<DepartmentWithHeadViewModel>>();
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithUsersInViewBag()
        {
            // Act
            var result = await _departmentController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.ViewData.ContainsKey("Users").Should().BeTrue();
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirectToActionResult_OnSuccessfulCreate()
        {
            // Arrange
            var department = new Department
            {
                Name = "Test Department"
            };

            var departmentHeadId = "1";

            // Act
            var result = await _departmentController.Create(department, departmentHeadId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithModel()
        {
            // Arrange
            var departmentId = 1;

            // Act
            var result = await _departmentController.Edit(departmentId);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<Department>();
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirectToActionResult_OnSuccessfulEdit()
        {
            // Arrange
            var department = new Department
            {
                Id = 1,
                Name = "Test Department"
            };
            var departmentHeadId = "1";

            // Act
            var result = await _departmentController.Edit(department.Id, department, departmentHeadId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithModel()
        {
            // Arrange
            var departmentId = 1;

            // Act
            var result = await _departmentController.Delete(departmentId);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<Department>();
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_OnSuccessfulDelete()
        {
            // Arrange
            var departmentId = 1;

            // Act
            var result = await _departmentController.DeleteConfirmed(departmentId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }
    }
}
