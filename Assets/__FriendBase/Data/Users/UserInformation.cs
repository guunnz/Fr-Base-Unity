using Architecture.Injector.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Users
{
    [System.Serializable]
    public class UserInformation
    {
        public delegate void GemsUpdate(int newAmount, int amountAdded);
        public event GemsUpdate OnGemsUpdate;

        public delegate void GoldUpdate(int newAmount, int amountAdded);
        public event GoldUpdate OnGoldUpdate;

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int FriendCount { get; set; }
        public int FriendRequestsCount { get; set; }
        public string FirebaseId { get; set; }
        public int Gems { get; private set; }
        public int Gold { get; private set; }

        public string Country { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Photo_url { get; set; }
        private List<int> BlockedPlayers { get; set; } = new List<int>();
        public bool Do_avatar_customization { get; set; }

        private AvatarCustomizationData avatarCustomizationData;
        public UserAccountStatus UserStatus { get; set; } = new UserAccountStatus();

        public List<string> AuthProviders { get; set; } = new List<string>();

        public UserInformation()
        {
            Gems = 0;
            Gold = 0;
        }

        public void SetBlockedPlayers(List<int> userIdList)
        {
            BlockedPlayers = userIdList;
        }
        public void AddBlockedPlayer(int userId)
        {
            BlockedPlayers.Add(userId);
        }

        public List<int> GetBlockedPlayers()
        {
            return BlockedPlayers;
        }

        public bool HasMailProvider()
        {
            foreach (string provider in AuthProviders)
            {
                Debug.Log("---- provider:" + provider);
            }
            if (AuthProviders.Contains(AuthProvidersFirebase.PASSWORD))
            {
                return true;
            }
            return false;
        }

        public void SetGems(int newGemsAmount)
        {
            Gems = newGemsAmount;
            if (OnGemsUpdate != null)
            {
                OnGemsUpdate(Gems, 0);
            }
        }

        public void AddGems(int amount)
        {
            Gems += amount;
            if (OnGemsUpdate != null)
            {
                OnGemsUpdate(Gems, amount);
            }
        }

        public void SubstractGems(int amount)
        {
            Gems -= amount;
            if (OnGemsUpdate != null)
            {
                OnGemsUpdate(Gems, -amount);
            }
        }

        public void SetGold(int newGoldAmount)
        {
            Gold = newGoldAmount;
            if (OnGoldUpdate != null)
            {
                OnGoldUpdate(Gold, 0);
            }
        }

        public void AddGold(int amount)
        {
            Gold += amount;
            Injection.Get<IAvatarEndpoints>().SetPlayerCoins(amount);
            if (OnGoldUpdate != null)
            {
                OnGoldUpdate(Gold, amount);
            }
        }

        public void SubstractGold(int amount)
        {
            Gold -= amount;

            Injection.Get<IAvatarEndpoints>().SetPlayerCoins(-amount);
            if (OnGoldUpdate != null)
            {
                OnGoldUpdate(Gold, -amount);
            }
        }

        public void SetEmail(string email)
        {
            this.Email = email;
        }

        public AvatarCustomizationData GetAvatarCustomizationData()
        {
            if (avatarCustomizationData == null)
            {
                avatarCustomizationData = new AvatarCustomizationData();
            }

            return avatarCustomizationData;
        }

        public void Initialize(UserInformation userInformation)
        {
            UserId = userInformation.UserId;
            UserName = userInformation.UserName;
            FirstName = userInformation.FirstName;
            LastName = userInformation.LastName;
            FriendCount = userInformation.FriendCount;
            FriendRequestsCount = userInformation.FriendRequestsCount;
            FirebaseId = userInformation.FirebaseId;
            Gems = userInformation.Gems;
            Gold = userInformation.Gold;
            Country = userInformation.Country;
            Email = userInformation.Email;
            Gender = userInformation.Gender;
            Photo_url = userInformation.Photo_url;
            Do_avatar_customization = userInformation.Do_avatar_customization;
        }
    }
}