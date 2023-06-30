using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.AboutYou.View
{
    public class NamedButton : MonoBehaviour
    {
        public UIWidget<string> label;
        public Button button;

        public void Show(string content, Action onClick)
        {
            Clean();
            label.Value = content;
            button.onClick.AddListener(() => onClick());
            gameObject.SetActive(true);
        }

        void EnsureComponents()
        {
            label ??= GetComponentInChildren<UIWidget<string>>();
            button ??= GetComponentInChildren<Button>();
            if (label == null)
            {
                Debug.LogError("label null", gameObject);
            }
        }

        public void Hide()
        {
            Clean();
            gameObject.SetActive(false);
        }

        void Clean()
        {
            EnsureComponents();
            button.onClick.RemoveAllListeners();
            label.Value = string.Empty;
        }
    }
}