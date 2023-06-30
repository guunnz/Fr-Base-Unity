using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomPanelButtonsManager : MonoBehaviour
{
    [SerializeField] private Button btnBurger;
    [SerializeField] private Button btnRooms;
    [SerializeField] private Button btnFurnitures;
    [SerializeField] private Button btnHome;
    [SerializeField] private GameObject gemsHolder;
    [SerializeField] private Button avatarButton;

    private IGameData gameData;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        UIStoreFurnituresManager.OnOpenEvent += OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent += OnCloseFurnituresStore;
    }

    void OnDestroy()
    {
        UIStoreFurnituresManager.OnOpenEvent -= OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent -= OnCloseFurnituresStore;
    }

    void OnOpenFurnituresStore()
    {
        HideAll();
    }

    void OnCloseFurnituresStore()
    {
        ShowAll();
    }

    public void HideAll()
    {
        btnBurger.gameObject.SetActive(false);
        btnRooms.gameObject.SetActive(false);
        btnFurnitures.gameObject.SetActive(false);
        btnHome.gameObject.SetActive(false);
        gemsHolder.gameObject.SetActive(false);
        avatarButton.gameObject.SetActive(false);
    }

    public void ShowAll()
    {
        bool isMyRoom = CurrentRoom.Instance.IsMyRoom();
;
        btnBurger.gameObject.SetActive(true);
        btnRooms.gameObject.SetActive(true);
        btnFurnitures.gameObject.SetActive(isMyRoom);
        btnHome.gameObject.SetActive(!isMyRoom);
        gemsHolder.gameObject.SetActive(true);
        avatarButton.gameObject.SetActive(true);
    }
}
