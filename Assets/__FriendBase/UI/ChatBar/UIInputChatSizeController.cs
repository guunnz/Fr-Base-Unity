using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class UIInputChatSizeController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private float minValue;
    private float maxValue;
    private float padding = 10;
    void Start()
    {
        inputField.onValueChanged.AddListener(ResizeInputField);
        rectTransform = inputField.GetComponent<RectTransform>();
        minValue = rectTransform.sizeDelta.x;
        maxValue = Screen.width * 0.70f * canvas.scaleFactor;
    }

    private async void ResizeInputField(string text)
    {
        await Task.Delay(1);
        float textWidth = inputField.textComponent.preferredWidth;

        //Debug.Log("textWidth: " + textWidth);
        float newWidth = Mathf.Clamp(textWidth + padding, minValue, maxValue);

        //Debug.Log("newWidth: " + textWidth);
        rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
    }

    void Update()
    {
        
    }
}
