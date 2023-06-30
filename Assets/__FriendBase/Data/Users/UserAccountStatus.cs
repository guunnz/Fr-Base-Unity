using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Users
{
    public class UserAccountStatus
    {
        public enum USER_ACCOUNT_STATUS { ACTIVE, BANNED, SUSPENDED };

        public USER_ACCOUNT_STATUS StatusAccount { get; private set; }
        public int UserId { get; private set; }

        public float TimeSuspensionStart { get; private set; }
        public float TimeSuspensionEnd { get; private set; }
        public float TimeSuspensionLeft { get; private set; }

        private float timeStartSuspension;

        public UserAccountStatus()
        {
            SetActive();
        }

        public void ActivateBan()
        {
            StatusAccount = USER_ACCOUNT_STATUS.BANNED;
        }

        public void ActivateSuspension(string timeSuspensionStart, string timeSuspensionEnd, string timeSuspensionLeft)
        {
            StatusAccount = USER_ACCOUNT_STATUS.SUSPENDED;
            if (!string.IsNullOrEmpty(timeSuspensionLeft))
            {
                TimeSuspensionLeft = int.Parse(timeSuspensionLeft) * 1000;
                timeStartSuspension = Time.time * 1000;
                GetTimeSuspensionLeft();
            }
        }

        public TimeSpan GetTimeSuspensionLeft()
        {
            float timeNow = Time.time * 1000;
            float deltaTime = timeNow - timeStartSuspension;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSuspensionLeft - deltaTime);
            return timeSpan;
        }

        public void SetActive()
        {
            StatusAccount = USER_ACCOUNT_STATUS.ACTIVE;
        }

        public bool IsBanned()
        {
            return StatusAccount == USER_ACCOUNT_STATUS.BANNED;
        }

        public bool IsSuspended()
        {
            return StatusAccount == USER_ACCOUNT_STATUS.SUSPENDED;
        }

        public bool IsActive()
        {
            return StatusAccount == USER_ACCOUNT_STATUS.ACTIVE;
        }

        public void SetStatus(string status, int userId, string timeSuspensionStart, string timeSuspensionEnd, string timeSuspensionLeft)
        {
            UserId = userId;
            switch (status)
            {
                case "active":
                    SetActive();
                    break;
                case "banned":
                    ActivateBan();
                    break;
                case "suspended":
                    ActivateSuspension(timeSuspensionStart, timeSuspensionEnd, timeSuspensionLeft);
                    break;
            }
        }
    }
}