using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using LocalizationSystem;
using Socket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameInvitation : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI InvitedToPlayText;

    [SerializeField] TextMeshProUGUI AcceptText;
    [SerializeField] TextMeshProUGUI DeclineText;

    [SerializeField] GameObject container;
    [SerializeField] GameObject loading;
    [SerializeField] Image gameImage;

    private IncomingEventMinigameInvite LastMinigameInvite;

    IAvatarEndpoints AvatarEndpoints;
    IGameData gameData;
    ILanguage language;
    private ILoader loader;

    private Game MinigameSelected;

    public bool canBeInvited = true;

    public static MinigameInvitation instance;

    public IEnumerator SetGameImage(string miniGameName)
    {
        gameImage.enabled = false;
        loader = Injection.Get<ILoader>();

        loader.LoadItem(new LoaderItemSprite(miniGameName + "_Sprite_Unselected"));

        LoaderAbstractItem spriteAddressables = loader.GetItem(miniGameName + "_Sprite_Unselected");

        while (spriteAddressables == null || spriteAddressables.State != LoaderItemState.SUCCEED)
        {
            yield return null;
        }
        LoaderItemSprite gameSpriteAux = spriteAddressables as LoaderItemSprite;
        Sprite gameSprite = gameSpriteAux.GetSprite();
    }

    private AbstractIncomingSocketEvent MatchFoundEvent;

    private bool DoAfterAccept;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MINIGAME_INVITE, GotInvitedToMinigame);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MATCH_EVENT, Match);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.MINIGAME_INVITE, GotInvitedToMinigame);
    }

    private void Match(AbstractIncomingSocketEvent incomingEvent)
    {
        MatchFoundEvent = incomingEvent;
    }

    private void GotInvitedToMinigame(AbstractIncomingSocketEvent incomingEvent)
    {
       
        language = Injection.Get<ILanguage>();
        IncomingEventMinigameInvite incomingEventMinigameInvite = incomingEvent as IncomingEventMinigameInvite;
        gameData = Injection.Get<IGameData>();

        if (incomingEventMinigameInvite.user_id == gameData.GetUserInformation().UserId.ToString())
        {
            return;
        }

        if (!canBeInvited)
            return;

        LastMinigameInvite = incomingEventMinigameInvite;
        string friendName = gameData.GetFriendList().Single(x => x.userID.ToString() == incomingEventMinigameInvite.user_id).username;

        InvitedToPlayText.text = language.GetTextByKey(LangKeys.RACING_HAS_INVITED_YOU_TO_PLAY_USERNAME).Replace("[username]", "<color=#7A1602>" + friendName + "</color>");
        AcceptText.text = language.GetTextByKey(LangKeys.RACING_LETS_PLAY);
        DeclineText.text = language.GetTextByKey(LangKeys.RACING_REJECT);

        SetGameImage("Racing");
        //When game name is received do this with the name received and convert to enum
        switch ("Racing")
        {
            case "Racing":
                MinigameSelected = Game.Racing;
                break;
        }

        container.SetActive(true);
    }

    public void Reject()
    {
        if (AvatarEndpoints == null)
            AvatarEndpoints = Injection.Get<IAvatarEndpoints>();
        AvatarEndpoints.GameFriendInviteInteractAsync(LastMinigameInvite.gameInvitationId, false);
        container.SetActive(false);
    }

    public void Accept()
    {
        if (AvatarEndpoints == null)
            AvatarEndpoints = Injection.Get<IAvatarEndpoints>();

        AvatarEndpoints.GameFriendInviteInteractAsync(LastMinigameInvite.gameInvitationId, true);
        DoAfterAccept = true;

        container.SetActive(false);
        if (SceneManager.GetSceneByName("Minigames").isLoaded == false)
        {
            CurrentRoom.Instance.GoToMinigames();
        }
        else
        {
            StartCoroutine(IAfterAccept());
        }
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (DoAfterAccept)
        {
            StartCoroutine(IAfterAccept());
        }
        else
        {
            MinigameInvitation.instance.canBeInvited = true;
        }
    }

    IEnumerator IAfterAccept()
    {
        loading.SetActive(true);
        DoAfterAccept = false;
        MinigameMenuItem minigameMenu = FindObjectsOfType<MinigameMenuItem>().FirstOrDefault(x => x.GetIdGame() == MinigameSelected);
        MinigameMultiplayerManager minigameMultiplayerManager = FindObjectOfType<MinigameMultiplayerManager>();

        while (minigameMenu == null)
        {
            if (minigameMultiplayerManager != null)
                break;

            minigameMenu = FindObjectsOfType<MinigameMenuItem>().FirstOrDefault(x => x.GetIdGame() == MinigameSelected);
            yield return null;
        }


        if (minigameMultiplayerManager == null)
        {
            yield return new WaitForSeconds(0.1f);
            minigameMenu.SelectDirectly();
        }

        while (minigameMultiplayerManager == null)
        {
            minigameMultiplayerManager = FindObjectOfType<MinigameMultiplayerManager>();
            yield return null;
        }

        while (MatchFoundEvent == null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        minigameMultiplayerManager.FromMultiplayerInvite(MatchFoundEvent);
        yield return new WaitForSeconds(0.2f);
        loading.SetActive(false);
    }
}