using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Encodings.Web;
using TwoFactorAuthNet;

namespace HelpDeskApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TwoFactorAuth _tfa;

        public AccountController(IAccountService accountService, IEmailService emailService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _accountService = accountService;
            _emailService = emailService;
            _userManager = userManager;
            _tfa = new TwoFactorAuth("HelpDeskApp");
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            Log.Information("Register page entered");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Log.Information("Registering user");

            var result = await _accountService.RegisterUserAsync(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            await _emailService.SendEmail(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");

            return RedirectToAction("RegisterConfirmation");
        }

        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            Log.Information("Login page entered");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            Log.Information("Confirming email");
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                Log.Information($"Error confirming email for user with ID '{userId}':");
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            Log.Information($"Email confirmed for user with ID '{userId}'.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            Log.Information("Logging in user");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if(user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if(result.RequiresTwoFactor)
                    {
                        if (await _userManager.GetTwoFactorEnabledAsync(user) && await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction(nameof(VerifyTwoFactor));
                        }
                    }
                    else if(result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }
                }
            }

            Console.WriteLine("Invalid ModelState");

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            Log.Information("Forgot password page entered");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Log.Information("Forgot password");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailService.SendEmail(model.Email, "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token = null)
        {
            Log.Information("Reset password page entered");
            if (token == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var model = new ResetPasswordViewModel { Token = token };
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Log.Information("Resetting password");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Log.Information("Logging out user");
            await _accountService.LogoutUserAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableTwoFactor()
        {
            Log.Information("Enabling two-factor authentication");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var secret = _tfa.CreateSecret(160);
            await _userManager.SetAuthenticationTokenAsync(user, "HelpDeskApp", "Secret", secret);

            var qrCodeData = _tfa.GetQrCodeImageAsDataUri(user.Email, secret);
            //ViewBag.QrCodeData = qrCodeData;

            var model = new TwoFactorViewModel
            {
                QrCodeData = qrCodeData
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableTwoFactor(TwoFactorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var secret = await _userManager.GetAuthenticationTokenAsync(user, "HelpDeskApp", "Secret");
            var isValid = _tfa.VerifyCode(secret, model.VerificationCode);

            if (isValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                TempData["SuccessMessage"] = "Two-factor authentication has been enabled.";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Verification code is invalid";
            ModelState.AddModelError(string.Empty, "Verification code is invalid");
            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyTwoFactor()
        {
            Log.Information("Verifying two-factor authentication");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFactor(TwoFactorViewModel model)
        {
            var verificationCode = model.VerificationCode;
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var secret = await _userManager.GetAuthenticationTokenAsync(user, "HelpDeskApp", "Secret");
            var isValid = _tfa.VerifyCode(secret, verificationCode);

            if (isValid)
            {
                await _signInManager.SignInAsync(user, false);
                TempData["SuccessMessage"] = "You have successfully logged in.";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Verification code is invalid";
            ModelState.AddModelError(string.Empty, "Verification code is invalid");
            return View();
        }
    }
}