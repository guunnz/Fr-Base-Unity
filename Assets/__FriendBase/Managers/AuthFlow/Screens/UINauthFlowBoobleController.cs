using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UINauthFlowBoobleController : AbstractUIPanel
{
    public enum IconType { QUESTION, ALERT, INFO };

    [SerializeField] private TextMeshProUGUI txtBubble;
    [SerializeField] private Image iconContainer;

    [SerializeField] private Sprite iconQuestion;
    [SerializeField] private Sprite iconAlert;
    [SerializeField] private Sprite iconInfo;

    private IconType iconType { get; set; }

    public void ShowBubble(string text, IconType iconType)
    {
        base.Open();
        txtBubble.text = text;
        this.iconType = iconType;
        switch (iconType)
        {
            case IconType.QUESTION:
                iconContainer.sprite = iconQuestion;
                txtBubble.color = new Color32(48, 57, 71, 255);
                break;
            case IconType.ALERT:
                iconContainer.sprite = iconAlert;
                txtBubble.color = new Color32(219, 59, 36, 255);
                break;
            case IconType.INFO:
                iconContainer.sprite = iconInfo;
                txtBubble.color = new Color32(75, 84, 93, 255);
                break;
        }
    }
}
