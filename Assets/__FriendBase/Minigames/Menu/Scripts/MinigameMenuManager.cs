using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using LocalizationSystem;
using UnityEngine.EventSystems;

public class MinigameMenuManager : MonoBehaviour
{
    [SerializeField] Transform minigamesParent;
    [SerializeField] Transform minigamesMenu;
    [SerializeField] GameObject minigameMenuPrefab;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI loadingText;


    float unitsToMoveSelectingGame = 620;

    private ILoader loader;
    private IGameData gameData;
    private Game currentGameSelected = Game.Racing;
    private Dictionary<Game, MinigameInformation> miniGamesDictionary;
    private MinigameInputManager InputMinigame;
    [HideInInspector] public UnityEvent SelectEvent;
    [SerializeField] MinigameManager minigameManager;
    [SerializeField] GameObject menuContainer;
    private float inputDelay = 0.4f;
    private float inputDelayAux = 0.4f;
    private List<GameObject> InstantiatedMinigames = new List<GameObject>();
    private bool inMenu = true;
    private ILanguage language;

    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float acceleration = 0;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI ChooseAGameText;

    //[SerializeField] TextMeshProUGUI MoreGamesComingSoon;
    //[SerializeField] TextMeshProUGUI PlayText;
    private void Start()
    {
        StartCoroutine(InitializeCoroutine());
        SetLanguageKeys();
        Injection.Get<ILoading>().Unload();
    }


    public void SetLanguageKeys()
    {
        language = Injection.Get<ILanguage>();

        //language.SetCurrentLanguage(LanguageType.PORTUGUESE);
        //txtTabLeft.text = "Places";
        //txtTabRight.text = "Events";

        //language.SetTextByKey(PlayText, LangKeys.MINIGAME_PLAY);
        //language.SetTextByKey(MoreGamesComingSoon, LangKeys.MINIGAME_MORE_GAMES_COMING_SOON);
        language.SetTextByKey(ChooseAGameText, LangKeys.MINIGAME_CHOOSE_A_GAME);
        language.SetText(loadingText, language.GetTextByKey(LangKeys.Loading) + "...");
    }

    public void Initialize()
    {
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        inMenu = true;
        InputMinigame = MinigameInputManager.Singleton;
        gameData = Injection.Get<IGameData>();
        SetCoins();

        loader = Injection.Get<ILoader>();
        //Wait Loader to Load
        while (loader == null)
        {
            yield return null;
        }
        //
        miniGamesDictionary = gameData.GetAllMinigamesForMenu();

        foreach (var item in miniGamesDictionary)
        {
            MinigameMenuItem minigame = Instantiate(minigameMenuPrefab, minigamesParent).GetComponent<MinigameMenuItem>();

            InstantiatedMinigames.Add(minigame.gameObject);
            StartCoroutine(minigame.Initialize(item.Value, this));
        }

    }

    public Game GetCurrentMinigameSelected()
    {
        return currentGameSelected;
    }

    private void Update()
    {
        if (inMenu)
        {
            if (inputDelay <= 0)
            {
                float x = InputMinigame.GetHorizontal();
                if (x != 0)
                {
                    ChangeMinigame(x);
                }
            }
            else
            {
                inputDelay -= Time.deltaTime;
            }

            if (InputMinigame.GetPrimaryButton() && currentGameSelected != Game.ComingSoon)
            {
                Select(miniGamesDictionary.Single(x => x.Key == currentGameSelected).Value);
            }


            if (acceleration > 0.3f)
            {
                if (inputDelay <= 0)
                {
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {
                        if ((lp.x > fp.x))  //If the movement was to the right
                        {
                            Debug.LogError("right");
                            ChangeMinigame(-1);

                        }
                        else
                        {
                            Debug.LogError("left");
                            ChangeMinigame(1);
                        }
                    }
                }
            }


            if (Input.touchCount == 1) // user is touching the screen with a single touch
            {
                if (InputMinigame.AnyButton())
                    return;
                Debug.Log(acceleration);
                Touch touch = Input.GetTouch(0); // get the touch
                if (touch.phase == TouchPhase.Began) //check for the first touch
                {
                    acceleration = 0;
                    fp = touch.position;
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
                {
                    lp = touch.position;
                    if (acceleration > 1)
                    {
                        acceleration = 1;
                    }
                    else
                    {
                        acceleration += Time.deltaTime * 2;
                    }

                }
                else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
                {
                    lp = touch.position;  //last touch position. Ommitted if you use list
                }
            }
            else
            {
                if (acceleration <= 0)
                {
                    acceleration = 0;
                }
                else
                {
                    acceleration -= Time.deltaTime;
                }
            }

        }
    }


    // Update is called once per frame






    private void SetCoins()
    {
        coinsText.text = gameData.GetUserInformation().Gold.ToString(); //ShouldGetFromEndpoint
    }

    public void ChangeMinigame(float x)
    {
        if (inputDelay >= 0)
            return;

        inputDelay = inputDelayAux;
        if (x < 0)
        {
            if (currentGameSelected == (Game)1)
            {
                return;
            }
            minigamesMenu.DOLocalMove(new Vector3(minigamesParent.localPosition.x + unitsToMoveSelectingGame, minigamesParent.position.y, minigamesParent.position.z), inputDelayAux / 2);
            currentGameSelected--;
        }
        else
        {
            if (currentGameSelected == Game.ComingSoon)
            {
                return;
            }
            minigamesMenu.DOLocalMove(new Vector3(minigamesParent.localPosition.x - unitsToMoveSelectingGame, minigamesParent.position.y, minigamesParent.position.z), inputDelayAux / 2);
            currentGameSelected++;
        }
        SelectEvent.Invoke();
    }

    public void ToggleMenu(bool enabled)
    {
        inMenu = enabled;
        menuContainer.SetActive(enabled);
        if (!enabled)
        {
            SelectEvent.RemoveAllListeners();
            InstantiatedMinigames.ForEach(minigame => Destroy(minigame));
        }
    }

    public void Select(MinigameInformation info)
    {
        ToggleMenu(false);
        minigameManager.LoadGame(info);
    }
}