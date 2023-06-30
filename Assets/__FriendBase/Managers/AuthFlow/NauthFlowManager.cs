using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using DebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NauthFlowManager : MonoBehaviour
{
    [SerializeField] private List<AbstractAuthFlowScreen> listScreens;

    public enum STATE_AUTH_FLOW { FIRST_TIME, DEFAULT_SCREEN, LINK_PROVIDER }

    private AbstractAuthFlowScreen currentScreen;
    private ILoading loading;
    private IDebugConsole debugConsole;
    private NauthFlowFirebaseInitialiization firebaseInitialization;

    private static STATE_AUTH_FLOW stateAuthFlow;

    public static void SetStateAuthFlow(STATE_AUTH_FLOW state)
    {
        stateAuthFlow = state;
    }

    void Start()
    {
        loading = Injection.Get<ILoading>();
        SetLoadingState(true);

        firebaseInitialization = new NauthFlowFirebaseInitialiization(this);
        firebaseInitialization.OnFirebaseInitializationComplete += OnFirebaseInitializationComplete;
        firebaseInitialization.Init();
    }

    void OnDestroy()
    {
        firebaseInitialization.Destroy();
        firebaseInitialization.OnFirebaseInitializationComplete -= OnFirebaseInitializationComplete;
    }

    void OnFirebaseInitializationComplete(NauthFlowFirebaseInitialiization.FirebaseCompleteStatus status)
    {
        switch (status)
        {
            case NauthFlowFirebaseInitialiization.FirebaseCompleteStatus.UPDATE_GAME:
                SceneManager.UnloadSceneAsync(GameScenes.NewAuthFlow);
                SceneManager.LoadSceneAsync(GameScenes.UpdateGame, LoadSceneMode.Additive);
                break;
            case NauthFlowFirebaseInitialiization.FirebaseCompleteStatus.SUCCEED:
                StartCoroutine(Init());
                break;
        }
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        switch (stateAuthFlow)
        {
            case STATE_AUTH_FLOW.FIRST_TIME:
                GoScreen(NauthFlowScreenType.LOGIN);
                SetStateAuthFlow(STATE_AUTH_FLOW.DEFAULT_SCREEN);
                break;
            case STATE_AUTH_FLOW.DEFAULT_SCREEN:
                GoScreen(NauthFlowScreenType.LOGIN);
                break;
            case STATE_AUTH_FLOW.LINK_PROVIDER:
                GoScreen(NauthFlowScreenType.LINK_PROVIDER);
                SetStateAuthFlow(STATE_AUTH_FLOW.DEFAULT_SCREEN);
                break;
        }
    }

    public void GoScreen(NauthFlowScreenType screenType)
    {
        if (currentScreen!=null)
        {
            currentScreen.Close();
        }
        currentScreen = GetScreenByType(screenType);
        currentScreen.Open(this);
    }

    AbstractAuthFlowScreen GetScreenByType(NauthFlowScreenType screenType)
    {
        foreach (AbstractAuthFlowScreen screen in listScreens)
        {
            if (screen.ScreenType == screenType)
            {
                return screen;
            }
        }
        return null;
    }

    public void SetLoadingState(bool isLoading)
    {
        if (isLoading)
        {
            loading.Load();
        }
        else
        {
            loading.Unload();
        }
    }
}


/*
 * -LoginTracking() Agregar
 * -TrackUserSession Ver, ahora esta en GetInitialAvatarEndpoints()
 * -SetCurrentLanguage from server
 * 
 */


/*
 * 
 * - Check DeepLinks
 * - Check Banned Users
 * 

 * 
 * DONE
 * 
 * Agregar textos missing
 * Sacar Italic de Input Field
 * Cambiar label rojo por naranje
 * Terms and conditions
 * Check if username exists
 * Initial decision when connect on automatic login
 * 
 * - Alargar textfield en login buttons
 * - Ver tema loading al autologin
 * - No poder seleccionar fecha cuando gira
 * 
 * - si me quiero loguear con una cuenta inexistente me crea un guest user OJO -Screen You need to create an account
 * - Control Up/Down when Keyboard open/close
 * 
 * TO DO
 * 
 * 
 * - Ver que pasa si me desconecto en Enter Nick
 * - Msg de error para link guest account si ya se usa esa cuenta
 * - Errors Providers
 * 
 *  * - si me quiero registrar con una cuenta existente en vez de loguearme deberia mostrar cartel
 *  
 * - Integrate with Tomy
 * - Ver de no pedir Token en cada Endpoint
 */