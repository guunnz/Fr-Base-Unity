using AppleAuth.Interfaces;

namespace AuthFlow.AppleLogin.Infrastructure
{
    public class AppleData
    {
        public string UserId { get;}
        public string Email { get;}
        public IPersonName FullName { get;}
        public string IdentityToken { get;}
        public string AuthorizationCode { get;}

        public AppleData(string userId, string email, IPersonName fullName, string identityToken, string authorizationCode)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            IdentityToken = identityToken;
            AuthorizationCode = authorizationCode;
        }
    }
}