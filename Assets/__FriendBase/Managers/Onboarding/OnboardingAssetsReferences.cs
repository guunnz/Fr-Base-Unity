using System.Collections;
using System.Collections.Generic;
using Managers.Avatar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingAssetsReferences : MonoBehaviour
    {
        [SerializeField] public Camera Camera;
        [SerializeField] public Canvas MyCanvas;
        [SerializeField] public GameObject RioBackground;
        [SerializeField] public GameObject ParkBackground;
        [SerializeField] public GameObject PanelBlackScreen;
        [SerializeField] public GameObject People;
        [SerializeField] public AvatarCustomizationController AvatarController;
        [SerializeField] public GameObject AvatarSnapshot;
        [SerializeField] public Button BtnRooms;
        [SerializeField] public Button BtnHome;
        [SerializeField] public Button BtnFurnitures;
        [SerializeField] public Button BtnChat;
        [SerializeField] public OnboardingGenericPopUp PopUpWelcome;
        [SerializeField] public OnboardingGenericPopUp PopUpTapHereToGo;
        [SerializeField] public OnboardingGenericPopUp PopUpWelcomeGift;
        [SerializeField] public OnboardingGenericPopUp PopUpBuyFurnituresAndRoomsOrPets;
        [SerializeField] public OnboardingGenericPopUp PopUpMinigames;
        [SerializeField] public OnboardingGenericPopUp PopUpGoToOtherRooms;
        [SerializeField] public OnboardingProfileCardManager ProfileCardManager;
        [SerializeField] public AvatarCustomizationController AvatarFriendController;
                                
        [SerializeField] public GameObject AvatarFriendController1;
        [SerializeField] public GameObject AvatarFriendController2;
        [SerializeField] public GameObject AvatarFriendController3;
        [SerializeField] public GameObject FriendStartPoint;
        [SerializeField] public GameObject FriendEndPoint;
        [SerializeField] public Animator AvatarAnimator;


        [SerializeField] public GameObject FriendStartPoint1;
        [SerializeField] public GameObject FriendStartPoint2;
        [SerializeField] public GameObject FriendStartPoint3;
        [SerializeField] public GameObject FriendEndPoint1;
        [SerializeField] public GameObject FriendEndPoint2;
        [SerializeField] public GameObject FriendEndPoint3;
        [SerializeField] public GameObject NextButton;


        [SerializeField] public OnboardingGenericPopUp PopUpTapOnFriend;
        [SerializeField] public OnboardingGenericPopUp PopUpOpenPrivateChat;
        [SerializeField] public OnboardingGenericPopUp PopUpTapClosePrivateChat;
        [SerializeField] public OnboardingGenericPopUp PopUpReopenPrivate;
        [SerializeField] public OnboardingGenericPopUp PopUpPublicChat;
        [SerializeField] public OnboardingGenericPopUp PopUpClosePublic;
        [SerializeField] public OnboardingGenericPopUp FriendbaseMaster;
        [SerializeField] public GameObject ScreenTransition;
        [SerializeField] public GameObject PetsText;
        [SerializeField] public Transform moveToGuest;
        [SerializeField] public GameObject YouWereGuest;
        [SerializeField] public Button BtnSkipOnboarding;
        [SerializeField] public TextMeshProUGUI txtSkipOnboarding;
        [SerializeField] public TextMeshProUGUI txtNext;
        [SerializeField] public OnboardingRoomList OnboardingRoomsList;
        [SerializeField] public OnboardingChatPanel OnboardingPrivateChatPanel;
        [SerializeField] public OnboardingChatPanel OnboardingPublicChatPanel;
        [SerializeField] public AvatarNotificationController FriendNotificationController;
    }
}