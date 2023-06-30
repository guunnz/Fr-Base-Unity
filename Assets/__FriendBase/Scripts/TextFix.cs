using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TextFix : MonoBehaviour
{
    public TMP_FontAsset fontForFind;
    public TMP_FontAsset fontForReplace;
    void Start()
    {
        TextMeshProUGUI[] texts = FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];


        texts.Where(x => x.font == fontForFind).ToList().ForEach(x => x.font = fontForReplace);
    }

}
