// TODO(Jesse): DELETE THIS
//
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using JetBrains.Annotations;

namespace AuthFlow.Actions
{
    [UsedImplicitly]
    public class CheckCookieCredentials
    {
        private ILocalUserInfo localUserInfo;

        public CheckCookieCredentials(ILocalUserInfo localUserInfo)
        {
            this.localUserInfo = localUserInfo;
        }

        public bool Execute()
        {
            /* if (localUserInfo["logged-in"] != "ok") */
            /* { */
            /*     return false; */
            /* } */

            var user = FirebaseAuth.DefaultInstance.CurrentUser;
            return user != null && !string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.UserId);
        }
    }
}
