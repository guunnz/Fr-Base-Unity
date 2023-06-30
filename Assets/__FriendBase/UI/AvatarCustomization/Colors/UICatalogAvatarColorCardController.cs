using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ScrollView;
using System;
using Data.Catalog;

public class UICatalogAvatarColorCardController : UIAbstractCardController
{
    protected Action<EventType, GenericCatalogItem, UIAbstractCardController> callback;
    [SerializeField] protected Image imageColor;
    [SerializeField] protected Image imageNoColor;
    [SerializeField] protected Image selectedBorder;
    [SerializeField] protected Image selectedIcon;

    public ColorCatalogItem ObjCat { get; private set; }

    private bool isCardSelected;
    public bool IsCardSelected
    {
        get { return isCardSelected; }
        set {
            isCardSelected = value;
            ControlIfSelectable();
        }
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        ObjCat = itemData as ColorCatalogItem;
        if (ObjCat == null)
        {
            return;
        }
        this.callback = callback;
        IsCardSelected = false;
        SetUpColorCard();
    }

    void HideAll()
    {
        imageColor.gameObject.SetActive(false);
        imageNoColor.gameObject.SetActive(false);
        selectedBorder.gameObject.SetActive(false);
        selectedIcon.gameObject.SetActive(false);
    }

    void SetUpColorCard()
    {
        HideAll();
        if (ObjCat.IdItem==0)
        {
            //No Color
            imageNoColor.gameObject.SetActive(true);
        }
        else
        {
            imageColor.gameObject.SetActive(true);
            imageColor.color = ObjCat.Color;
        }
        ControlIfSelectable();
    }

    void ControlIfSelectable()
    {
        selectedBorder.gameObject.SetActive(isCardSelected);
        selectedIcon.gameObject.SetActive(isCardSelected);
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, ObjCat,this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, ObjCat, this);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
