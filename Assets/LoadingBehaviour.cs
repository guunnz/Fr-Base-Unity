using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Architecture.Injector.Core;
using UnityEngine.SceneManagement;
using LocalizationSystem;
using JetBrains.Annotations;

public class LoadingBehaviour : MonoBehaviour, ILoading
{
    private List<string> TipsList = new List<string>();
    public List<Sprite> PicturesList;
    public Image BlackBgImage;
    public Image Image1;
    public Image Image2;
    public Image FriendbaseLogo;
    public Image Gradient;
    private bool DoFadeToFirstImage;
    public TextMeshProUGUI tipsText;
    public TextMeshProUGUI loadingText;
    public GameObject Container;
    IAnalyticsSender analyticsSender;
    public float TimePerFade;
    public float FadeDuration;
    public bool Test;
    public bool firstTimeDone = false;
    private static LoadingBehaviour instance;

    private Coroutine CfadeLoad;
    private Coroutine CloadingText;

    private bool loading;
    private bool sceneChanged;
    int lastFirstPic;
    int lastSecondPic;

    private ILanguage language;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        analyticsSender = Injection.Get<IAnalyticsSender>();

        SceneManager.sceneLoaded += RegisterInjection;

        RegisterInjection();
        PopulateTipsList();

        DontDestroyOnLoad(this.gameObject);
        if (Test)
            Load();
        language = Injection.Get<ILanguage>();
    }

    void RegisterInjection()
    {
        Injection.Register<ILoading>(instance);
    }

    void RegisterInjection(Scene activeScene, LoadSceneMode loadMode)
    {
        if (Injection.Get<ILoading>() == null)
            Injection.Register<ILoading>(instance);
    }

    public bool isloading()
    {
        return loading;
    }

    public void PopulateTipsList()
    {
        TipsList.Add(LangKeys.Tip1);
        TipsList.Add(LangKeys.Tip2);
        TipsList.Add(LangKeys.Tip3);
        TipsList.Add(LangKeys.Tip4);
        TipsList.Add(LangKeys.Tip5);
        TipsList.Add(LangKeys.Tip6);
    }

    private void OnApplicationFocus(bool focus)
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
        if (loading)
        {
            if (!focus && analyticsSender != null)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.MinimizeDuringLoading);
            }
        }
    }

    public void OnApplicationQuit()
    {
        if (loading)
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.CloseDuringLoading);
        }
    }

    public void Load(bool fullAlpha = true)
    {
        if (language == null)
        {
            Debug.Log("Had to load language");
            language = Injection.Get<ILanguage>();
        }
        if (loading)
            return;

        Container.SetActive(true);
        BlackBgImage.gameObject.SetActive(true);

        if (fullAlpha)
        {
            Image1.DOFade(1, 0f);
            Gradient.DOFade(1, 0f);
            tipsText.DOFade(1, 0f);
            loadingText.DOFade(1, 0f);
            FriendbaseLogo.DOFade(1, 0f);
        }
        else
        {
            Image1.DOFade(1, 1f);
            tipsText.DOFade(1, 1f);
            loadingText.DOFade(1, 1f);
            FriendbaseLogo.DOFade(1, 1f);
            Gradient.DOFade(1, 1f);
        }

        CfadeLoad = StartCoroutine(FadeLoad());
        CloadingText = StartCoroutine(LoadingText());
    }

    public void Unload()
    {
        if (!loading)
            return;

        StartCoroutine(IUnload());
    }

    private IEnumerator IUnload()
    {
        BlackBgImage.gameObject.SetActive(false);
        StopCoroutine(CfadeLoad);
        StopCoroutine(CloadingText);
        yield return new WaitForSeconds(0.6f);
        loading = false;
        Image1.DOFade(0, 1f);
        Image2.DOFade(0, 1f);
        tipsText.DOFade(0, 1f);
        loadingText.DOFade(0, 1f);
        FriendbaseLogo.DOFade(0, 1f);
        Gradient.DOFade(0, 1f);
        yield return new WaitForSeconds(1.2f);
        Container.SetActive(false);
        firstTimeDone = false;
        Image1.DOFade(0, 0f);
        Image2.DOFade(0, 0f);
        tipsText.DOFade(0, 0f);
        loadingText.DOFade(0, 0f);
        FriendbaseLogo.DOFade(0, 0f);
        Gradient.DOFade(0, 0f);

    }
    public IEnumerator LoadingText()
    {
        int index = 3;
        while (true)
        {
            loadingText.text = language.GetTextByKey(LangKeys.Loading);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < index; i++)
            {
                loadingText.text += ".";
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public IEnumerator FadeLoad()
    {
        loading = true;
        lastFirstPic = Random.Range(0, PicturesList.Count);
        lastSecondPic = Random.Range(0, PicturesList.Count);
        tipsText.text = language.GetTextByKey(TipsList[Random.Range(0, TipsList.Count)]);
        while (true)
        {

            int firstPic = Random.Range(0, PicturesList.Count);
            int secondPic = Random.Range(0, PicturesList.Count);



            Sprite sprite1 = PicturesList[firstPic];
            Sprite sprite2 = PicturesList[secondPic];

            while (firstPic == lastSecondPic)
            {
                firstPic = Random.Range(0, PicturesList.Count);
            }

            while (secondPic == lastFirstPic)
            {
                secondPic = Random.Range(0, PicturesList.Count);
            }

            while (secondPic == firstPic)
            {
                firstPic = Random.Range(0, PicturesList.Count);
                secondPic = Random.Range(0, PicturesList.Count);
            }


            lastFirstPic = firstPic;
            lastSecondPic = secondPic;


            sprite2 = PicturesList[secondPic];
            sprite1 = PicturesList[firstPic];

            if (firstTimeDone)
            {
                if (DoFadeToFirstImage)
                {
                    Image1.sprite = sprite1;
                }
                else
                {
                    Image2.sprite = sprite2;
                }
            }
            else
            {
                firstTimeDone = true;
                Image1.sprite = sprite1;
                Image2.sprite = sprite2;
                yield return new WaitForSeconds(FadeDuration);
            }


            tipsText.DOFade(0, TimePerFade);
            if (DoFadeToFirstImage)
            {
                Image1.DOFade(1, TimePerFade);
                Image2.DOFade(0, TimePerFade);
            }
            else
            {
                Image1.DOFade(0, TimePerFade);
                Image2.DOFade(1, TimePerFade);
            }

            DoFadeToFirstImage = !DoFadeToFirstImage;

            yield return new WaitForSeconds(TimePerFade / 2);


            tipsText.text = language.GetTextByKey(TipsList[Random.Range(0, TipsList.Count)]);
            tipsText.DOFade(1, TimePerFade / 2);

            yield return new WaitForSeconds(FadeDuration - TimePerFade / 2);
        }
    }
}
