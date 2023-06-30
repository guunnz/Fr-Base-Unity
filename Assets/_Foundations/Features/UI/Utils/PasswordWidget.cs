using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Utils
{
    public class PasswordWidget : MonoBehaviour
    {
        public Sprite hiddenSprite;
        public Sprite shownSprite;
        public Image showHideImage;


        public TMP_InputField field;
        public Button showHide;
        public bool isHidden = true;


        private void Start()
        {
            UpdateShowHide(isHidden);
            showHide.onClick.AddListener(OnSwitchShowHide);
        }

        private void OnSwitchShowHide()
        {
            isHidden = !isHidden;
            UpdateShowHide(isHidden);
        }

        private void UpdateShowHide(bool hidden)
        {
            showHideImage.sprite = hidden ? hiddenSprite : shownSprite;
            field.contentType = hidden ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            field.textComponent.SetAllDirty();
        }

        public void ClearField()
        {
            field.text = "";
        }
    }
}