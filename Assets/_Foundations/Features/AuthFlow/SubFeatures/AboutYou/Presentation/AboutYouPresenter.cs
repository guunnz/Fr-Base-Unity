using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.ViewManager;
using AuthFlow.AboutYou.Core.Services;
using Firebase.Auth;
using Functional.Maybe;
using JetBrains.Annotations;
using UI.DataField;
using UniRx;
using UnityEngine;

namespace AuthFlow.AboutYou.Presentation
{
    [UsedImplicitly]
    public class AboutYouPresenter
    {
        readonly IAboutYouView view;
        readonly IAboutYouWebClient webClient;
        readonly IAboutYouStateManager state;
        readonly IViewManager viewManager;

        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sectionDisposables = new CompositeDisposable();

        public AboutYouPresenter(IAboutYouView view, IAboutYouWebClient webClient, IAboutYouStateManager state,
            IViewManager viewManager)
        {
            this.view = view;
            this.webClient = webClient;
            this.state = state;
            this.viewManager = viewManager;

            this.view.OnShowView.Subscribe(Present).AddTo(disposables);
            this.view.OnHideView.Subscribe(Hide).AddTo(disposables);
            this.view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);
        }

        void Present()
        {
            sectionDisposables.Clear();
            switch (GetPage())
            {
                case Page.Names:
                    PresentNames();
                    break;
                case Page.BirthGender:
                    PresentBirthGender();
                    break;
                case Page.SetUserName:
                    PresentUserName();
                    break;
                case Page.Completed:
                    EndWorkflow();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        async void PresentUserName()
        {
            // TODO(Jesse): This should sync on the EndAuthPresenter instead.
            // If we don't end up hitting this view, we'll never actually
            // sync this data to the server!!

            // TODO(Jesse): This should also surface errors to the user. Right
            // now if the submission doesn't work the page just appears dead.
            view.ShowUserName();


            await view.OnSubmitSection.Do(async _ =>
            {
                view.SetLoadingPanelActive(true);

                if (await ValidateUserName())
                {
                    bool UserDataSyncSuccess = true;
                    try
                    {
                        await SyncUserData();
                    }
                    catch (Exception e)
                    {
                        UserDataSyncSuccess = false;
                    }

                    if (UserDataSyncSuccess)
                    {
                        // Done .. should we just call EndWorkflow directly?
                        // Right now it gets called indirectly through Present()
                    }
                    else
                    {
                        // Display an error?
                    }
                }

                view.SetLoadingPanelActive(false);

                Present();
            });
        }

        async Task<bool> ValidateUserName()
        {
            bool usernameIsValid = false;
            bool usernameIsAvailable = false;
            int userNameCharLimit = 3;
            int userNameCharMaxLimit = 20;

            var userName = view.UserName.Value;
            if (string.IsNullOrEmpty(userName))
            {
                view.UserName.ShowError("This field is required");
            }
            else if (userName.Length < userNameCharLimit || userName.Length > userNameCharMaxLimit)
            {
                view.UserName.ShowError($"Username must contain at least {userNameCharLimit} characters");
                view.UserName.ShowError($"Must be between {userNameCharLimit} - {userNameCharMaxLimit} characters");
            }
            else
            {
                usernameIsValid = true;
            }

            if (usernameIsValid)
            {
                // TODO(Jesse): We need a try/catch block around this .. probably?
                usernameIsAvailable = await webClient.IsAvailableUserName(userName);

                if (usernameIsAvailable)
                {
                    state.UserName = userName;
                }
                else
                {
                    view.UserName.ShowError($"the name {userName} is already taken");
                }
            }

            bool Result = usernameIsAvailable && usernameIsValid;
            return Result;
        }

        void EndWorkflow()
        {
            viewManager.GetOut("next-view");
        }

        void PresentNames()
        {
            state.FirstName.Do(v => view.FirstName.Value = v);
            state.LastName.Do(v => view.LastName.Value = v);

            view.ShowNames();
            view.OnSubmitSection
                .Select(() => ValidateName(view.FirstName, "Invalid Name"))
                .WhereTrue()
                .Do(() => state.FirstName = view.FirstName.Value.ToMaybe())
                .Select(() => ValidateName(view.LastName, "Invalid Name"))
                .WhereTrue()
                .Do(() => state.LastName = view.LastName.Value.ToMaybe())
                .Subscribe(Present)
                .AddTo(sectionDisposables);
        }

        bool ValidateName(IDataField<string> nameDataField, string message)
        {
            if (string.IsNullOrEmpty(nameDataField.Value) || nameDataField.Value.Length < 3)
            {
                nameDataField.ShowError(message);
                return false;
            }

            return true;
        }

        void PresentBirthGender()
        {
            webClient
                .GetGendersOptions()
                .Do(genders => Debug.Log("get genders " + string.Join(",", genders)))
                .Do(genders => view.SetGenders(genders))
                .Do(view.ShowBirthGender)
                .Do(genders =>
                {
                    state.Gender
                        .Select(genders.IndexOf)
                        .Where(i => i >= 0)
                        .Do(i => view.GenderIndex = i);
                })
                .SelectMany(genders => view.OnSubmitSection
                    .Do(() => state.BirthDate = view.BirthDate.ToMaybe())
                    .Do(() => state.Gender = genders[view.GenderIndex].ToLower().ToMaybe())
                )
                .Do(Present)
                .Subscribe()
                .AddTo(sectionDisposables);
        }

        bool ValidateBirthAndGender(List<string> genders)
        {
            var date = view.BirthDate;
            var bottomLimit = DateTime.UtcNow - TimeSpan.FromDays(365 * 150);
            var topLimit = DateTime.UtcNow - TimeSpan.FromDays(365 * 18);
            if (date < bottomLimit)
            {
                //show error
            }

            return true;
        }

        IObservable<Unit> SyncUserData()
        {
            return webClient.UpdateUserData(state.FirstName, state.LastName, state.BirthDate, state.Gender, state.UserName);
        }


        void Hide()
        {
        }

        void CleanUp()
        {
        }

        Page GetPage()
        {
            if (!state.FirstName.HasValue || !state.LastName.HasValue)
            {
                return Page.Names;
            }

            if (!state.Gender.HasValue || !state.BirthDate.HasValue)
            {
                return Page.BirthGender;
            }

            if (!state.UserName.HasValue)
            {
                return Page.SetUserName;
            }

            return Page.Completed;
        }

        enum Page
        {
            Names,
            BirthGender,
            Completed,
            SetUserName
        }
    }
}
