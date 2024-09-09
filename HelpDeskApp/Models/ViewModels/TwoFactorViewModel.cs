namespace HelpDeskApp.Models.ViewModels
{
    public class TwoFactorViewModel
    {
        public string? VerificationCode { get; set; }
        public string? QrCodeData { get; set; }
    }
}
