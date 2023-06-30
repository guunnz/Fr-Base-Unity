using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using DebugConsole;
using UnityEngine;
namespace Data.Catalog
{
    public class GenericCatalog 
    {
        public bool Ready { get; private set; }
        public ItemType ItemType{ get; }
        protected Dictionary<int, GenericCatalogItem> _elements = new Dictionary<int, GenericCatalogItem>();
        protected List<GenericCatalogItem> _listElements = new List<GenericCatalogItem>();
        public List<GenericCatalogItem> listElements
        {
            get {
                CreateListFromDictionary();
                return _listElements;
            }
        }
        public GenericCatalog(ItemType itemType)
        {
            ItemType = itemType;
            Ready = false;
        }
        public virtual void Initialize(List<GenericCatalogItem> listElements)
        {
            if (listElements == null)
            {
                return;
            }
            _elements = new Dictionary<int, GenericCatalogItem>();
            int amount = listElements.Count;
            for (int i = 0; i < amount; i++)
            {
                try
                {
                    if (ItemType == listElements[i].ItemType)
                    {
                        _elements.Add(listElements[i].IdItem, listElements[i]);
                    }
                    else
                    {
                        Injection.Get<IDebugConsole>().ErrorLog("GenericCatalog:Initialize", "Invalid element:" + listElements[i].ItemType + " idItem:" + listElements[i].IdItem, "Catalog itemType:" + ItemType);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e + " PREFAB NAME: " + listElements[i].NamePrefab + " / ITEM TYPE:" + ItemType.ToString());
                }
            }
            Ready = true;
        }
        public virtual void AddItem(GenericCatalogItem item)
        {
            if (item != null)
            {
                if (ItemType == item.ItemType)
                {
                    _elements.Add(item.IdItem, item);
                    _listElements = new List<GenericCatalogItem>();
                }
                else
                {
                    Injection.Get<IDebugConsole>().ErrorLog("GenericCatalog:AddItem", "Invalid element:" + item.ItemType + " idItem:" + item.IdItem, "Catalog itemType:" + ItemType);
                }
            }
        }
        public virtual GenericCatalogItem GetItem(int id)
        {
            if (_elements.ContainsKey(id))
            {
                return _elements[id];
            }
            return null;
        }
        public virtual GenericCatalogItem GetItemByPrefabName(string name)
        {
            foreach (KeyValuePair<int, GenericCatalogItem> element in _elements)
            {
                if (element.Value.NamePrefab.Equals(name))
                {
                    return element.Value;
                }
            }
            return null;
        }
        public virtual GenericCatalogItem GetItemByIdWebClient(int IdItemWebClient)
        {
            foreach (KeyValuePair<int, GenericCatalogItem> element in _elements)
            {
                if (element.Value.IdItemWebClient == IdItemWebClient)
                {
                    return element.Value;
                }
            }
            return null;
        }
        public virtual GenericCatalogItem GetItemByIndex(int index)
        {
            if (_listElements.Count == 0)
            {
                CreateListFromDictionary();
            }
            if (index >= 0 && index < _listElements.Count)
            {
                return _listElements[index];
            }
            return null;
        }
        public virtual GenericCatalogItem GetRandomItem()
        {
            if (_listElements.Count == 0)
            {
                CreateListFromDictionary();
            }
            GenericCatalogItem objCat = _listElements[UnityEngine.Random.Range(0, _listElements.Count)];

            while (!objCat.ActiveInCatalog)
            {
                objCat = _listElements[UnityEngine.Random.Range(0, _listElements.Count)];
            }
            return objCat;
        }
        void CreateListFromDictionary()
        {
            _listElements = new List<GenericCatalogItem>();
            foreach (KeyValuePair<int, GenericCatalogItem> element in _elements)
            {
                _listElements.Add(element.Value);
            }
        }
        public virtual int GetAmountItems()
        {
            if (_listElements.Count == 0)
            {
                CreateListFromDictionary();
            }
            return _listElements.Count;
        }
        public virtual int[] GetArrayIds()
        {
            if (_listElements.Count == 0)
            {
                CreateListFromDictionary();
            }
            int amount = _listElements.Count;
            int[] arrayIds = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                arrayIds[i] = _listElements[i].IdItem;
            }
            return arrayIds;
        }
    }
}