// TODO(Jesse): DELETE THIS
//
using AuthFlow.EndAuth.Repo;
using JetBrains.Annotations;

namespace AuthFlow.Actions
{
    [UsedImplicitly]
    public class MarkAsLoggedIn
    {
        readonly ILocalUserInfo localUserInfo;

        public MarkAsLoggedIn(ILocalUserInfo localUserInfo)
        {
            this.localUserInfo = localUserInfo;
        }

        public void Execute()
        {
            /* localUserInfo["logged-in"] = "ok"; */
        }
    }
}
