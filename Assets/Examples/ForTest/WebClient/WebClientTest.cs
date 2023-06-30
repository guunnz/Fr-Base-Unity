using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Web;

namespace Examples.ForTest.WebClient
{
    public class WebClientTest : MonoBehaviour
    {
        public Button send;
        public TMP_Dropdown method;
        public TMP_InputField jsonInput;
        public TMP_InputField urlInput;
        public TextMeshProUGUI response;

        private void Start()
        {
            send
                .OnClickAsObservable()
                .SelectMany(_ => Req())
                .Subscribe(info =>
                    {
                        Debug.Log("LE RESPONSEEEE    " + info.text);
                        response.text = info.text;
                    },
                    Debug.LogError);
        }

        private IObservable<RequestInfo> Req()
        {
            return method.value switch
            {
                0 => Web.WebClient.Get(urlInput.text),
                1 => Web.WebClient.Post(urlInput.text, jsonInput.text),
                2 => Web.WebClient.Put(urlInput.text, jsonInput.text),
                3 => Web.WebClient.Delete(urlInput.text),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}