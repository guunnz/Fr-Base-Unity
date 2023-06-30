using System;

namespace User.Core.Domain
{
    public class UserInfo
    {
        public readonly string displayName;
        public readonly string email;
        //public readonly Uri photoUrl;
        public readonly string providerId;
        public readonly string userId;
        public readonly string token;

        public UserInfo(/*Uri photoUrl,*/
            string displayName, string email, Uri authPhotoUrl, string providerId, string userId, string token)
        {
            this.displayName = displayName;
            this.email = email;
            //this.photoUrl = photoUrl;
            this.providerId = providerId;
            this.userId = userId;
            this.token = token;
        }

        public override string ToString()
        {
            return "userInfo: {\n" +
                   " displayName : " + displayName + " \n " +
                   " email : " + email + " \n " +
                   //" photoUrl : " + photoUrl + " \n " +
                   " providerId : " + providerId + " \n " +
                   " userId : " + userId + " \n " +
                   " token : " + token + " \n " +
                   "}";
        }
    }
}