using System;
using LocalStorage.Core;
using UnityEngine;

namespace AuthFlow.Infrastructure
{
    public class LocalUserAuthRepo : IUserAuthRepo
    {
        private ILocalStorage localStorage;
        private const string authInfoKey = "authInfoKey";

        public LocalUserAuthRepo(ILocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        public void SaveUserAuthInfo(string userId, string displayName)
        {
            var userAuthDTO = new UserAuthDTO
            {
                userId = userId,
                displayName = displayName
            };
            var jsonUserAuth = JsonUtility.ToJson(userAuthDTO);
            localStorage.SetString(authInfoKey, jsonUserAuth);
        }

        public (string userId, string displayName)? GetUserAuthInfo()
        {
            var jsonUserAuth = localStorage.GetString(authInfoKey);
            if (string.IsNullOrEmpty(jsonUserAuth))
            {
                return null;
            }
            var userAuthDto = JsonUtility.FromJson<UserAuthDTO>(jsonUserAuth);
            return (userAuthDto.userId, userAuthDto.displayName);
        }
    }

    [Serializable]
    public struct UserAuthDTO
    {
        public string userId;
        public string displayName;
    }
}