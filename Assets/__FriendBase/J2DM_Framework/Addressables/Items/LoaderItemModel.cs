using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Architecture.Injector.Core;

namespace AddressablesSystem
{
    public class LoaderItemModel : LoaderAbstractItem
    {
        public override LoaderItemTypes ItemType => LoaderItemTypes.MODEL;
        private AsyncOperationHandle<GameObject> _objHandle;

        public override Object GetNewItem()
        {
            if (State == LoaderItemState.SUCCEED)
            {
                return Injection.Get<ILoader>().InstantiateItem(_objHandle.Result);
            }
            return null;
        }

        public LoaderItemModel(string id)
        {
            Id = id;
            State = LoaderItemState.NONE;
        }

        public override void Load()
        {
            if (State == LoaderItemState.NONE || State == LoaderItemState.FAILED)
            {
                Addressables.LoadAssetAsync<GameObject>(Id).Completed += OnLoaded;
                State = LoaderItemState.LOADING;
            }
            else if (State == LoaderItemState.SUCCEED || State == LoaderItemState.ERROR)
            {
                SendDelegateItemLoadedResult();
            }
        }

        public GameObject GetModel()
        {
            Object obj = GetNewItem();
            if (obj != null)
            {
                return obj as GameObject;
            }
            return null;
        }

        public void OnLoaded(AsyncOperationHandle<GameObject> obj)
        {
            Addressables.LoadAssetAsync<GameObject>(Id).Completed -= OnLoaded;

            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                State = LoaderItemState.SUCCEED;
                _objHandle = obj;
            }
            else
            {
                State = LoaderItemState.FAILED;
            }
            SendDelegateItemLoadedResult();
        }
    }
}