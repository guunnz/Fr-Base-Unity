using System;
using JetBrains.Annotations;
using User.Core.Domain;
using User.Infrastructure;

namespace User.Core.Actions
{
    [UsedImplicitly]
    public class GetUserInfo
    {
        readonly UserInfoRepository userInfoRepository; // todo: refactor into interface

        public GetUserInfo(UserInfoRepository userInfoRepository)
        {
            this.userInfoRepository = userInfoRepository;
        }

        public IObservable<UserInfo> Execute()
        {
            return userInfoRepository.GetInfo();
        }
    }
}