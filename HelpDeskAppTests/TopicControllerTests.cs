using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDeskAppTests
{
    public class TopicControllerTests
    {
        private readonly TopicController _topicController;

        public TopicControllerTests()
        {
            var topicService = new TestTopicService();
            var departmentService = new TestDepartmentService();
            _topicController = new TopicController(topicService, departmentService);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithModel()
        {
            // Act
            var result = await _topicController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<List<TopicViewModel>>();
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithDepartmentsInViewBag()
        {
            // Act
            var result = await _topicController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.ViewData.ContainsKey("Departments").Should().BeTrue();
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirectToActionResult_OnSuccessfulCreate()
        {
            // Arrange
            var topic = new Topic
            {
                Name = "Test Topic",
                Id = 1
            };

            // Act
            var result = await _topicController.Create(topic);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithDepartmentsInViewBag()
        {
            // Arrange
            var topic = new Topic
            {
                Name = "Test Topic",
                Id = 1
            };

            // Act
            var result = await _topicController.Edit(topic.Id);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.ViewData.ContainsKey("Departments").Should().BeTrue();
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirectToActionResult_OnSuccessfulEdit()
        {
            // Arrange
            var topic = new Topic
            {
                Name = "Test Topic",
                Id = 1
            };

            // Act
            var result = await _topicController.Edit(topic.Id, topic);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithTopicViewModel()
        {
            // Arrange
            var topic = new Topic
            {
                Name = "Test Topic",
                Id = 1
            };

            // Act
            var result = await _topicController.Delete(topic.Id);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<TopicViewModel>();
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_OnSuccessfulDelete()
        {
            // Arrange
            var topic = new Topic
            {
                Name = "Test Topic",
                Id = 1
            };

            // Act
            var result = await _topicController.DeleteConfirmed(topic.Id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }
    }
}
