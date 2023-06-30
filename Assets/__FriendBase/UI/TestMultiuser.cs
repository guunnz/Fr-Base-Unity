using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiuser;
using Architecture.Injector.Core;
using AddressablesSystem;
using DebugConsole;
using Data;
using Data.Bag;
using UniRx;
using Data.Catalog;
using Newtonsoft.Json.Linq;
using Web;
using Newtonsoft.Json;
using Data.Catalog.Items;
using JetBrains.Annotations;
using WebClientTools.Core.Services;
using System.Linq;
using FriendsView.Core.Domain;
using AuthFlow.Firebase.Core.Actions;
using System;
using Data.Users;

//Test Class that do nothing
public class TestMultiuser : MonoBehaviour
{
    [SerializeField] protected UICatalogAvatarManager catalogAvatarManager;

    CompositeDisposable disposables = new CompositeDisposable();

    private IItemTypeUtils ItemTypeUtils = Injection.Get<IItemTypeUtils>();
    private IGameData gameData = Injection.Get<IGameData>();

    public void UsingPostExample()
    {
        string url = Constants.ApiRoot + "/store/";
        var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImY1NWUyOTRlZWRjMTY3Y2Q5N2JiNWE4MTliYmY3OTA2MzZmMTIzN2UiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vZnJpZW5kYmFzZS1kZXYiLCJhdWQiOiJmcmllbmRiYXNlLWRldiIsImF1dGhfdGltZSI6MTYzNjczNDI3OSwidXNlcl9pZCI6InJNZDdaVk1FRmtZZ21DWm9GQzZUclVWT0l3dTEiLCJzdWIiOiJyTWQ3WlZNRUZrWWdtQ1pvRkM2VHJVVk9Jd3UxIiwiaWF0IjoxNjM2NzM0Mjc5LCJleHAiOjE2MzY3Mzc4NzksImVtYWlsIjoiYy5wYWxhY2lvc0BvcHRpY3Bvd2VyLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImMucGFsYWNpb3NAb3B0aWNwb3dlci5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.YR2jNaUA5ya5keHKtMLQHLvNUULLKCpXYFKa1vQa0n0nZwO-TNPmj6DXGPCVMcil6AwOUSYrQcRp-khlwOIRWKrFGHDhnY_6j32c0WqG95O5bpN8W1h4v7CitUwXGpcb2u4JciIfb4-D_NleG8FWXddqV5Xv2W_DiBv_usuHHR36KjW0BYpbsW9IUw8w5T5PBK5DXsEfLLil4eUvvdSgfozAgCOUHJBpr5VBPuaunGOFoGXG5hMhK0AkJpJ5xWHlFp9BoYRcuMq3TNEqQVwvhNulGpLJlmdq-cLt1lvgeWi4k8ZEjEJT8xaYBoHCav7w2zJGObi8JqCB1Rfxfzr6kg";
        var authHeader = ("authorization", "Bearer " + token);

        WebClient.Get(url, false, authHeader).Do(info =>
        {
            JObject jsonResponse = info.json;
            Debug.Log("Server responds : " + jsonResponse.ToString(Formatting.Indented));

            Debug.Log("DATA--- " + jsonResponse["data"].Values<JObject>());
            //Debug.Log("<color=green>" + jsonString + "</color>");

        }).Subscribe();
    }

    //1- Get Catalog
    //2- Get Bag
    //3- Purchase Items
    //4- Get Avatar Skin
    //5- Set Avatar Skin
    //6- IAP Validation

    // 3VSMskowwLYtcC9M4BjTnNvNiDY2

    readonly GetFirebaseUid getFirebaseUid;

    readonly CompositeDisposable sectionDisposables = new CompositeDisposable();


    void Awake()
    {
        //UsingPostExample();

        //IAvatarEndpoints webClient = Injection.Get<IAvatarEndpoints>();

        //webClient.GetAvatarCatalogItemsList();

        ////InventoryEndPoint();

        //    return new AvatarGenericCatalogItem(itemType, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog,
        //        gemsPrice, goldPrice, currencyType, layers);
        //}).ToList();

        //Loader Test
        //ILoader loader = Injection.Get<ILoader>();
        //loader.LoadItem(new J2DM_LoaderItemSprite("Avatar-nose"));
        //_loadObject2D.Load("Avatar-nose");

        //Debug Test
        // if (Injection.Get<IDebugConsole>().isLogTypeEnable(LOG_TYPE.ANALYTICS)) Injection.Get<IDebugConsole>().TraceLog(LOG_TYPE.ANALYTICS, "---------DEBUG ANALYTICS");

        //Multiuser Test
        /*
        IMultiuser serviceMultiuser = Injection.Get<IMultiuser>();
        serviceMultiuser.Connect(new ConnectMultiuserParams("", "", 8080), null);

        serviceMultiuser.Suscribe(ReceiveTramaLogin, IDTramas.USER_LOGIN);
        serviceMultiuser.SuscribeSendTrama(SendTramaLogin, IDTramas.USER_LOGIN);

        serviceMultiuser.Send(new OutputUserLogin("matiasini@gmail.com", "1234"));
        */
        //serviceMultiuser.OnTrama(IDTramas.USER_LOGIN).Subscribe(ReceiveTramaLogin).AddTo(disposables);
        //serviceMultiuser.DeliverTriggerTramaToSuscribers(new IncomingUserLogin(0));


        //Injection.Get<IAvatarEndpoints>().GetPlayerInventory()
        //   .Subscribe(listBagItems =>
        //   {
        //       gameData.AddItemsToBag(listBagItems);
        //   });

        //Injection.Get<IAvatarEndpoints>().GetAvatarCatalogItemsList()
        //   .Subscribe(listItems =>
        //   {
        //       gameData.InitializeCatalogs(listItems.ToList<GenericCatalogItem>());
        //   });

        //Injection.Get<IAvatarEndpoints>().GetAvatarSkin()
        //   .Subscribe(json =>
        //   {
        //       string jsonString = json.ToString(Formatting.Indented);
        //       Debug.Log("GetAvatarSkin <color=green>" + jsonString + "</color>");
        //       //AvatarCustomizationData avatarData = new AvatarCustomizationData();
        //       //avatarData.SetData(json);
        //       //avatarData.GetSerializeData();
        //   });
    }

    void Start()
    {
        //catalogAvatarManager.Open();
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    public void ReceiveTramaLogin(AbstractIncomingTrama trama)
    {
        Debug.Log("----- ReceiveTrama:" + trama.TramaID);
    }

    public void SendTramaLogin(AbstractTrama trama)
    {
        Debug.Log("----- SendTramaLogin:" + trama.TramaID);
    }
}
