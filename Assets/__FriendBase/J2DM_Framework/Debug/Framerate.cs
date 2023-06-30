using UnityEngine;
using TMPro;

namespace DebugConsole
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Framerate : MonoBehaviour
    {
        private TextMeshProUGUI textFramerate;

        void Awake()
        {
            textFramerate = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            int framerate = Mathf.RoundToInt(1.0f / Time.deltaTime);
            textFramerate.text = framerate + "";
        }
    }
}

