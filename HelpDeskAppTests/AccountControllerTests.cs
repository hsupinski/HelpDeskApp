using FluentAssertions;
using HelpDeskApp.Controllers;
using HelpDeskApp.Models.ViewModels;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System.Security.Claims;

namespace HelpDeskAppTests
{
    public class AccountControllerTests
    {
        private readonly AccountController _accountController;
        private readonly RegisterViewModel exampleRegisterViewModel;
        private readonly LoginViewModel exampleLoginViewModel;
        private readonly ForgotPasswordViewModel exampleForgotPasswordViewModel;
        private readonly ResetPasswordViewModel exampleResetPasswordViewModel;
        private readonly TwoFactorViewModel exampleTwoFactorViewModel;
        public AccountControllerTests()
        {
            var accountService = new TestAccountService();
            var emailService = new TestEmailService();
            var userManager = new TestUserManager();
            var signInManager = new TestSignInManager();

            _accountController = new AccountController(accountService, emailService, userManager, signInManager);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://mocked-url");

            _accountController.Url = mockUrlHelper.Object;

            exampleRegisterViewModel = new RegisterViewModel
            {
                Username = "test",
                Email = "test@example.com",
                Password = "Test123!"
            };

            exampleLoginViewModel = new LoginViewModel
            {
                Username = "test",
                Password = "Test123!"
            };

            exampleForgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Email = "test@example.com"
            };

            exampleResetPasswordViewModel = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Password = "Test123!",
                Token = "validToken"
            };

            exampleTwoFactorViewModel = new TwoFactorViewModel
            {
                VerificationCode = "123456",
            };

            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();

            request.Setup(x => x.Scheme).Returns("http");
            httpContext.Setup(x => x.Request).Returns(request.Object);

            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };
        }
        [Fact]
        public void Register_ReturnsViewResult()
        {
            // Act
            var result = _accountController.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Register_ReturnsRedirectToActionResult_OnSuccessfulRegister()
        {
            // Arrange
            var model = exampleRegisterViewModel;

            // Act
            var result = await _accountController.Register(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("RegisterConfirmation");
        }



        [Fact]
        public async Task Login_ReturnsViewResult()
        {
            // Act
            var result = _accountController.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_ReturnsViewResult_OnSuccessfulLogin()
        {
            // Arrange
            var model = exampleLoginViewModel;

            // Act
            var result = await _accountController.Login(model);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_ReturnsViewResult_OnFailedLogin()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "invalid",
                Password = "invalid"
            };

            // Act
            var result = await _accountController.Login(model);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().Be(model);

            _accountController.ModelState.Should().BeEmpty();
        }

        [Fact]
        public async Task Logout_ReturnsRedirectToActionResult()
        {
            // Act
            var result = await _accountController.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");

            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }

        [Fact]
        public void ForgotPassword_ReturnsViewResult()
        {
            // Act
            var result = _accountController.ForgotPassword();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ForgotPassword_ReturnsRedirectToActionResult_OnValidModel()
        {
            // Arrange
            var model = exampleForgotPasswordViewModel;

            // Act
            var result = await _accountController.ForgotPassword(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("ForgotPasswordConfirmation");
        }



        [Fact]
        public void ForgotPasswordConfirmation_ReturnsViewResult()
        {
            // Act
            var result = _accountController.ForgotPasswordConfirmation();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ResetPassword_ReturnsViewResult_OnInvalidToken()
        {
            // Act
            var result = _accountController.ResetPassword("invalid");

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_ReturnsRedirectToActionResult_OnValidToken()
        {
            // Arrange
            var model = exampleResetPasswordViewModel;

            // Act
            var result = await _accountController.ResetPassword(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_ReturnsRedirectToActionResult_OnSuccessfulReset()
        {
            // Arrange
            var model = exampleResetPasswordViewModel;

            // Act
            var result = await _accountController.ResetPassword(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("ResetPasswordConfirmation");
        }

        [Fact]
        public async Task ResetPassword_ReturnsRedirectToActionResult_OnFailedReset()
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "invalid",
                Password = "invalid",
                Token = "invalid"
            };

            // Act
            var result = await _accountController.ResetPassword(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void ResetPasswordConfirmation_ReturnsViewResult()
        {
            // Act
            var result = _accountController.ResetPasswordConfirmation();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void VerifyTwoFactor_ReturnsViewResult()
        {
            // Act
            var result = _accountController.VerifyTwoFactor();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EnableTwoFactor_ReturnsNotFoundResult_WhenUserIsNull()
        {
            // Arrange
            var userManagerMock = new Mock<TestUserManager>();
            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);
            var accountController = new AccountController(new TestAccountService(), new TestEmailService(), userManagerMock.Object, new TestSignInManager());

            // Act
            var result = await accountController.EnableTwoFactor();

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}