using Architecture.Injector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    public string Scene;
    public string SceneUnload;
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(LoadScene);
    }

    public void LoadScene()
    {
        Injection.Get<ILoading>().Load();
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        SceneManager.LoadScene(Scene, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(SceneUnload);
    }
}