using System.ComponentModel.DataAnnotations;

namespace HelpDeskApp.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
