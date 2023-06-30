
using System.Threading.Tasks;
using Data.Users;
using Firebase.Auth;
using Newtonsoft.Json.Linq;
using static NauthFlowEndpoints;

public interface IAuthFlowEndpoint 
{
    Task<string> IsUserLoggedIn();
    Task<bool> GetInitialAvatarEndpoints();
    Task<bool> SendEmailResetPassword(string email);
    Task<UserInformation> CreatePhoenixUser(string mail, string userId, string loginToken, ProviderType providerType, string username, string birthday);
    Task<UserInformation> CreatePhoenixGuestUser(string userId, string loginToken);
    Task<UserInformation> LinkPhoenixGuestUser(string mail, string userId, string loginToken, ProviderType providerType, string username, string birthday);
    Task<UserNameResult> IsAvailableUserName(string userName);
}
