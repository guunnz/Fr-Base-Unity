using AuthFlow.AppleLogin.Core.Services;
using AuthFlow.EndAuth.Repo;
using JetBrains.Annotations;

namespace AuthFlow.AppleLogin.Infrastructure
{
    [UsedImplicitly]
    public class AppleLoginRepository : IAppleLoginRepository
    {
        readonly ILocalUserInfo userInfo;

        public AppleLoginRepository(ILocalUserInfo userInfo)
        {
            this.userInfo = userInfo;
        }

        public string AppleID
        {
            get => userInfo["apple-user-id"];
            set => userInfo["apple-user-id"] = value;
        }
    }
}