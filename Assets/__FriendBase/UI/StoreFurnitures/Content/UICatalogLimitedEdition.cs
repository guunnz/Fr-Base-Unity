using System.Collections;
using System.Collections.Generic;
using Data.Catalog;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICatalogLimitedEdition : MonoBehaviour
{
    [SerializeField] protected GameObject container;
    [SerializeField] protected GameObject[] limitedEditionCards;
    [SerializeField] protected GameObject containerIcons;
    [SerializeField] protected GameObject[] limitedEditionIcons;
    [SerializeField] protected Image imageItem;
    [SerializeField] protected Vector2 _anchorMinNormal = new Vector2(0.1f, 0.1f);
    [SerializeField] protected Vector2 _anchorMaxNormal = new Vector2(0.9f, 0.9f);

    [SerializeField] protected Vector2 _anchorMinLimited = new Vector2(0.1f, 0.0f);
    [SerializeField] protected Vector2 _anchorMaxLimited = new Vector2(0.9f, 0.8f);

    [SerializeField] protected Image imageIconInventory;
    [SerializeField] protected TextMeshProUGUI txtAmountInventory;

    private GenericCatalogItem objCat;
    private bool isInventoryItem;
    private string amountInInventory;

    public void CheckLimitedEdition(GenericCatalogItem objCat, bool isInventoryItem, string amountInInventory)
    {
        this.objCat = objCat;
        SetAnchors(_anchorMinNormal, _anchorMaxNormal);

        HideCards();
        HideIcons();

        this.isInventoryItem = isInventoryItem;
        this.amountInInventory = amountInInventory;

        if (this.objCat.LimitedEdition==null)
        {
            return;
        }

        if (!this.objCat.LimitedEdition.Equals(LimitedEditionType.NONE))
        {
            if (isInventoryItem)
            {
                ShowInventoryDesign();
            }
            else
            {
                ShowEditionCard();
            }
        }
    }

    void ShowInventoryDesign()
    {
        if (imageIconInventory!=null)
        {
            imageIconInventory.gameObject.SetActive(false);
        }
        if (txtAmountInventory != null)
        {
            txtAmountInventory.gameObject.SetActive(false);
        }

        containerIcons.SetActive(true);
        switch (this.objCat.LimitedEdition)
        {
            case LimitedEditionType.DEFAULT:
                limitedEditionIcons[0].SetActive(true);
                TextMeshProUGUI text0 = limitedEditionIcons[0].GetComponentInChildren<TextMeshProUGUI>();
                if (text0!=null)
                {
                    text0.text = amountInInventory;
                }
                break;
            case LimitedEditionType.HALLOWEEN:
                limitedEditionIcons[1].SetActive(true);
                TextMeshProUGUI text1 = limitedEditionIcons[1].GetComponentInChildren<TextMeshProUGUI>();
                if (text1 != null)
                {
                    text1.text = amountInInventory;
                }
                break;
            case LimitedEditionType.CHRISTMAS:
                limitedEditionIcons[2].SetActive(true);
                TextMeshProUGUI text2 = limitedEditionIcons[2].GetComponentInChildren<TextMeshProUGUI>();
                if (text2 != null)
                {
                    text2.text = amountInInventory;
                }
                break;
            case LimitedEditionType.VALENTINE:
                limitedEditionIcons[3].SetActive(true);
                TextMeshProUGUI text3 = limitedEditionIcons[3].GetComponentInChildren<TextMeshProUGUI>();
                if (text3 != null)
                {
                    text3.text = amountInInventory;
                }
                break;
            case LimitedEditionType.EASTER:
                limitedEditionIcons[4].SetActive(true);
                TextMeshProUGUI text4 = limitedEditionIcons[4].GetComponentInChildren<TextMeshProUGUI>();
                if (text4 != null)
                {
                    text4.text = amountInInventory;
                }
                break;
        }
    }

    void ShowEditionCard()
    {
        container.SetActive(true);
        switch (this.objCat.LimitedEdition)
        {
            case LimitedEditionType.DEFAULT:
                SetAnchors(_anchorMinLimited, _anchorMaxLimited);
                limitedEditionCards[0].SetActive(true);
                limitedEditionCards[0].GetComponentInChildren<TextMeshProUGUI>().text = "Limited edition";
                break;
            case LimitedEditionType.HALLOWEEN:
                SetAnchors(_anchorMinLimited, _anchorMaxLimited);
                limitedEditionCards[1].SetActive(true);
                limitedEditionCards[1].GetComponentInChildren<TextMeshProUGUI>().text = "Halloween '22";
                break;
            case LimitedEditionType.CHRISTMAS:
                SetAnchors(_anchorMinLimited, _anchorMaxLimited);
                limitedEditionCards[2].SetActive(true);
                limitedEditionCards[2].GetComponentInChildren<TextMeshProUGUI>().text = "Christmas exclusive";
                break;
            case LimitedEditionType.VALENTINE:
                SetAnchors(_anchorMinLimited, _anchorMaxLimited);
                limitedEditionCards[3].SetActive(true);
                limitedEditionCards[3].GetComponentInChildren<TextMeshProUGUI>().text = "Valentine's exclusive";
                break;
            case LimitedEditionType.EASTER:
                SetAnchors(_anchorMinLimited, _anchorMaxLimited);
                limitedEditionCards[4].SetActive(true);
                limitedEditionCards[4].GetComponentInChildren<TextMeshProUGUI>().text = "Easter's exclusive";
                break;
        }
    }

    void SetAnchors(Vector2 vectorMin, Vector2 vectorMax)
    {
        imageItem.GetComponent<RectTransform>().anchorMin = vectorMin;
        imageItem.GetComponent<RectTransform>().anchorMax = vectorMax;
    }

    void HideCards()
    {
        container.SetActive(false);
        int amount = limitedEditionCards.Length;
        for (int i=0; i<amount; i++)
        {
            limitedEditionCards[i].SetActive(false);
        }
    }

    void HideIcons()
    {
        containerIcons.SetActive(false);
        int amount = limitedEditionIcons.Length;
        for (int i = 0; i < amount; i++)
        {
            limitedEditionIcons[i].SetActive(false);
        }
    }
}
