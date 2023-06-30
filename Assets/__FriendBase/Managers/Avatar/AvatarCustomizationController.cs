using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Users;
using Data.Catalog;
using Data;
using Architecture.Injector.Core;
using DebugConsole;
using Gradients;
using System;
using Newtonsoft.Json.Linq;
using Data.Catalog.Items;
using System.Linq;

public class AvatarCustomizationController : MonoBehaviour
{
    public AvatarCustomizationData AvatarCustomizationData { get; private set; }

    private IGameData gameData;
    private Dictionary<ItemType, AvatarCustomizationRule> avatarCustomizationRules;
    private Dictionary<ItemType, Load2DSpriteRenderer[]> load2DSpriteRendererByItem;
    private IDebugConsole debugConsole;
    private bool changingCustom;
    private Coroutine ToggleClothCoroutine;
    private List<GameObject> objectsToActivate = new List<GameObject>();
    private List<GameObject> objectsToDeactivate = new List<GameObject>();


    void Awake()
    {
        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();
        avatarCustomizationRules = gameData.GetAvatarCustomizationRules();
        AvatarCustomizationData = new AvatarCustomizationData();
        load2DSpriteRendererByItem = new Dictionary<ItemType, Load2DSpriteRenderer[]>();

        ReadGameObjectReferences();
    }

    void ReadGameObjectReferences()
    {
        foreach (ItemType itemType in GameData.AvatarItemsType)
        {
            int amount = avatarCustomizationRules[itemType].GameObjectNames.Length;
            Load2DSpriteRenderer[] spriteRenderers = new Load2DSpriteRenderer[amount];

            for (int i = 0; i < amount; i++)
            {
                spriteRenderers[i] =
                    GetSpriteRendererChildByName(avatarCustomizationRules[itemType].GameObjectNames[i]);
                if (spriteRenderers[i] == null)
                {
                    debugConsole.ErrorLog("AvatarCustomizationController:ReadGameObjectReferences", "Null GameObject",
                        "itemType:" + itemType + " name:" + avatarCustomizationRules[itemType].GameObjectNames[i]);
                }
            }

            load2DSpriteRendererByItem.Add(itemType, spriteRenderers);
        }
    }

    Load2DSpriteRenderer GetSpriteRendererChildByName(string gameObjectName)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name.Equals(gameObjectName))
            {
                return child.gameObject.GetComponent<Load2DSpriteRenderer>();
            }
        }

        return null;
    }

    public AvatarCustomizationSimpleData GetSerializeData()
    {
        return AvatarCustomizationData.GetSerializeData();
    }

    public JObject GetSerializeDataWebClient()
    {
        return AvatarCustomizationData.GetSerializeDataWebClient();
    }

    public void SetAvatarCustomizationDataFromJoinRoom(JObject avatarCustomizationJsonData)
    {
        AvatarCustomizationData.SetDataFromJoinRoom(avatarCustomizationJsonData);

        SetAvatarCustomizationData(AvatarCustomizationData.GetSerializeData());
    }

    public void SetAvatarCustomizationData(JObject avatarCustomizationJsonData)
    {
        AvatarCustomizationData.SetDataFromUserSkin(avatarCustomizationJsonData);

        SetAvatarCustomizationData(AvatarCustomizationData.GetSerializeData());
    }

    public void SetAvatarCustomizationData(AvatarCustomizationSimpleData avatarCustomizationSimpleData)
    {
        if (changingCustom)
        {
            return;
        }

        StartCoroutine(CSetAvatarCustomizationData(avatarCustomizationSimpleData));
    }

    private IEnumerator CSetAvatarCustomizationData(AvatarCustomizationSimpleData avatarCustomizationSimpleData)
    {
        changingCustom = true;
        Vector3 lastPosition = this.transform.localPosition;
        this.transform.localPosition = new Vector2(10000, 0);
        foreach (AvatarCustomizationSimpleDataUnit avatarCustomizationSimpleDataUnit in avatarCustomizationSimpleData
         .DataUnits)
        {
            ItemType itemType = (ItemType)avatarCustomizationSimpleDataUnit.ItemType;

            if (Array.IndexOf(GameData.AvatarItemsType, itemType) >= 0)
            {
                AvatarGenericCatalogItem catalogItem =
                    gameData.GetCatalogByItemType(itemType).GetItem(avatarCustomizationSimpleDataUnit.IdItem) as
                        AvatarGenericCatalogItem;
                if (catalogItem != null)
                {
                    ChangeAvatarPart(itemType, catalogItem);
                }
                else
                {
                    if (avatarCustomizationRules[itemType].Deselectable)
                    {
                        ChangeAvatarPart(itemType, null);
                    }
                    else
                    {
                        debugConsole.ErrorLog("AvatarCustomizationController:SetAvatarCustomizationData",
                            "Invalid IdItem",
                            "itemType:" + itemType + " IdItem:" + avatarCustomizationSimpleDataUnit.IdItem);
                    }
                }
            }
            else
            {
                debugConsole.ErrorLog("AvatarCustomizationController:SetAvatarCustomizationData", "Invalid itemType",
                    "itemType:" + itemType);
            }


            ColorCatalogItem colorCatalogItem =
                gameData.GetCatalogByItemType(ItemType.COLOR).GetItem(avatarCustomizationSimpleDataUnit.IdColor) as
                    ColorCatalogItem;
            if (colorCatalogItem != null)
            {
                ChangeColorPart(itemType, colorCatalogItem);
            }
            else
            {
                debugConsole.ErrorLog("AvatarCustomizationController:SetAvatarCustomizationData", "Invalid idColor",
                    "idColor:" + avatarCustomizationSimpleDataUnit.IdColor);
            }
        }

        while (!IsAvatarReady())
        {
            yield return new WaitForEndOfFrame();
        }

        changingCustom = false;
        this.transform.localPosition = lastPosition;
    }

    public void ChangeColorPart(ItemType itemType, ColorCatalogItem colorCatalogItem)
    {
        AvatarCustomizationRule rule = avatarCustomizationRules[itemType];
        if (rule.ColorIdsAvailable.Length == 0)
        {
            //This bodyPart can not change color
            return;
        }

        AvatarCustomizationData.ChangeColorPart(itemType, colorCatalogItem);

        UpdateAllColorsByItemType(itemType, colorCatalogItem);

        if (rule.CanDisableColor)
        {
            FillGameObjectsByType(itemType, AvatarCustomizationData.GetDataUnit(itemType).AvatarObjCat);
        }
    }

    public void UpdateAllColorsByItemType(ItemType itemTypeToUpdate, ColorCatalogItem colorCatalogItem)
    {
        foreach (ItemType itemType in GameData.AvatarItemsType)
        {
            AvatarCustomizationRule rule = avatarCustomizationRules[itemType];
            if (itemType == itemTypeToUpdate || rule.DependencyColor == itemTypeToUpdate)
            {
                ChangeAvatarPartColor(itemType, colorCatalogItem);
            }
        }
    }

    public void ChangeAvatarPartColor(ItemType itemType, ColorCatalogItem colorCatalogItem)
    {
        AvatarCustomizationRule rule = avatarCustomizationRules[itemType];

        Load2DSpriteRenderer[] load2DSpriteRenderers = load2DSpriteRendererByItem[itemType];
        int amountGameObjects = load2DSpriteRenderers.Length;

        for (int i = 0; i < amountGameObjects; i++)
        {
            if (rule.ApplyColorOnGameObject[i])
            {
                //load2DSpriteRenderers[i].GetComponent<SpriteRenderer>().color = colorCatalogItem.Color;
                load2DSpriteRenderers[i].GetComponent<GradientItemController>().SetGradientColor(colorCatalogItem);
            }
        }
    }

    public void ChangeAvatarPart(ItemType itemTypeToChange, AvatarGenericCatalogItem catalogItem, bool changeClothPreventNakedFrame = false)
    {
        AvatarCustomizationData.ChangePart(itemTypeToChange, catalogItem);

        AvatarCustomizationRule rule = avatarCustomizationRules[itemTypeToChange];
        

        if (changeClothPreventNakedFrame)
        {
            CheckForceDisableItems(itemTypeToChange, true);
            CheckForceEnableItems(itemTypeToChange, true);
            StartCoroutine(FillGameObjectsByTypeForAvatarCustomization(itemTypeToChange, catalogItem));
        }
        else
        {
            CheckForceDisableItems(itemTypeToChange);
            CheckForceEnableItems(itemTypeToChange);
            FillGameObjectsByType(itemTypeToChange, catalogItem);
        }

        if (rule.IsBoobMaster)
        {
            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                AvatarCustomizationRule currentRule = avatarCustomizationRules[itemType];
                if (currentRule.UseBoobs != -1)
                {
                    if (AvatarCustomizationData.GetDataUnit(itemType).AvatarObjCat != null)
                    {
                        if (changeClothPreventNakedFrame)
                        {
                            StartCoroutine(FillGameObjectsByTypeForAvatarCustomization(itemType, AvatarCustomizationData.GetDataUnit(itemType).AvatarObjCat));
                        }
                        else
                        {
                            FillGameObjectsByType(itemType, AvatarCustomizationData.GetDataUnit(itemType).AvatarObjCat);
                        }
                    }
                }
            }
        }
    }

    void CheckForceDisableItems(ItemType itemType, bool fromAvatarCustomization = false)
    {
        //Check Force Disable Prop
        AvatarCustomizationRule rule = avatarCustomizationRules[itemType];

        if (rule.ForceDisableItemTypes != null)
        {
            foreach (ItemType currentItemType in rule.ForceDisableItemTypes)
            {
                if (fromAvatarCustomization)
                {
                    DisableItemTypeAvatarCustomization(currentItemType);
                }
                else
                {
                    DisableItemType(currentItemType);
                }
            }
        }
    }

    void CheckForceEnableItems(ItemType itemType, bool fromAvatarCustomization = false)
    {
        //Check force enable Items
        AvatarCustomizationRule rule = avatarCustomizationRules[itemType];

        if (rule.ForceEnableItemTypes != null)
        {
            foreach (ItemType currentItemType in rule.ForceEnableItemTypes)
            {
                if (fromAvatarCustomization)
                {
                    ActivateGameObjectsByTypeFromAvatarCustomization(currentItemType);
                }
                else
                {
                    ActivateGameObjectsByType(currentItemType);
                }
            }
        }
    }

    void FillGameObjectsByType(ItemType itemType, AvatarGenericCatalogItem catalogItem)
    {
        if (catalogItem == null)
        {
            DeactivateGameObjectsByType(itemType);
            return;
        }

        AvatarCustomizationRule rule = avatarCustomizationRules[catalogItem.ItemType];
        Load2DSpriteRenderer[] load2DSpriteRenderers = load2DSpriteRendererByItem[catalogItem.ItemType];
        int amountGameObjects = load2DSpriteRenderers.Length;
        bool isBoobsActive = AvatarCustomizationData.IsBoobsActive();
        int idColor = AvatarCustomizationData.GetDataUnit(catalogItem.ItemType).ColorObjCat.IdItem;

        if (rule.FillGameObjectType == AvatarCustomizationRule.FillGameObjectsType.SEQUENCE)
        {
            //Fill in Sequence Type
            DeactivateGameObjectsByType(catalogItem.ItemType);

            int amountLayers = catalogItem.Layers.Length;
            for (int i = 0; i < amountLayers; i++)
            {
                int layer = catalogItem.Layers[i];
                if (layer >= 0 && layer <= amountGameObjects)
                {
                    load2DSpriteRenderers[layer].gameObject.SetActive(true);
                    string namePrefab = catalogItem.GetNamePrefabByItem(layer, isBoobsActive);
                    load2DSpriteRenderers[layer].Load(namePrefab, true);
                }
            }
        }
        else if (rule.FillGameObjectType == AvatarCustomizationRule.FillGameObjectsType.FIX)
        {
            DeactivateGameObjectsByType(catalogItem.ItemType);

            for (int i = 0; i < amountGameObjects; i++)
            {
                load2DSpriteRenderers[i].gameObject.SetActive(true);
                string namePrefab = catalogItem.GetNamePrefabByItem(i, isBoobsActive);
                load2DSpriteRenderers[i].Load(namePrefab, false);

                if (idColor == 0 && rule.CanDisableColor && rule.ApplyColorOnGameObject[i])
                {
                    load2DSpriteRenderers[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void DeactivateAllGameObjects()
    {
        foreach (ItemType itemType in GameData.AvatarItemsType)
        {
            DeactivateGameObjectsByType(itemType);
        }
    }

    void DeactivateGameObjectsByType(ItemType itemType)
    {
        Load2DSpriteRenderer[] spriteRenderers = load2DSpriteRendererByItem[itemType];
        int amountGameObjects = spriteRenderers.Length;

        //Deactivate all gameobjects of a specific type
        for (int i = 0; i < amountGameObjects; i++)
        {
            spriteRenderers[i].gameObject.SetActive(false);
        }
    }

    void ActivateGameObjectsByType(ItemType itemType)
    {
        AvatarCustomizationDataUnit dataUnit = AvatarCustomizationData.GetDataUnit(itemType);
        if (dataUnit.AvatarObjCat == null)
        {
            if (dataUnit.LastAvatarObjCat != null)
            {
                dataUnit.SetAvatarGenericCatalogItem(dataUnit.LastAvatarObjCat);
            }
            else
            {
                //We do not have last element => we set the default values
                dataUnit.SetAvatarGenericCatalogItem(gameData.GetDefaultAvatarCatalogItem(itemType));
                dataUnit.SetColorObjCat(gameData.GetDefaultColorCatalogItem(itemType));
            }
        }

        FillGameObjectsByType(itemType, dataUnit.AvatarObjCat);
        UpdateAllColorsByItemType(itemType, dataUnit.ColorObjCat);
    }

    void ActivateGameObjectsByTypeFromAvatarCustomization(ItemType itemType)
    {
        AvatarCustomizationDataUnit dataUnit = AvatarCustomizationData.GetDataUnit(itemType);
        if (dataUnit.AvatarObjCat == null)
        {
            if (dataUnit.LastAvatarObjCat != null)
            {
                dataUnit.SetAvatarGenericCatalogItem(dataUnit.LastAvatarObjCat);
            }
            else
            {
                //We do not have last element => we set the default values
                dataUnit.SetAvatarGenericCatalogItem(gameData.GetDefaultAvatarCatalogItem(itemType));
                dataUnit.SetColorObjCat(gameData.GetDefaultColorCatalogItem(itemType));
            }
        }

        StartCoroutine(FillGameObjectsByTypeForAvatarCustomization(itemType, dataUnit.AvatarObjCat));
        UpdateAllColorsByItemType(itemType, dataUnit.ColorObjCat);
    }

    void DeactivateGameObjectsByTypeForAvatarCustomization(ItemType itemType)
    {
        Load2DSpriteRenderer[] spriteRenderers = load2DSpriteRendererByItem[itemType];
        int amountGameObjects = spriteRenderers.Length;

        //Deactivate all gameobjects of a specific type
        for (int i = 0; i < amountGameObjects; i++)
        {
            objectsToDeactivate.Add(spriteRenderers[i].gameObject);
        }
    }

    IEnumerator FillGameObjectsByTypeForAvatarCustomization(ItemType itemType, AvatarGenericCatalogItem catalogItem)
    {
        if (catalogItem == null)
        {
            DisableItemTypeForAvatarCustomization(itemType);
            yield break;
        }

        AvatarCustomizationRule rule = avatarCustomizationRules[catalogItem.ItemType];
        Load2DSpriteRenderer[] load2DSpriteRenderers = load2DSpriteRendererByItem[catalogItem.ItemType];
        int amountGameObjects = load2DSpriteRenderers.Length;
        bool isBoobsActive = AvatarCustomizationData.IsBoobsActive();
        int idColor = AvatarCustomizationData.GetDataUnit(catalogItem.ItemType).ColorObjCat.IdItem;

        if (rule.FillGameObjectType == AvatarCustomizationRule.FillGameObjectsType.SEQUENCE)
        {
            //Fill in Sequence Type
            //DeactivateGameObjectsByType(catalogItem.ItemType);
            DisableItemTypeForAvatarCustomization(catalogItem.ItemType);

            int amountLayers = catalogItem.Layers.Length;
            for (int i = 0; i < amountLayers; i++)
            {
                int layer = catalogItem.Layers[i];
                if (layer >= 0 && layer <= amountGameObjects)
                {
                    string namePrefab = catalogItem.GetNamePrefabByItem(layer, isBoobsActive);
                    load2DSpriteRenderers[layer].Load(namePrefab, false);
                    objectsToActivate.Add(load2DSpriteRenderers[layer].gameObject);
                }
            }

            if (ToggleClothCoroutine == null)
                ToggleClothCoroutine = StartCoroutine(ToggleClothAvatarCustomization());
        }
        else if (rule.FillGameObjectType == AvatarCustomizationRule.FillGameObjectsType.FIX)
        {
            DeactivateGameObjectsByType(catalogItem.ItemType);

            for (int i = 0; i < amountGameObjects; i++)
            {
                load2DSpriteRenderers[i].gameObject.SetActive(true);
                string namePrefab = catalogItem.GetNamePrefabByItem(i, isBoobsActive);
                load2DSpriteRenderers[i].Load(namePrefab, false);

                if (idColor == 0 && rule.CanDisableColor && rule.ApplyColorOnGameObject[i])
                {
                    load2DSpriteRenderers[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisableItemTypeForAvatarCustomization(ItemType itemType)
    {
        DeactivateGameObjectsByTypeForAvatarCustomization(itemType);
    }

    public IEnumerator ToggleClothAvatarCustomization()
    {
        while (objectsToActivate.Any(x => x.GetComponent<Load2DSpriteRenderer>().loadingState == Load2DSpriteRenderer.LOADING_STATE.LOADING))
        {
            yield return null;
        }
        //DeactivateAllGameObjects();
        objectsToDeactivate.ForEach(x => x.SetActive(false));
        objectsToActivate.ForEach(x => x.SetActive(true));
        yield return null;
        objectsToActivate.Clear();
        objectsToDeactivate.Clear();
        ToggleClothCoroutine = null;
    }
    public void DisableItemTypeAvatarCustomization(ItemType itemType)
    {
        DeactivateGameObjectsByTypeForAvatarCustomization(itemType);
        AvatarCustomizationData.DisableItemType(itemType);
    }

    public void DisableItemType(ItemType itemType)
    {
        DeactivateGameObjectsByType(itemType);
        AvatarCustomizationData.DisableItemType(itemType);
    }

    public bool IsAvatarReady()
    {
        foreach (ItemType itemType in GameData.AvatarItemsType)
        {
            Load2DSpriteRenderer[] load2DSpriteRenderers = load2DSpriteRendererByItem[itemType];
            foreach (Load2DSpriteRenderer load2DSpriteRenderer in load2DSpriteRenderers)
            {
                if (load2DSpriteRenderer.loadingState == Load2DSpriteRenderer.LOADING_STATE.LOADING || load2DSpriteRenderer.loadingState == Load2DSpriteRenderer.LOADING_STATE.FAILED)
                {
                    return false;
                }
            }
        }
        return true;
    }
}