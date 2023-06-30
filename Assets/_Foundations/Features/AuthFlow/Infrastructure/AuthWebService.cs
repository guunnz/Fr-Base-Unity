using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using Web;
using Newtonsoft.Json.Linq;
using UniRx.Diagnostics;
using UnityEngine;
using WebClientTools.Core.Services;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class AuthWebService : IAuthWebService
    {
        public AuthWebService()
        {
        }

        public IObservable<bool> EmailExist(string email)
        {
            return WebClient
                .Post(Constants.EmailExistsURL, new JObject {["email"] = email})
                .Select(resp => resp.json["exists"]?.Value<bool>() ?? true);
        }
    }
}

//
//  -----------------------------------------
//  ---  old create email implementation  ---
//  -----------------------------------------
//
// const string registrationType = "email";
// var json = new JObject
// {
//     ["user"] = new JObject
//     {
//         ["email"] = email,
//         ["firebase_uid"] = firebaseUid,
//         ["registration_type"] = registrationType //todo: we should ahve this be a real registration type
//     }
// }.ToString();
//
// var req = WebClient.Request(
//     webMethod: WebMethod.Post,
//     server: CreateEmailURL,
//     json: json,
//     bits: false,
//     ("Content-Type", "application/json"),
//     ("authorization", "Bearer token")
// );
//
//
// return req
//     .Select(r => r.json)
//     .ObserveOnMainThread()
//     .Select(valid => valid["data"]["email"])
//     .Select(emailToken => emailToken != null && !string.IsNullOrEmpty(emailToken.ToString()));
