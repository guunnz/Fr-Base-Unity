using Architecture.Injector.Core;
using JetBrains.Annotations;
using LocalizationSystem;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class PasswordValidator : IPasswordValidator
    {
        public (bool, string) Validate(string pass)
        {
            (bool, string) Result = (true, null);

            if (string.IsNullOrEmpty(pass))
            {
                Result = (false, Injection.Get<ILanguage>().GetTextByKey(LangKeys.AUTH_ENTER_YOUR_PASSWORD));
            }
            else if(pass.Length < 6)
            {
                Result = (false, Injection.Get<ILanguage>().GetTextByKey(LangKeys.AUTH_MUST_BE_AT_LEAST_6));
            }

            return Result;
        }
    }
}
