using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AddressablesSystem
{
    public abstract class LoaderAbstractItem
    {
        public delegate void ItemLoadedResult(LoaderAbstractItem item);
        public static event ItemLoadedResult OnItemLoadedResult;

        public abstract LoaderItemTypes ItemType { get; }
        public string Id { get; protected set; }
        public LoaderItemState State { get; protected set; }

        public abstract UnityEngine.Object GetNewItem();
        public abstract void Load();

        public void SendDelegateItemLoadedResult()
        {
            if (OnItemLoadedResult != null)
            {
                OnItemLoadedResult(this);
            }
        }

        public void SetItemError()
        {
            //This is used when the item is not in addressables, we do this so that Unity do not crash
            State = LoaderItemState.ERROR;
        }
    }
}