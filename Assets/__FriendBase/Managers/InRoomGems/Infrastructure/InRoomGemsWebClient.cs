using System;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Data;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using Web;
using WebClientTools.Core.Services;

namespace Managers.InRoomGems.Infrastructure
{
    public static class InRoomGemsWebClient
    {
        private static (bool, int) result;

        public static async Task<(bool, int)> RewardGems(string gemToken)
        {
            try
            {
                var gemJson = new JObject
                {
                    ["token"] = gemToken
                };

                var endpoint = Constants.UsersEndpoint + "/" + Injection.Get<IGameData>().GetUserInformation().UserId +
                               "/reward-gem";

                var header = await Injection.Get<IWebHeadersBuilder>().BearerToken;


                var response = await WebClient.Post(endpoint, gemJson, true, header);
                Debug.Log("Reward gems Response " + response);

                var jResponse = response.json;
                
                var userJson = jResponse["data"];

                var newGemsAmount = userJson["gems"].Value<int>();

                result = (true, newGemsAmount);
            }
            catch (Exception e)
            {
                result = (false, 0);
                Debug.LogError(
                    "Unable to reward gem. The token may have just expired when user touch the gem or request could have failed");
            }

            return result;
        }
    }
}