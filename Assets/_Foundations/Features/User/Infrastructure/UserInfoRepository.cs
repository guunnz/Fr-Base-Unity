// TODO(Jesse): DELETE THIS
//
using System;
using AuthFlow.EndAuth.Repo;
using JetBrains.Annotations;
using LocalStorage.Core;
using UniRx;
using UnityEngine;
using User.Core.Domain;

namespace User.Infrastructure
{
    [UsedImplicitly]
    public class UserInfoRepository
    {
        const string UserInfoKey = "cookie-user-info";
        readonly ILocalUserInfo cookie;

        public UserInfoRepository(ILocalUserInfo cookie)
        {
            this.cookie = cookie;
        }

        public IObservable<UserInfo> GetInfo()
        {
            return Observable.Create<UserInfo>(obs =>
            {
                obs.OnNext(RetrieveUserInfo());
                obs.OnCompleted();
                return Disposable.Empty;
            }).SubscribeOnMainThread().ObserveOnMainThread();
        }

        public IObservable<Unit> SaveInfo(UserInfo userInfo)
        {
            return Observable.Create<Unit>(obs =>
            {
                SaveInfoAndPrint(userInfo);
                obs.OnNext(Unit.Default);
                obs.OnCompleted();
                return Disposable.Empty;
            }).SubscribeOnMainThread().ObserveOnMainThread();
        }

        void SaveInfoAndPrint(UserInfo userInfo)
        {
            Debug.Log("try to save info " + userInfo);
            SaveUserInfo(userInfo);
            Debug.Log("info " + userInfo + " saved successfully");
        }

        void SaveUserInfo(UserInfo userInfo)
        {
            var dto = ToDTO(userInfo);

            var json = JsonUtility.ToJson(dto, true);

            cookie[UserInfoKey] = json;
        }

        UserInfo RetrieveUserInfo()
        {
            var userInfoJson = cookie[UserInfoKey];
            return string.IsNullOrEmpty(userInfoJson) ? null : FromDTO(JsonUtility.FromJson<UserInfoDTO>(userInfoJson));
        }

        static UserInfoDTO ToDTO(UserInfo info)
        {
            var dto = new UserInfoDTO();
            Debug.Log("dto.displayName = info.displayName;");
            dto.displayName = info.displayName;
            Debug.Log("dto.email = info.email;");
            dto.email = info.email;
            Debug.Log("dto.photoUrl = info.photoUrl.ToString();");
            dto.photoUrl = ""; //info.photoUrl.ToString();// todo: check wtf is happening
            Debug.Log("dto.providerId = info.providerId;");
            dto.providerId = info.providerId;
            Debug.Log("dto.userId = info.userId;");
            dto.userId = info.userId;
            Debug.Log("dto.token = info.token;");
            dto.token = info.token;
            Debug.Log("dto created!");
            return dto;
        }

        static UserInfo FromDTO(UserInfoDTO dto)
        {
            var created = Uri.TryCreate(dto.photoUrl, UriKind.RelativeOrAbsolute, out var photoUrl);
            if (!created)
            {
                Uri.TryCreate("www.google.com", UriKind.RelativeOrAbsolute, out photoUrl);
                Debug.LogWarning(
                    $"URI not created for photo url : {photoUrl} using google.com as a default value to continue");
            }

            return new UserInfo(
                dto.displayName,
                dto.email,
                photoUrl,
                dto.providerId,
                dto.userId,
                dto.token
            );
        }
    }

    [Serializable]
    public struct UserInfoDTO
    {
        public string displayName;
        public string email;
        public string photoUrl;
        public string providerId;
        public string userId;
        public string token;
    }
}
