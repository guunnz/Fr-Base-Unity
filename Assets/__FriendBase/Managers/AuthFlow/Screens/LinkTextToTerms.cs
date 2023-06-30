using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinkTextToTerms : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI txtTitle;
    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(txtTitle, Input.mousePosition, null);

        if (linkIndex==-1)
        {
            return;
        }

        var linkId = txtTitle.textInfo.linkInfo[linkIndex].GetLinkID();
        if (linkId.Equals("ID_TERMS"))
        {
            Application.OpenURL("https://friendbase.com/terms-of-use-cookie-policy-app/");
        }
    }
}
