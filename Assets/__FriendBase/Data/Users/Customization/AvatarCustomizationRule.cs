using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Data;
using DebugConsole;
using Architecture.Injector.Core;

public class AvatarCustomizationRule
{
    public enum FillGameObjectsType { FIX, SEQUENCE };

    public ItemType ItemType { get; private set; }
    public string PsbName { get; private set; }
    public string PsbNameUI { get; private set; }
    public string[] GameObjectNames { get; private set; }
    public string[] SubfixForGameObjects { get; private set; }
    public FillGameObjectsType FillGameObjectType { get; private set; }
    public bool[] ApplyColorOnGameObject { get; private set; }
    public bool IsBoobMaster { get; private set; }
    public int UseBoobs { get; private set; }
    public ItemType[] ForceEnableItemTypes { get; private set; }
    public ItemType[] ForceDisableItemTypes { get; private set; }
    public int[] ColorIdsAvailable { get; private set; }
    public ItemType DependencyColor { get; private set; }
    public int IdItemDefault { get; private set; }
    public int IdColorDefault { get; private set; }
    public bool Deselectable { get; private set; }
    public bool CanDisableColor { get; private set; }

    public AvatarCustomizationRule(ItemType itemType, string psbName, string psbNameUI, string[] gameObjectNames, string[] subfixForGameObjects, FillGameObjectsType fillGameObjectType, bool[] applyColorOnGameObject, bool isBoobMaster, int useBoobs, ItemType[] forceEnableItemTypes, ItemType[] forceDisableItemTypes, int[] colorIdsAvailable, ItemType dependencyColor, int idItemDefault, int idColorDefault, bool deselectable, bool canDisableColor)
    {
        ItemType = itemType;
        PsbName = psbName;
        PsbNameUI = psbNameUI;
        GameObjectNames = gameObjectNames;
        SubfixForGameObjects = subfixForGameObjects;
        FillGameObjectType = fillGameObjectType;
        ApplyColorOnGameObject = applyColorOnGameObject;
        IsBoobMaster = isBoobMaster;
        UseBoobs = useBoobs;
        ForceEnableItemTypes = forceEnableItemTypes;
        ForceDisableItemTypes = forceDisableItemTypes;
        ColorIdsAvailable = colorIdsAvailable;
        DependencyColor = dependencyColor;
        IdItemDefault = idItemDefault;
        IdColorDefault = idColorDefault;
        Deselectable = deselectable;
        CanDisableColor = canDisableColor;

        if (gameObjectNames.Length != subfixForGameObjects.Length || subfixForGameObjects.Length != applyColorOnGameObject.Length)
        {
            Injection.Get<IDebugConsole>().ErrorLog("AvatarCustomizationRule", "Inconsistence Array Length", "ItemType:"+ ItemType);
        }
    }
}
