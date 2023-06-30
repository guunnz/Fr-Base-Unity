using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using Socket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarPetManager : MonoBehaviour
{
    private IGameData gameData = Injection.Get<IGameData>();

    private GenericBag petsBag;

    private ILoader loader;

    internal int? CurrentPetId = null;

    private PetBehaviour CurrentPet;

    internal string PetName;

    private void Start()
    {
        loader = Injection.Get<ILoader>();

    }

    public GameObject GetCurrentPetObject()
    {
        if (CurrentPet == null)
        {
            return null;
        }
        return CurrentPet.gameObject;
    }

    public GenericBagItem GetCurrentPet()
    {
        return petsBag.listElements.Single(x => x.ObjCat.IdItem == CurrentPetId);
    }

    public void SetPetNull(bool useSockets = true)
    {
        if (CurrentPet == null)
            return;
        CurrentPetId = null;
        PetName = "";
        Destroy(CurrentPet.gameObject);
        if (useSockets)
        {
            SimpleSocketManager.Instance.SendCurrentPet(CurrentRoom.Instance.RoomInformation.RoomName, CurrentRoom.Instance.RoomInformation.RoomIdInstance, CurrentPetId, CurrentPetId);
        }
    }

    public void SetPet(int? petId = null, int? petIdInGame = null, bool useSocket = true, string PetPrefabName = "")
    {
        StartCoroutine(SetPetCoroutine(petId, petIdInGame, useSocket, PetPrefabName));
    }

    public bool HasAnyPet()
    {
        petsBag = gameData.GetBagByItemType(Data.Catalog.ItemType.PETS);
        return petsBag != null && petsBag.listElements.Count > 0;
    }

    public void OnDestroy()
    {
        if (CurrentPet != null)
            Destroy(CurrentPet.gameObject);
    }

    public IEnumerator SetPetCoroutine(int? petId = null, int? petIdInGame = null, bool useSocket = true, string PetPrefabName = "")
    {
        yield return null;
        SetPetNull(false);
        //GenericBagItem petOnBag = petsBag.listElements.Single(x => x.ObjCat.IdItem == petId); // Get selected pet from socket

        string petName = PetPrefabName;// should get also prefabname
        if (PetPrefabName == null || petId == 0)
        {
            yield break;
        }
        PetName = PetPrefabName;
        CurrentPetId = petId;

        loader.LoadItem(new LoaderItemModel(petName + "_Prefab"));

        LoaderAbstractItem item = loader.GetItem(petName + "_Prefab");
        while (item == null || item.State != LoaderItemState.SUCCEED)
        {
            yield return null;
        }

        CurrentPet = loader.GetModel(petName + "_Prefab").GetComponent<PetBehaviour>();

        CurrentPet.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.2f, this.transform.position.z);

        CurrentPet.SetAvatarToFollow(this.transform);

        if (useSocket)
            SimpleSocketManager.Instance.SendCurrentPet(CurrentRoom.Instance.RoomInformation.RoomName, CurrentRoom.Instance.RoomInformation.RoomIdInstance, CurrentPetId, petIdInGame);

    }
}
