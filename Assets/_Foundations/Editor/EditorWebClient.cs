using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace FoundationEditorTools
{
    public static class EditorWebClient
    {
        private const string UserAgentValue = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

        public static string Post(string url, string data, Action<string> onResponse = null, Action onError = null)
        {
            try
            {
                Debug.Log($"request: {url} body: {data}");
                var http = (HttpWebRequest) WebRequest.Create(new Uri(url));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                var encoding = new ASCIIEncoding();
                var bytes = encoding.GetBytes(data);

                var newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                Debug.Log($"response : {content}");
                onResponse?.Invoke(content);
                
                return content;
            }
            catch (Exception)
            {
                onError?.Invoke();
            }

            return "";
        }


        public static string Get(string url, Action<string> onResponse = null, Action onError = null)
        {
            Debug.Log("request: " + url);
            using var client = new WebClient();
            client.Headers.Add("user-agent", UserAgentValue);
            var info = client.OpenRead(url);
            if (info != null)
            {
                var reader = new StreamReader(info);
                var content = reader.ReadToEnd();
                info.Close();
                reader.Close();
                Debug.Log("response: " + content);
                onResponse?.Invoke(content);
                return content;
            }

            onError?.Invoke();
            return "";
        }
    }
}