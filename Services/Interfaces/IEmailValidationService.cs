using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEmailValidationService
    {
        Task<bool> ValidateEmployeeEmail(string Email);
        Task<bool> ValidatePassword(string Password);
        Task SendValidationEmail(string Email, string userId);
        Task SendRecoveryEmail(string email, string recoveryToken);
    }
}
