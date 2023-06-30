// TODO(Jesse): DELETE THIS
//
using System;
using JetBrains.Annotations;
using UniRx;
using User.Core.Domain;
using User.Infrastructure;

namespace User.Core
{
    [UsedImplicitly]
    public class SaveUserInfo
    {
        readonly UserInfoRepository userInfoRepository; // todo: refactor into interface

        public SaveUserInfo(UserInfoRepository userInfoRepository)
        {
            this.userInfoRepository = userInfoRepository;
        }


        public IObservable<UserInfo> Execute(UserInfo info)
        {
            return userInfoRepository.SaveInfo(info).Select(_ => info);

        }
    }
}
