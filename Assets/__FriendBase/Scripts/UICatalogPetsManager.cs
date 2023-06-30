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
using TMPro;
using System.Linq;
using LocalizationSystem;

[System.Serializable]
public class PetFlowMenu
{
    public UICatalogPetsManager.PetFlow petFlow;
    public List<GameObject> FlowObjects;
}

public class UICatalogPetsManager : MonoBehaviour
{

    [SerializeField] private PetFlow CurrentFlow;
    public enum PetFlow
    {
        None = 0,
        FirstTimePurchase = 1,
        HasPetSelectedOpenFirst = 2,
        HasPetSelectedSelect = 3,
        MissingYourFriend = 4,
        HeyImBack = 5,
        YouHaveANewPet = 6,
        WillJoinYourAvatar = 7,
        ModalMain = 8,
    }


    [SerializeField] private List<PetFlowMenu> petFlowMenu;
    [SerializeField] private UICatalogPetsScrollView catalogPetsScrollView;
    [SerializeField] private PetsBuyManager petsBuyManager;
    public delegate void OnCardSelected(UICatalogPetsCardController card);
    public static event OnCardSelected OnCardSelectedEvent;
    private IGameData gameData = Injection.Get<IGameData>();
    private ILanguage language = Injection.Get<ILanguage>();

    public Load2DObject PostChoosePetImage;

    public GameObject BtnBuy;
    public GameObject BtnChoose;
    public GameObject ModalMain;

    public TextMeshProUGUI TextChoose;
    public TextMeshProUGUI TextChoose2;
    public TextMeshProUGUI TextChoose3;
    public TextMeshProUGUI TextBuy;
    public TextMeshProUGUI MissingYourFriend;
    public TextMeshProUGUI SelectOneBeSide;
    public TextMeshProUGUI HeyImBack;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI NameText2;
    public TextMeshProUGUI NameText3;
    public TextMeshProUGUI YouCanChangeItLater;
    public TextMeshProUGUI YouCanChangeItLater2;
    public TextMeshProUGUI YouCanChangeItLater3;
    public TextMeshProUGUI YourPet;
    public TextMeshProUGUI HidePetText;
    public TextMeshProUGUI WalkOnYourSide;
    public TextMeshProUGUI WalkOnYourSide2;
    public TextMeshProUGUI YouHaveANewPet;
    public TextMeshProUGUI YouHaveANewPet2;
    public TextMeshProUGUI ChooseFirstPet;
    public TextMeshProUGUI SelectOtherPet;

    private GenericCatalogItem LastPetChosen;
    public TMP_InputField PetNameInputField;
    public TextMeshProUGUI PostPurchaseAndNamingText;
    private AvatarPetManager myAvatarPetManager;

    private bool JustPurchased;

    [SerializeField] protected UICatalogPetsScrollView petsScrollView;
    private void Start()
    {
        myAvatarPetManager = CurrentRoom.Instance.AvatarsManager.GetMyAvatar().avatarPetManager;
        if (language == null)
            language = Injection.Get<ILanguage>();

        SetLanguage();
    }

    private void OnEnable()
    {
        Open();
    }

    private void OnDisable()
    {
        if (LastPetChosen != null)
            PlayerPrefs.SetString("Pet" + LastPetChosen.IdItem, PetNameInputField.text);
        Close();
    }
    public void Open()
    {
        StartCoroutine(WaitAndOpen());
    }



    public void SetLanguage()
    {
        language.SetTextByKey(MissingYourFriend, LangKeys.PETS_MISSING_YOUR_FRIEND);
        language.SetTextByKey(SelectOneBeSide, LangKeys.PETS_SELECT_ONE_AND_IT_WILL_BE_BY_YOUR_SIDE);
        language.SetTextByKey(WalkOnYourSide, LangKeys.PETS_YOUR_PET_IS_GOING_TO_WALK_BY_YOUR_SIDE);
        language.SetTextByKey(HeyImBack, LangKeys.PETS_HEY_IM_BACK);
        language.SetTextByKey(NameText, LangKeys.PETS_NAME);
        language.SetTextByKey(NameText2, LangKeys.PETS_NAME);
        language.SetTextByKey(NameText3, LangKeys.PETS_NAME);
        language.SetTextByKey(YouCanChangeItLater, LangKeys.PETS_YOU_CAN_CHANGE_IT_LATER);
        language.SetTextByKey(YouCanChangeItLater2, LangKeys.PETS_YOU_CAN_CHANGE_IT_LATER);
        language.SetTextByKey(YouCanChangeItLater3, LangKeys.PETS_YOU_CAN_CHANGE_IT_LATER);
        language.SetTextByKey(TextBuy, LangKeys.PETS_BUY);
        language.SetTextByKey(YourPet, LangKeys.PETS_YOUR_PET);
        language.SetTextByKey(HidePetText, LangKeys.PETS_HIDE);
        language.SetTextByKey(YouHaveANewPet, LangKeys.PETS_YOU_HAVE_A_NEW_PET);
        language.SetTextByKey(YouHaveANewPet2, LangKeys.PETS_YOU_HAVE_A_NEW_PET);
        language.SetTextByKey(WalkOnYourSide2, LangKeys.PETS_YOUR_PET_IS_GOING_TO_WALK_BY_YOUR_SIDE);
        language.SetTextByKey(SelectOtherPet, LangKeys.PETS_SELECT_OTHER_PET);
        language.SetTextByKey(ChooseFirstPet, LangKeys.PETS_CHOOSE_YOUR_FIRST_PET);
    }

    private void GoToFlow(PetFlow flow)
    {
        if (language == null)
            language = Injection.Get<ILanguage>();

        if (LastPetChosen != null)
            PlayerPrefs.SetString("Pet" + LastPetChosen.IdItem, PetNameInputField.text);
        CurrentFlow = flow;
        petFlowMenu.ForEach(x => x.FlowObjects.ForEach(y => y.SetActive(false)));
        if (flow == PetFlow.ModalMain)
        {
            ModalMain.SetActive(false);
            return;
        }

        petFlowMenu.Single(x => x.petFlow == CurrentFlow).FlowObjects.ForEach(y => y.SetActive(true));

        if (flow == PetFlow.HasPetSelectedSelect)
        {
            language.SetTextByKey(TextChoose, LangKeys.PETS_CHOOSE_PET);
            language.SetTextByKey(TextChoose2, LangKeys.PETS_CHOOSE_PET);
            language.SetTextByKey(TextChoose3, LangKeys.PETS_CHOOSE_PET);
        }
        else if (flow == PetFlow.FirstTimePurchase)
        {
            language.SetTextByKey(TextChoose, LangKeys.PETS_SAVE);
            language.SetTextByKey(TextChoose2, LangKeys.PETS_SAVE);
            language.SetTextByKey(TextChoose3, LangKeys.PETS_SAVE);
        }
        else
        {
            language.SetTextByKey(TextChoose, LangKeys.PETS_CHOOSE_PET);
            language.SetTextByKey(TextChoose2, LangKeys.PETS_CHOOSE_PET);
            language.SetTextByKey(TextChoose3, LangKeys.PETS_CHOOSE_PET);
        }
    }

    IEnumerator WaitAndOpen()
    {
        yield return new WaitForEndOfFrame();

        if (myAvatarPetManager.CurrentPetId > 0)
        {
            string PetName = PlayerPrefs.GetString("Pet" + myAvatarPetManager.CurrentPetId);
            if (!string.IsNullOrEmpty(PetName))
            {
                PetNameInputField.text = PetName.Replace("_", " ");
            }
            GoToFlow(PetFlow.HasPetSelectedOpenFirst);
            PostChoosePetImage.Load(myAvatarPetManager.PetName + "_Profile");
        }
        else
        {
            if (myAvatarPetManager.HasAnyPet())
            {
                GoToFlow(PetFlow.MissingYourFriend);
            }
            else
            {
                GoToFlow(PetFlow.FirstTimePurchase);
            }
        }
        catalogPetsScrollView.OnCardSelected += OnPetCardSelected;
    }

    public void Close()
    {
        catalogPetsScrollView.OnCardSelected -= OnPetCardSelected;
    }

    void OnPetCardSelected(GenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        UICatalogPetsCardController card = cardController as UICatalogPetsCardController;

        if (!card.IsCardSelected)
        {
            card.IsCardSelected = true;
            LastPetChosen = null;
            BtnBuy.SetActive(false);
            BtnChoose.SetActive(false);
            petsBuyManager.SetPetToBuy(null);
            LastPetChosen = catalogItem;
            if (card.Obtained)
            {
                SetChoice();
            }
            else
            {
                SetBuy();
            }
        }
        else
        {
            LastPetChosen = catalogItem;
            if (card.Obtained)
            {
                SetChoice();
            }
            else
            {
                SetBuy();
            }
        }
    }

    private void SetChoice()
    {
        BtnBuy.SetActive(false);
        BtnChoose.SetActive(true);
    }

    private void SetBuy()
    {
        BtnChoose.SetActive(false);
        BtnBuy.SetActive(true);
        petsBuyManager.SetPetToBuy(LastPetChosen);
    }

    public void SetChooseFlow()
    {
        GoToFlow(PetFlow.HasPetSelectedSelect);
    }

    public void ChoosePet(GenericCatalogItem item, bool afterPurchase = false)
    {
        LastPetChosen = item;

        string PetName = PlayerPrefs.GetString("Pet" + LastPetChosen.IdItem);

        if (!string.IsNullOrEmpty(PetName))
        {
            PetNameInputField.text = PetName.Replace("_", " ");
        }
        else
        {
            PetNameInputField.text = item.NamePrefab.Replace("_", " ");
        }

        if (myAvatarPetManager.CurrentPetId == item.IdItem)
        {
            GoToFlow(PetFlow.HasPetSelectedOpenFirst);
        }
        else if (afterPurchase)
        {
            GoToFlow(PetFlow.YouHaveANewPet);
        }
        else
        {
            GoToFlow(PetFlow.HeyImBack);
        }

        PostChoosePetImage.Load(item.NamePrefab + "_Profile");
        JustPurchased = afterPurchase;
        myAvatarPetManager.SetPet(item.IdItemWebClient, item.IdItem, PetPrefabName: item.NamePrefab);
    }

    public void SetName()
    {
        PlayerPrefs.SetString("Pet" + LastPetChosen.IdItem, PetNameInputField.text);
        if (JustPurchased)
        {
            GoToFlow(PetFlow.WillJoinYourAvatar);

            PostPurchaseAndNamingText.text = string.Format(language.GetTextByKey(LangKeys.PETS_AWESOME_NAME), PetNameInputField.text);
        }
        else
        {
            GoToFlow(PetFlow.ModalMain);
        }
    }

    public void HidePet()
    {
        myAvatarPetManager.SetPetNull();

        GoToFlow(PetFlow.ModalMain);
    }

    public void Choose()
    {
        ChoosePet(LastPetChosen, false);
    }

    public void Buy()
    {
        petsBuyManager.OnButtonBuy();
    }
}