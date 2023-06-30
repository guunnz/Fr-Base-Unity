using System;
using Functional.Maybe;

namespace AuthFlow.AboutYou.Core.Services
{
    public interface IAboutYouStateManager
    {
        void Clear();

        Maybe<string> FirstName { get; set; }
        Maybe<string> LastName { get; set; }
        
        Maybe<string> Gender { get; set; }
        Maybe<DateTime> BirthDate { get; set; } 
        Maybe<string> UserName { get; set; } 
        
        
    }
}