using System;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Functional.Maybe;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace AuthFlow.AboutYou.Infrastructure
{
    [UsedImplicitly]
    public class AboutYouStateManager : IAboutYouStateManager
    {
        JObject json = new JObject();

        readonly ILocalUserInfo localStorage;

        public AboutYouStateManager(ILocalUserInfo localStorage)
        {
            this.localStorage = localStorage;
        }

        void Save()
        {
            localStorage["aboutYouState"] = json.ToString();
        }

        void Load()
        {
            var storage = localStorage["aboutYouState"];
            json = string.IsNullOrEmpty(storage) ? new JObject() : JObject.Parse(storage);
        }

        public void Clear()
        {
          localStorage["aboutYouState"] = "{}";
        }

        public Maybe<string> FirstName
        {
            get
            {
                Load();
                return json.ValueMaybe<string>("first_name");
            }
            set
            {
                value.SetOnField(json, "first_name");
                Save();
            }
        }

        public Maybe<string> LastName
        {
            get
            {
                Load();
                return json.ValueMaybe<string>("last_name");
            }
            set
            {
                value.SetOnField(json, "last_name");

                Save();
            }
        }

        public Maybe<string> Gender
        {
            get
            {
                Load();
                return json.ValueMaybe<string>("gender");
            }
            set
            {
                value.SetOnField(json, "gender");
                Save();
            }
        }

        public Maybe<DateTime> BirthDate
        {
            get
            {
                Load();
                return json.ValueMaybe<long>("birthday").Select(
                    ticks => {
                    return ticks > 0 ?
                      new DateTime(ticks) :
                      Maybe<DateTime>.Nothing;
                    }
                );
            }
            set
            {
                value.Do(v => json["birthday"] = v.Ticks);
                Save();
            }
        }

        public Maybe<string> UserName
        {
            get
            {
                Load();
                return json.ValueMaybe<string>("username");
            }
            set
            {
                value.SetOnField(json, "username");
                Save();
            }
        }
    }
}
