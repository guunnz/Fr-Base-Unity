using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesSystem
{
    public interface ILoader
    {
        bool AddressableAssetExists(object key);
        void LoadItem(LoaderAbstractItem item);
        void LoadItems(List<LoaderAbstractItem> items);
        void ReleaseItems();
        UnityEngine.Object InstantiateItem(UnityEngine.Object obj);
        Sprite InstantiateSprite(UnityEngine.Object obj);
        LoaderAbstractItem GetItem(string id);
        GameObject GetModel(string id);
        void Suscribe(IReceiveLoadedItem suscriber, string[] idItems);
        void Suscribe(IReceiveLoadedItem suscriber, string idItem);
        void Unsuscribe(IReceiveLoadedItem suscriber, string[] idItems);
        void Unsuscribe(IReceiveLoadedItem suscriber, string idItem);
        void DeliverItemToSuscribers(LoaderAbstractItem item);
    }
}