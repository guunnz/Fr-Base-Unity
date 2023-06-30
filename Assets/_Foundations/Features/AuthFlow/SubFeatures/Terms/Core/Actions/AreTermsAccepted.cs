// TODO(Jesse): Delete this
//
using AuthFlow.EndAuth.Repo;
using JetBrains.Annotations;

namespace AuthFlow.Terms.Core.Actions
{
    [UsedImplicitly]
    public class AreTermsAccepted
    {
        readonly ILocalUserInfo localUserInfo;

        public AreTermsAccepted(ILocalUserInfo localUserInfo)
        {
            this.localUserInfo = localUserInfo;
        }

        public bool Execute()
        {
            return localUserInfo["terms"] == "True";
        }
    }

    [UsedImplicitly]
    public class SetTermsAccepted
    {
        readonly ILocalUserInfo localUserInfo;

        public SetTermsAccepted(ILocalUserInfo localUserInfo)
        {
            this.localUserInfo = localUserInfo;
        }

        public void Execute()
        {
            localUserInfo["terms"] = "True";
        }
    }
}
