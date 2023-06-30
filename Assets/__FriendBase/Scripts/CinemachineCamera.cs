using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CinemachineCamera : MonoBehaviour
{
    private CinemachineBrain cameraBrain;

    private Camera mainCam;

    private static CinemachineCamera _singleton;

    public static CinemachineCamera Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(CinemachineCamera)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] GameObject chat;
    [SerializeField] RectTransform chatArea;
    [SerializeField] Canvas chatCanvas;

    private float chatCameraXOffset;
    private const float SafeFloatOffsetAdjusts = 0.01f;

    private float notchXLeftMultiplier;
    private float notchXRightMultiplier;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
        Singleton = this;
        cameraBrain = GetComponent<CinemachineBrain>();

        chatCameraXOffset = -(chatArea.rect.width * chatCanvas.scaleFactor / Screen.width) + SafeFloatOffsetAdjusts;

        notchXLeftMultiplier = ((Screen.safeArea.xMin / Screen.safeArea.width) * 1.25f) + 1;
        notchXRightMultiplier = ((Screen.currentResolution.width / Screen.safeArea.xMax - 1) * 1.1f) + 1;
    }

    public float GetNotchXLeftMultiplier()
    {
        return notchXLeftMultiplier;
    }

    public float GetNotchXRightMultiplier()
    {
        return notchXRightMultiplier;
    }


    public bool isChatOpen()
    {
        if (chat == null)
        {
            return false;
        }

        return chat.activeSelf;
    }

    public void SetCameraChatMode()
    {
        StartCoroutine(SetCameraChatModeCoroutine());
    }

    private IEnumerator SetCameraChatModeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        mainCam.rect = new Rect(chatCameraXOffset, 0, 1, 1);
    }

    private IEnumerator SetCameraNormalModeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        mainCam.rect = new Rect(0f, 0, 1, 1);
    }

    public void SetCameraNormalMode()
    {
        StartCoroutine(SetCameraNormalModeCoroutine());
    }

    public CinemachineBrain GetBrain()
    {
        return cameraBrain;
    }
}