using System;
using System.Collections;
using System.Linq;
using System.Text;
using Architecture.Injector.Core;
using DebugConsole;
using Localization.DTOs;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Web
{
    [Serializable]
    public struct RequestInformation
    {
        public WebMethod webMethod;
        public string server;
        public string json;
        public bool downloadBits;
        public StringKVDTO[] headers;

        public RequestInformation(WebMethod webMethod, string server, string json, bool downloadBits,
            params (string, string)[] headers)
        {
            this.webMethod = webMethod;
            this.server = server;
            this.json = json;
            this.downloadBits = downloadBits;
            this.headers = headers.Select(s => new StringKVDTO(s.Item1, s.Item2)).ToArray();
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }

        public (string, string)[] GetHeaders()
        {
            return headers.Select(kv => (kv.k, kv.v)).ToArray();
        }
    }

    public class InternalWebClient
    {
        private IDebugConsole debugConsole;

        public IObservable<RequestInfo> Get(string server, bool bits = false, params (string, string)[] headers)
        {
            return ObservableRequest(WebMethod.Get, server, bits, headers: headers);
        }

        public IObservable<RequestInfo> Post(string server, string json, bool bits = false,
            params (string, string)[] headers)
        {
            return ObservableRequest(WebMethod.Post, server, bits, json, headers);
        }

        public IObservable<RequestInfo> Delete(string server, bool bits = false, params (string, string)[] headers)
        {
            return ObservableRequest(WebMethod.Delete, server, bits, headers: headers);
        }

        public IObservable<RequestInfo> Put(string server, string json, bool bits = false,
            params (string, string)[] headers)
        {
            return ObservableRequest(WebMethod.Put, server, bits, json, headers);
        }

        public IObservable<RequestInfo> Request(RequestInformation info)
        {
            Debug.Log("rinfo: " + info);
            return ObservableRequest(info.webMethod, info.server, info.downloadBits, info.json, info.GetHeaders());
        }

        private IObservable<RequestInfo> ObservableRequest(WebMethod webMethod, string server,
            bool downloadBits, string data = "", params (string, string)[] headers)
        {
            UnityWebRequest www;
            switch (webMethod)
            {
                case WebMethod.Post:
                    www = new UnityWebRequest(server, "POST");
                    www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.SetRequestHeader("Content-Type", "application/json");
                    break;
                case WebMethod.Put:
                    www = new UnityWebRequest(server, "PUT");
                    www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.SetRequestHeader("Content-Type", "application/json");
                    break;
                case WebMethod.Delete:
                    www = UnityWebRequest.Delete(server);
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.SetRequestHeader("Content-Type", "application/json");
                    break;
                case WebMethod.Get:
                    www = UnityWebRequest.Get(server);
                    break;
                default:
                    www = UnityWebRequest.Get(server);
                    break;
            }

            www.SetRequestHeader("1", "2");
            www.SetRequestHeader("11", "21");
            www.SetRequestHeader("111", "211");

            foreach (var (key, value) in headers)
            {
                www.SetRequestHeader(key, value);
            }

            //const string userAgentValue = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            //Debug.Log($"User Agent Value: {userAgentValue}");


            //Debug.Log($" --- Web Request {webMethod} --- {server}  : {data}");

            IEnumerator RequestRoutine(UnityWebRequest req)
            {
                yield return req.SendWebRequest();
            }


            var requestInfoBuilder = new StringBuilder();
            requestInfoBuilder.Append("Server : ");
            requestInfoBuilder.Append(server);
            requestInfoBuilder.Append("\n");

            requestInfoBuilder.Append(webMethod);
            requestInfoBuilder.Append("Headers => ");
            requestInfoBuilder.Append(string.Join(" ; ", headers));
            requestInfoBuilder.Append("\n");

            requestInfoBuilder.Append("Headers : \n");

            requestInfoBuilder.Append("\n");
            requestInfoBuilder.Append("Body => " + data);
            requestInfoBuilder.Append("\n");
            var requestInfo = requestInfoBuilder.ToString();

            //Debug.Log($"about to send {requestInfo}");

            if (www == null)
            {
                throw new Exception("www error, www is null, web method: " + webMethod);
            }

            var wwwObservable = RequestRoutine(www).ToObservable()
                .Do(_ =>
                {
                    if (www.result == UnityWebRequest.Result.ConnectionError)
                        throw new Exception("Network error " + www.error + " " + server);
                    if (www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError(requestInfo);
                        throw new Exception("HTTP error " + www.downloadHandler.text + "  " + www.error + " " +
                                            requestInfo + "  " + server +
                                            "potential headers: " + headers);
                    }
                })
                .Select(_ =>
                {
                    var info = new RequestInfo();
                    var responseText = www.downloadHandler.text;
                    info.data = downloadBits ? www.downloadHandler.data : Array.Empty<byte>();
                    info.text = responseText;
                    try
                    {
                        info.json = JObject.Parse(responseText);
                    }
                    catch (Exception)
                    {
                        /* ignored */
                    }

                    return info;
                })
                .Do(info =>
                {
                    if (debugConsole==null)
                    {
                        debugConsole = Injection.Get<IDebugConsole>();
                    }
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.WEB_RESPONSE))
                    {
                        var text = info.text ?? "<empty>";
                        text = text.Substring(0, Mathf.Min(text.Length, 10000));
                        debugConsole.TraceLog(LOG_TYPE.WEB_RESPONSE, $"WEB RESPONSE: {server}  {text}");
                    }
                })
                .DoOnError(er => Debug.LogError(er));


            if (Application.isPlaying)
                return wwwObservable
                    .ObserveOnMainThread()
                    .First();

            return wwwObservable.First();
        }
    }
}