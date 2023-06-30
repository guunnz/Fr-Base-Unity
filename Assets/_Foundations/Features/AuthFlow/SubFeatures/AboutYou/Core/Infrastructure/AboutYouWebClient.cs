using System;
using System.Collections.Generic;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Firebase.Core.Actions;
using Firebase.Auth;
using Functional.Maybe;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using Web;
using WebClientTools.Core.Services;

namespace AuthFlow.AboutYou.Core.Infrastructure
{
    [UsedImplicitly]
    public class AboutYouWebClient : IAboutYouWebClient
    {
        readonly GetFirebaseUid getFirebaseUid;
        readonly IWebHeadersBuilder headersBuilder;

        public IObservable<Web.RequestInfo> GetUserData()
        {
            var user = new JObject();

            return headersBuilder.BearerToken.SelectMany(header =>
                {
                    string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                    return  WebClient.Get($"{Constants.UsersEndpoint}/{uid}", true, header);
                }
            );
        }


        public IObservable<Unit> UpdateUserData(Maybe<string> firstName, Maybe<string> lastName, Maybe<DateTime> birthDate, Maybe<string> gender, Maybe<string> userName)
        {
            var user = new JObject();

            firstName.NoEmpty().SetOnField(user, "first_name");
            lastName.NoEmpty().SetOnField(user, "last_name");
            birthDate.ToISO().NoEmpty().SetOnField(user, "birthday");

            gender = gender.NoEmpty().ToLower();

            if (gender == "prefer not to say")
            {
              gender = "pnts"; // NOTE(Jesse): I identify as pants.
            }

            gender.SetOnField(user, "gender");

            userName.NoEmpty().SetOnField(user, "username");

            // TODO(Jesse): We should actually ask Firebase if the user has confirmed their email
            Maybe<string> confirmedString = "confirmed";
            confirmedString.SetOnField(user, "email_confirmation");

            // TODO(Jesse): We should query the UserData struct to make sure this
            // is actually true!!
            Maybe<string> termsAccepted = "true";
            termsAccepted.SetOnField(user, "terms_accepted");

            var json = new JObject {["user"] = user};

            // TODO(Jesse): We should actually return a boolean here in case the web reqeuest fails.
            // Right now this throws an exception I believe, which is annoying for callers to have
            // to deal with
            return headersBuilder.BearerToken.SelectMany(header =>
                {
                    return getFirebaseUid
                        .Execute()
                        .Select(uid => $"{Constants.UsersEndpoint}/{uid}")
                        .SelectMany(endpoint => WebClient.Put(endpoint, json, true, header))
                        .AsUnitObservable();
                }
            );
        }

        public IObservable<bool> IsAvailableUserName(string userName)
        {
            return headersBuilder.BearerToken.SelectMany(header =>
            {
                return WebClient
                    .Post(Constants.UsernameValidationEndpoint,
                        new JObject {["username"] = userName},
                        true, header)
                    .Select(r => r.json)
                    .Select(AllowedAndNotExist);
            });
        }

        static bool AllowedAndNotExist(JObject json)
        {
            return json["allowed"].Value<bool>() && !json["exists"].Value<bool>();
        }

        public AboutYouWebClient(GetFirebaseUid getFirebaseUid, ILocalUserInfo localUserInfo,
            IWebHeadersBuilder headersBuilder)
        {
            this.getFirebaseUid = getFirebaseUid;
            this.headersBuilder = headersBuilder;
        }

        public IObservable<List<string>> GetGendersOptions()
        {
            Debug.Log("Ask For Gender Options");

            return Observable.Return(new List<string>
            {
                "Prefer not to say",
                "Male",
                "Female",
                "Other",
            });
        }
    }
}

//Todo: where to return the enter your name and chage eter toyiu name
//clear data when you roll back to enter your name
