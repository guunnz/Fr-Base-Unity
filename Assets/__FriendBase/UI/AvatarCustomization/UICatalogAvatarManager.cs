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
using Functional.Maybe;
using UniRx;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LocalizationSystem;
using Socket;
using UnityEngine.SceneManagement;

public class UICatalogAvatarManager : MonoBehaviour
{
    public enum AVATAR_PANEL_TYPE
    {
        CREATE_AVATAR,
        CHANGE_AVATAR
    };
    IAnalyticsSender analyticsSender;
    [SerializeField] private UICatalogAvatarScrollView catalogAvatarScrollView;
    [SerializeField] private UICatalogAvatarColorScrollView catalogAvatarColorScrollView;
    [SerializeField] private TabManager principalTabManager;
    [SerializeField] private TabManager secondaryTabManager;
    [SerializeField] private AvatarCustomizationController avatarControllerPrefab;
    [SerializeField] private UIDialogPanel panelCustomizationComplete;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private UICatalogAvatarRandomize catalogAvatarRandomize;

    [SerializeField] private UIPanelChangeAvatar panelChangeAvatar;
    [SerializeField] private UIPanelCreateAvatar panelCreateAvatar;
    [SerializeField] private UIFirstTimeModal firstTimeModal;
    //[SerializeField] private GameObject loaderPanel;
    [SerializeField] private ParticleSystem randomParticlesPrefab;

    public AVATAR_PANEL_TYPE AvatarPanelType { get; set; }

    private IGameData gameData = Injection.Get<IGameData>();
    private Dictionary<ItemType, AvatarCustomizationRule> rules;

    private AvatarCustomizationData avatarCustomizationData;
    private AvatarCustomizationController avatarCustomizationController;

    public delegate void CreateAvatarComplete();
    public event CreateAvatarComplete OnCreateAvatarComplete;

    public delegate void ChangeAvatarComplete();
    public event ChangeAvatarComplete OnChangeAvatarComplete;

    private ParticleSystem randomParticles;
    protected ILanguage language;

    private List<List<ItemType>> itemTypesSecondaryTabs = new List<List<ItemType>>
    {
        new List<ItemType>
        {
            ItemType.BODY,
            ItemType.HAIR,
            ItemType.FACE,
            ItemType.EAR,
            ItemType.EYEBROW,
            ItemType.EYE,
            ItemType.NOSE,
            ItemType.MOUTH,
        },
        new List<ItemType>
        {
            ItemType.UP_PART,
            ItemType.BOTTOM_PART,
            ItemType.DRESSES,
            ItemType.SHOES,
            ItemType.GLASSES,
            ItemType.ACCESORIES,
        }
    };

    protected Camera _cameraAvatar;

    void Awake()
    {
        rules = gameData.GetAvatarCustomizationRules();
        language = Injection.Get<ILanguage>();
        analyticsSender = Injection.Get<IAnalyticsSender>();
    }

    private void OnUserBanned()
    {
        SceneManager.UnloadSceneAsync(GameScenes.AvatarCustomization);
    }

    public void TalkAnimation()
    {
        avatarCustomizationController.GetComponent<AvatarAnimationController>().SetTalkAnimation(3);
    }

    public void Open(AVATAR_PANEL_TYPE avatarPanelType)
    {
        //loaderPanel.SetActive(false);
        this.AvatarPanelType = avatarPanelType;

        StartCoroutine(WaitAndOpen());
    }

    IEnumerator WaitAndOpen()
    {
        yield return new WaitForEndOfFrame();

        SimpleSocketManager.Instance.OnUserBanned += OnUserBanned;

        CreateRenderCamera();

        catalogAvatarScrollView.SetAvatarPanelType(AvatarPanelType);

        principalTabManager.OnTabSelected += OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected += OnSecondaryTabSelected;
        catalogAvatarScrollView.OnCardSelected += OnAvatarCardSelected;
        catalogAvatarColorScrollView.OnCardSelected += OnColorCardSelected;
        catalogAvatarRandomize.OnAvatarRandomizeReady += UpdateAvatarData;

        //We have a copy of the avatar customization data
        avatarCustomizationController.SetAvatarCustomizationData(gameData.GetUserInformation().GetAvatarCustomizationData().GetSerializeData());
        avatarCustomizationData = avatarCustomizationController.AvatarCustomizationData;
        catalogAvatarScrollView.SetAvatarCustomizationData(avatarCustomizationData);
        catalogAvatarColorScrollView.SetAvatarCustomizationData(avatarCustomizationData);
        principalTabManager.SetTab(0);

        panelCreateAvatar.Close();
        panelChangeAvatar.Close();

        if (AvatarPanelType == AVATAR_PANEL_TYPE.CREATE_AVATAR)
        {
            panelCreateAvatar.Open();
            firstTimeModal.Open();
        }
        else
        {
            panelChangeAvatar.Open(avatarCustomizationData);
        }

        panelChangeAvatar.OnBackButtonPressed += OnBackFromChangeAvatar;
    }

    public void Close()
    {
        principalTabManager.OnTabSelected -= OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected -= OnSecondaryTabSelected;
        catalogAvatarScrollView.OnCardSelected -= OnAvatarCardSelected;
        catalogAvatarColorScrollView.OnCardSelected -= OnColorCardSelected;
        panelChangeAvatar.OnBackButtonPressed -= OnBackFromChangeAvatar;
        catalogAvatarRandomize.OnAvatarRandomizeReady -= UpdateAvatarData;

        panelCreateAvatar.Close();
        panelChangeAvatar.Close();

        if (_cameraAvatar)
        {
            Destroy(_cameraAvatar.gameObject);
        }

        SimpleSocketManager.Instance.OnUserBanned -= OnUserBanned;
    }

    void CreateRenderCamera()
    {
        const float scaleAvatar = 20;

        GameObject emptyGameobject = new GameObject();
        GameObject camera = Instantiate(emptyGameobject, transform.position, Quaternion.identity);
        camera.name = "AvatarCamera";
        camera.transform.position = new Vector3(0, 2000, 0);

        GameObject avatar = Instantiate(avatarControllerPrefab, transform.position, Quaternion.identity).gameObject;
        avatar.transform.localScale = Vector3.one * scaleAvatar;
        avatarCustomizationController = avatar.GetComponent<AvatarCustomizationController>();

        avatarCustomizationController.transform.SetParent(camera.transform);
        avatarCustomizationController.transform.localPosition = new Vector3(0, -220, 400);

        _cameraAvatar = camera.AddComponent<Camera>();
        _cameraAvatar.clearFlags = CameraClearFlags.SolidColor;
        _cameraAvatar.targetTexture = renderTexture;

        randomParticles = Instantiate(randomParticlesPrefab, transform.position, Quaternion.identity);
        randomParticles.transform.localScale = Vector3.one * 1.3f;
        randomParticles.transform.SetParent(camera.transform);
        randomParticles.transform.localPosition = new Vector3(0, 0, 403);
    }

    void OnAvatarCardSelected(AvatarGenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        //Check if it the same item that was selected
        if (catalogItem.IdItem == avatarCustomizationController.AvatarCustomizationData
            .GetDataUnit(catalogItem.ItemType).AvatarObjCat?.IdItem)
        {
            //Same item, check if we can deselect it
            if (rules[catalogItem.ItemType].Deselectable)
            {
                avatarCustomizationController.DisableItemType(catalogItem.ItemType);
            }
        }
        else
        {
            avatarCustomizationController.ChangeAvatarPart(catalogItem.ItemType, catalogItem, true);
        }
    }

    void OnColorCardSelected(ColorCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        avatarCustomizationController.ChangeColorPart(catalogAvatarScrollView.ItemType, catalogItem);
        catalogAvatarScrollView.ChangeColor(catalogItem.IdItem);
    }

    public void Open()
    {
        //loaderPanel.SetActive(false);
        //principalTabManager.SetTab(0);
    }

    void OnPrincipalTabSelected(int index)
    {
        ShowSecondaryTabs(index);
        secondaryTabManager.SetTab(0);
    }

    void OnSecondaryTabSelected(int index)
    {
        UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(index) as UICatalogAvatarTabController;
        catalogAvatarScrollView.ShowObjects(tabItem.ItemType,
            avatarCustomizationData.GetDataUnit(tabItem.ItemType).ColorObjCat.IdItem);
        catalogAvatarColorScrollView.ShowObjects(tabItem.ItemType);

        //Check if we show/hide tab colors
        AvatarCustomizationRule rule = rules[tabItem.ItemType];
        int[] colorsAvalable = rule.ColorIdsAvailable;

        catalogAvatarColorScrollView.gameObject.SetActive(colorsAvalable.Length > 0);
        catalogAvatarScrollView.EnlargeAnchorsScrollView(colorsAvalable.Length > 0);
    }

    void HideSecondaryTabs()
    {
        secondaryTabManager.HideAllTabs();
        secondaryTabManager.UnselectAllTabs();
    }

    void ShowSecondaryTabs(int index)
    {
        HideSecondaryTabs();
        List<ItemType> itemTypes = itemTypesSecondaryTabs[index];
        int amount = itemTypes.Count;
        for (int i = 0; i < amount; i++)
        {
            UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(i) as UICatalogAvatarTabController;
            tabItem.gameObject.SetActive(true);
            tabItem.SetTab(itemTypes[i]);
        }
    }

    //It is call when you complete creating your avatar for the first time
    public void OnBtnCreateAvatarComplete()
    {
        string txtTitle = language.GetTextByKey(LangKeys.STORE_WAIT_A_SEC);
        string txtDesc = language.GetTextByKey(LangKeys.STORE_THIS_IS_YOUR_LAST_CHANCE_FREE);
        string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_YES_SAVE);
        string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_NO_KEEP_EDITING);

        panelCustomizationComplete.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
        {
            //loaderPanel.SetActive(true);
            JObject json = avatarCustomizationData.GetSerializeDataWebClient();
            analyticsSender.SendAnalytics(AnalyticsEvent.CreatedAvatarFirstTime);
            Injection.Get<IAvatarEndpoints>().SetAvatarSkin(json)
               .Subscribe(json =>
               {
                   //loaderPanel.SetActive(false);
                   gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarCustomizationData);
                   gameData.AddSkinToInventory(avatarCustomizationData);

                   if (OnCreateAvatarComplete != null)
                   {
                       OnCreateAvatarComplete();
                   }
               });
        });
    }

    void OnBackFromChangeAvatar()
    {
        //Before leaving we update the avatar data to the correct information 
        avatarCustomizationData.SetData(gameData.GetUserInformation().GetAvatarCustomizationData());
        avatarCustomizationController.SetAvatarCustomizationData(avatarCustomizationData.GetSerializeData());

        if (OnChangeAvatarComplete != null)
        {
            OnChangeAvatarComplete();
        }
    }

    public void OnBtnChangeAvatarComplete()
    {
        if (avatarCustomizationData.HasAllItemsOnInventory())
        {
            //loaderPanel.SetActive(true);
            //Send new skin avatar to backend
            JObject json = avatarCustomizationData.GetSerializeDataWebClient();
            Injection.Get<IAvatarEndpoints>().SetAvatarSkin(json).Subscribe(json =>
            {
                //loaderPanel.SetActive(false);
                gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarCustomizationData);
                //After changing the skin on the backend we go to the room
                panelChangeAvatar.OnButtonBack();
            });
        }
        else
        {
            panelChangeAvatar.ShowPanelBuyClothes(avatarCustomizationData);
        }
    }

    public void UpdateAvatarData(AvatarCustomizationSimpleData avatarCustomizationSimpleData)
    {
        if (randomParticles != null)
        {
            randomParticles.Play();
        }
        avatarCustomizationController.SetAvatarCustomizationData(avatarCustomizationSimpleData);
        catalogAvatarScrollView.RefreshCardsSelectionState();
        catalogAvatarColorScrollView.RefreshCardsSelectionState();
    }
}