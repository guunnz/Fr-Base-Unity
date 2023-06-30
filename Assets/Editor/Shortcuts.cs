using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Shortcuts
{
    [MenuItem(Const.GameNameMenu + "Play Form Main Scene #P")]
    public static void PlayFromMainScene()
    {
        EditorSceneManager.OpenScene("Assets/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem(Const.GameNameMenu + "Go World Scene #W")]
    public static void GoWorldScene()
    {
        if (EditorApplication.isPlaying) return;
        EditorSceneManager.OpenScene("Assets/GameplayScene.unity");
        
    }
}