using System;
using System.Collections.Generic;
using Architecture.MVP;
using UI.DataField;
using UniRx;

namespace AuthFlow.AboutYou.Presentation
{
    public interface IAboutYouView : IPresentable
    {
        void ShowNames();
        void ShowBirthGender();

        void SetLoadingPanelActive(bool state);

        IObservable<Unit> OnSubmitSection { get; }
        int GenderIndex { get; set; }

        DateTime BirthDate { get; }

        IDataField<string> UserName { get; }
        IDataField<string> FirstName { get; }
        IDataField<string> LastName { get; }


        void SetGenders(List<string> genders);
        void ShowUserName();
    }
}
