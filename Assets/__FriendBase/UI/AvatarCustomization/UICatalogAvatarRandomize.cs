using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.TabController;
using Data.Catalog;
using Architecture.Injector.Core;
using Data;
using Data.Users;
using UI.ScrollView;
using System;
using UniRx;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UICatalogAvatarRandomize : MonoBehaviour
{
    IGameData gameData = Injection.Get<IGameData>();

    Dictionary<ItemType, AvatarCustomizationRule> rules;

    public delegate void AvatarRandomizeReady(AvatarCustomizationSimpleData avatarCustomizationSimpleData);
    public event AvatarRandomizeReady OnAvatarRandomizeReady;

    public Button randomizeButton;

    void Start()
    {
        rules = gameData.GetAvatarCustomizationRules();
    }

    public void CreateOneRandomSkin()
    {
        StartCoroutine(CreateOneRandomSkinCoroutine());
    }

    IEnumerator CreateOneRandomSkinCoroutine()
    {
        var amountBodyParts = GameData.AvatarItemsType.Length;

        var dataUnits = new AvatarCustomizationSimpleDataUnit[amountBodyParts];

        for (var i = 0; i < amountBodyParts; i++)
        {
            var itemType = GameData.AvatarItemsType[i];
            var idItem = gameData.GetCatalogByItemType(itemType).GetRandomItem().IdItem;

            //If the item is deselectable we make a random and see if we disable it
            if (rules[itemType].Deselectable || itemType==ItemType.DRESSES)
            {
                if (Random.Range(0, 1000) > 500)
                {
                    idItem = -1;
                }
            }

            var idColor = GetRandomColorId(itemType);

            dataUnits[i] = new AvatarCustomizationSimpleDataUnit((int) itemType, idItem, idColor);
        }

        if (OnAvatarRandomizeReady!=null)
        {
            OnAvatarRandomizeReady(new AvatarCustomizationSimpleData(dataUnits));
        }
        yield return new WaitForSeconds(0.25f);
    }

    int GetRandomColorId(ItemType itemType)
    {
        int idColor = gameData.GetCatalogByItemType(ItemType.COLOR).GetRandomItem().IdItem;

        int[] colorsAvalable = rules[itemType].ColorIdsAvailable;
        if (colorsAvalable.Length > 0)
        {
            idColor = colorsAvalable[Random.Range(0, colorsAvalable.Length)];
        }

        return idColor;
    }
}