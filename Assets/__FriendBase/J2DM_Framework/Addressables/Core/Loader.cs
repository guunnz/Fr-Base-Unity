using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using Architecture.Context;
using Architecture.Injector.Core;

namespace AddressablesSystem
{
    public class Loader: ScriptModule, ILoader
    {
        public delegate void AllItemsLoaded();
        public static event AllItemsLoaded OnAllItemsLoaded;

        private Dictionary<string, string> listItemsToLoad = new Dictionary<string, string>();
        private Dictionary<string, LoaderAbstractItem> items = new Dictionary<string, LoaderAbstractItem>();

        private Dictionary<string, List<IReceiveLoadedItem>> itemsDeliveries;

        private List<LoaderDataToCommit> listSuscribersToCommit = new List<LoaderDataToCommit>();
        private List<LoaderAbstractItem> listItemsToDeliver = new List<LoaderAbstractItem>();

        public override void Init()
        {
            Injection.Register<ILoader>(this);
            itemsDeliveries = new Dictionary<string, List<IReceiveLoadedItem>>();
            LoaderAbstractItem.OnItemLoadedResult += OnItemLoadedResult;
            Addressables.InitializeAsync().Completed += (op) =>
            {
                //Addressables Ready
            };
        }

        private void OnItemLoadedResult(LoaderAbstractItem item)
        {
            listItemsToDeliver.Add(item);
        }

        public bool AddressableAssetExists(object key)
        {
            if (Application.isPlaying)
            {
                foreach (var l in Addressables.ResourceLocators)
                {
                    IList<IResourceLocation> locs;
                    if (l.Locate(key, null, out locs))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public void LoadItem(LoaderAbstractItem item)
        {
            string id = item.Id;
            if (!items.ContainsKey(id))
            {
                items.Add(id, item);
            }

            LoaderAbstractItem currentItem = items[id];
            if (!listItemsToLoad.ContainsKey(currentItem.Id))
            {
                listItemsToLoad.Add(currentItem.Id, currentItem.Id);
            }

            if (!AddressableAssetExists(id))
            {
                currentItem.SetItemError();
            }

            currentItem.Load();
        }

        public void LoadItems(List<LoaderAbstractItem> items)
        {
            int amount = items.Count;
            for (int i = 0; i < amount; i++)
            {
                string id = items[i].Id;
                if (!this.items.ContainsKey(id))
                {
                    this.items.Add(id, items[i]);
                }

                LoaderAbstractItem currentItem = this.items[id];
                if (!listItemsToLoad.ContainsKey(currentItem.Id))
                {
                    listItemsToLoad.Add(currentItem.Id, currentItem.Id);
                }

                currentItem.Load();
            }
        }

        public void ReleaseItems()
        {

        }

        public UnityEngine.Object InstantiateItem(UnityEngine.Object obj)
        {
            return Instantiate(obj);
        }

        public Sprite InstantiateSprite(UnityEngine.Object obj)
        {
            return Instantiate(obj) as Sprite;
        }

        public LoaderAbstractItem GetItem(string id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }
            return null;
        }

        public GameObject GetModel(string id)
        {
            if (items.ContainsKey(id))
            {
                LoaderItemModel modelItem = items[id] as LoaderItemModel;
                if (modelItem != null)
                {
                    return modelItem.GetModel();
                }
            }
            return null;
        }

        private void Update()
        {
            //Check Suscribe/Unsuscribe
            int amount = listSuscribersToCommit.Count;
            for (int i = 0; i < amount; i++)
            {
                LoaderDataToCommit data = listSuscribersToCommit[i];
                if (data.typeCommit == LoaderDataToCommit.TypeCommit.SUSCRIBE)
                {
                    SuscribeCommit(data);
                }
                else if (data.typeCommit == LoaderDataToCommit.TypeCommit.UNSUSCRIBE)
                {
                    UnsuscribeCommit(data);
                }
            }
            listSuscribersToCommit.Clear();

            //Check Deliver Items
            if (listItemsToDeliver.Count > 0)
            {
                int amountDeliver = listItemsToDeliver.Count;
                for (int i = 0; i < amountDeliver; i++)
                {
                    LoaderAbstractItem item = listItemsToDeliver[i];
                    DeliverItemToSuscribers(item);

                    if (listItemsToLoad.ContainsKey(item.Id))
                    {
                        listItemsToLoad.Remove(item.Id);
                    }

                    if (listItemsToLoad.Count == 0)
                    {
                        if (OnAllItemsLoaded != null)
                        {
                            OnAllItemsLoaded();
                        }
                    }
                }

                listItemsToDeliver.Clear();
            }
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //-------------------------   S U S C R I B E  ------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public void Suscribe(IReceiveLoadedItem suscriber, string[] idItems)
        {
            if (suscriber == null)
            {
                return;
            }
            if (idItems == null || idItems.Length == 0)
            {
                return;
            }

            int amount = idItems.Length;
            for (int i = 0; i < amount; i++)
            {
                listSuscribersToCommit.Add(new LoaderDataToCommit(idItems[i], suscriber, LoaderDataToCommit.TypeCommit.SUSCRIBE));
            }
        }

        public void Suscribe(IReceiveLoadedItem suscriber, string idItem)
        {
            if (suscriber == null)
            {
                return;
            }
            if (idItem == null)
            {
                return;
            }

            listSuscribersToCommit.Add(new LoaderDataToCommit(idItem, suscriber, LoaderDataToCommit.TypeCommit.SUSCRIBE));
        }

        private void SuscribeCommit(LoaderDataToCommit data)
        {
            if (!itemsDeliveries.ContainsKey(data.id))
            {
                itemsDeliveries[data.id] = new List<IReceiveLoadedItem>();
            }
            itemsDeliveries[data.id].Add(data.suscriber);
        }

        public void Unsuscribe(IReceiveLoadedItem suscriber, string[] idItems)
        {
            if (suscriber == null)
            {
                return;
            }
            if (idItems == null || idItems.Length == 0)
            {
                return;
            }

            int amount = idItems.Length;
            for (int i = 0; i < amount; i++)
            {
                listSuscribersToCommit.Add(new LoaderDataToCommit(idItems[i], suscriber, LoaderDataToCommit.TypeCommit.UNSUSCRIBE));
            }
        }

        public void Unsuscribe(IReceiveLoadedItem suscriber, string idItem)
        {
            if (suscriber == null)
            {
                return;
            }
            if (idItem == null)
            {
                return;
            }

            listSuscribersToCommit.Add(new LoaderDataToCommit(idItem, suscriber, LoaderDataToCommit.TypeCommit.UNSUSCRIBE));
        }

        private void UnsuscribeCommit(LoaderDataToCommit data)
        {
            if (itemsDeliveries.ContainsKey(data.id))
            {
                for (int j = itemsDeliveries[data.id].Count - 1; j >= 0; j--)
                {
                    if (itemsDeliveries[data.id][j] == data.suscriber)
                    {
                        itemsDeliveries[data.id].RemoveAt(j);
                    }
                }
            }
        }

        public void DeliverItemToSuscribers(LoaderAbstractItem item)
        {
            if (item == null)
            {
                return;
            }
            string itemId = item.Id;
            if (itemsDeliveries.ContainsKey(itemId))
            {
                int amount = itemsDeliveries[itemId].Count;
                for (int i = 0; i < amount; i++)
                {
                    itemsDeliveries[itemId][i].ReceiveLoadedItem(item);
                }
            }
        }
    }
}