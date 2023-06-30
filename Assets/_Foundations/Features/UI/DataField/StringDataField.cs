using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;

namespace UI.DataField
{
    public class StringDataField : DataField<string>
    {
        [SerializeField] string title;
        [SerializeField] string placeholderText;
        [Space(10)]
        [SerializeField] UIWidget<string> titleWidget;
        [SerializeField] TMP_InputField field;
        [SerializeField] UIWidget<string> error;
        [SerializeField] CanvasGroup errorGroup;

        void Reset()
        {
            if (titleWidget)
            {
                title = titleWidget.Value;
            }

            if (field && field.placeholder is TextMeshProUGUI phLabel)
            {
                placeholderText = phLabel.text;
            }
        }

        void OnValidate()
        {
            if (titleWidget)
            {
                titleWidget.Value = title;
            }
            if (field && field.placeholder is TextMeshProUGUI phLabel)
            {
                phLabel.text = placeholderText;
            }
        }

        readonly CompositeDisposable disposables = new CompositeDisposable();

        public override string Value
        {
            get => field.text;
            set => field.text = value;
        }

        public override void ShowError(string errorMessage)
        {
            disposables.Clear();
            error.Value = errorMessage;
            ShowErrorRoutine().ToObservable().Subscribe().AddTo(disposables);
            Debug.LogWarning($"<< error displayed >> {errorMessage}",this);
        }

        IEnumerator ShowErrorRoutine()
        {
            errorGroup.alpha = 1;
            yield return new WaitForSecondsRealtime(1);
            var t = 1f;
            while (t > 0)
            {
                errorGroup.alpha = t;
                t -= Time.deltaTime * 2;
                yield return null;
            }
            errorGroup.alpha = 0;
        }
    }
}