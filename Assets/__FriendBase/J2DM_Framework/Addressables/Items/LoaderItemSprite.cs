using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using Architecture.Injector.Core;

namespace AddressablesSystem
{
    public class LoaderItemSprite : LoaderAbstractItem
    {
        public override LoaderItemTypes ItemType => LoaderItemTypes.SPRITE;
        private AsyncOperationHandle<Sprite> _objHandle;

        public override UnityEngine.Object GetNewItem()
        {
            if (State == LoaderItemState.SUCCEED)
            {
                return Injection.Get<ILoader>().InstantiateSprite(_objHandle.Result);
            }
            return null;
        }

        public LoaderItemSprite(string id)
        {
            Id = id;
            State = LoaderItemState.NONE;
        }

        public override void Load()
        {
            if (State == LoaderItemState.NONE || State == LoaderItemState.FAILED)
            {
                Addressables.LoadAssetAsync<Sprite>(Id).Completed += OnLoaded;
                State = LoaderItemState.LOADING;
            }
            else if (State == LoaderItemState.SUCCEED || State == LoaderItemState.ERROR)
            {
                SendDelegateItemLoadedResult();
            }
        }

        public Sprite GetSprite()
        {
            Object obj = GetNewItem();
            if (obj != null)
            {
                return obj as Sprite;
            }
            return null;
        }

        public void OnLoaded(AsyncOperationHandle<Sprite> obj)
        {
            Addressables.LoadAssetAsync<Sprite>(Id).Completed -= OnLoaded;

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