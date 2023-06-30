using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.TabController;
using Data.Catalog;

public class UICatalogAvatarTabController : TabController
{
    [SerializeField] private Load2DObject load2DObject;
    [SerializeField] private GameObject selectedTabGameobject;

    public ItemType ItemType { get; private set; }

    public void SetTab(ItemType itemType)
    {
        selectedTabGameobject.SetActive(false);
        ItemType = itemType;
        if (load2DObject != null)
        {
            load2DObject.Load(UIReferences.TAB_STORE + (int)itemType);
        }
    }

    protected override void OnSelectTab()
    {
        selectedTabGameobject.SetActive(true);
    }

    protected override void OnUnselectTab()
    {
        selectedTabGameobject.SetActive(false);
    }
}
