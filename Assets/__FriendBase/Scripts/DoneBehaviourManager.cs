using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TextField
{
    public bool shouldSubmit;
    public TMPro.TMP_InputField inputField;
    [Header("Only If Should Submit is false")]
    public TMPro.TMP_InputField nextInputField;
    public ResponsiveUtilities.ResponsiveInputUtilities nextResponsiveInput;
}

public class DoneBehaviourManager : MonoBehaviour
{
    [SerializeField] List<TextField> TextFields = new List<TextField>();

    [SerializeField] Button submitButton;

    private bool cannotGoNext;

    private void OnEnable()
    {
        foreach (TextField textField in TextFields)
        {
            TMPro.TMP_InputField inputField = textField.inputField;
            if (textField.shouldSubmit)
            {
                inputField.onSubmit.AddListener(delegate { Submit(); });
            }
            else
            {
                inputField.onSubmit.AddListener(delegate { GoToNextText(textField); });
            }
        }
    }

    private void OnDisable()
    {
        foreach (TextField textField in TextFields)
        {
            TMPro.TMP_InputField inputField = textField.inputField;

            inputField.onSubmit.RemoveAllListeners();

        }
    }

    private void OnDestroy()
    {
        foreach (TextField textField in TextFields)
        {
            TMPro.TMP_InputField inputField = textField.inputField;

            inputField.onSubmit.RemoveAllListeners();

        }
    }


    void GoToNextText(TextField textField)
    {
        if (!textField.inputField.gameObject.activeSelf)
        {
            return;
        }
        if (textField.nextResponsiveInput != null)
            textField.nextResponsiveInput.RelocateInputField(true);

        textField.nextInputField.ActivateInputField();
    }

    void Submit()
    {
        StartCoroutine(SubmitCoroutine());
    }

    public IEnumerator SubmitCoroutine()
    {
        yield return null;
        if (submitButton != null)
            submitButton.onClick.Invoke();
    }
}