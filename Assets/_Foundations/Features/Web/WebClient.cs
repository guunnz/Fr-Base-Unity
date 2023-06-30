using System;

namespace Web
{
    public static class WebClient
    {
        private static readonly Lazy<InternalWebClient> Client =
            new Lazy<InternalWebClient>(() => new InternalWebClient());

        public static IObservable<RequestInfo> Get(string server, bool bits = false, params (string, string)[] headers)
        {
            return Client.Value.Get(server, bits, headers);
        }

        public static IObservable<RequestInfo> Delete(string server, bool bits = false,
            params (string, string)[] headers)
        {
            return Client.Value.Delete(server, bits, headers);
        }

        public static IObservable<RequestInfo> Post(string server, object json, bool bits = false,
            params (string, string)[] headers)
        {
            return Client.Value.Post(server, json.ToString(), bits, headers);
        }

        public static IObservable<RequestInfo> Request(WebMethod webMethod, string server, string json, bool bits = false,
            params (string, string)[] headers) =>
            Client.Value.Request(new RequestInformation(webMethod, server, json, bits, headers));

        public static IObservable<RequestInfo> Put(string server, object json, bool bits = false,
            params (string, string)[] headers)
        {
            return Client.Value.Put(server, json.ToString(), bits, headers);
        }
    }
}