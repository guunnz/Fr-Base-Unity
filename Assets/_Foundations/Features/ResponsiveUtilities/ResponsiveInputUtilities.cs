using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ResponsiveUtilities
{
    //[System.Serializable]
    //public class RectTransformToMove
    //{
    //    public RectTransform Rect;
    //    public RectTransform RectEndPos;
    //    internal Vector2 startAnchorMin;
    //    internal Vector2 startAnchorMax;
    //    internal Vector3 startLocalPos;
    //}

    public class ResponsiveInputUtilities : MonoBehaviour
    {
        [SerializeField] TMP_InputField field;
        [SerializeField] RectTransform inputRect;
        [SerializeField] RectTransform endPosition;
        [SerializeField] RectTransform submitButton;
        [SerializeField] RectTransform submitButtonEndPos;
        //[SerializeField] List<RectTransformToMove> RectTransformsToMove = new List<RectTransformToMove>();

        [SerializeField] GameObject form;
        [SerializeField] List<GameObject> objectsToToggle;

        Vector2 fieldStartAnchorMin;
        Vector2 fieldStartAnchorMax;
        Vector3 fieldStartLocalPos;

        Vector2 submitStartAnchorMin;
        Vector2 submitStartAnchorMax;
        Vector3 submitStartLocalPos;

        CompositeDisposable disposables = new CompositeDisposable();

        void OnEnable()
        {
            field.OnSelectAsObservable().Subscribe(_ => RelocateInputField(true)).AddTo(disposables);
            field.onEndEdit.AsObservable().Subscribe(_ => RelocateInputField(false)).AddTo(disposables);

            fieldStartAnchorMax = inputRect.anchorMax;
            fieldStartAnchorMin = inputRect.anchorMin;
            fieldStartLocalPos = inputRect.localPosition;

            if (submitButton != null)
            {
                submitStartAnchorMax = submitButton.anchorMax;
                submitStartAnchorMin = submitButton.anchorMin;
                submitStartLocalPos = submitButton.localPosition;
            }


            //if (RectTransformsToMove.Count > 0)
            //{
            //    foreach (RectTransformToMove rect in RectTransformsToMove)
            //    {
            //        rect.startAnchorMax = rect.Rect.anchorMax;
            //        rect.startAnchorMin = rect.Rect.anchorMin;
            //        rect.startLocalPos = rect.Rect.localPosition;
            //    }
            //}

        }

        public void RelocateInputField(bool moveToEndPos)
        {
            if (moveToEndPos)
            {
                if (endPosition != null)
                {
                    inputRect.anchorMax = endPosition.anchorMax;
                    inputRect.anchorMin = endPosition.anchorMin;
                    inputRect.localPosition = endPosition.localPosition;
                }

                if (submitButton != null)
                {
                    submitButton.anchorMax = submitButtonEndPos.anchorMax;
                    submitButton.anchorMin = submitButtonEndPos.anchorMin;
                    submitButton.localPosition = submitButtonEndPos.localPosition;
                }

                //foreach (RectTransformToMove rect in RectTransformsToMove)
                //{
                //    rect.Rect.anchorMax = rect.RectEndPos.anchorMax;
                //    rect.Rect.anchorMin = rect.RectEndPos.anchorMin;
                //    rect.Rect.localPosition = rect.RectEndPos.localPosition;
                //}

                if (objectsToToggle.Count > 0)
                {
                    foreach (GameObject obj in objectsToToggle)
                    {
                        obj.SetActive(false);
                    }
                }

                if (form != null)
                    form.SetActive(false);
            }
            else
            {
                inputRect.anchorMax.Set(fieldStartAnchorMax.x, fieldStartAnchorMax.y);
                inputRect.anchorMin.Set(fieldStartAnchorMin.x, fieldStartAnchorMin.y);
                inputRect.localPosition = fieldStartLocalPos;

                if (submitButton != null)
                {
                    submitButton.anchorMax.Set(submitStartAnchorMax.x, submitStartAnchorMax.y);
                    submitButton.anchorMin.Set(submitStartAnchorMin.x, submitStartAnchorMin.y);
                    submitButton.localPosition = submitStartLocalPos;
                }

                //foreach (RectTransformToMove rect in RectTransformsToMove)
                //{
                //    rect.Rect.anchorMax.Set(rect.startAnchorMax.x, rect.startAnchorMax.y);
                //    rect.Rect.anchorMin.Set(rect.startAnchorMin.x, rect.startAnchorMin.y);
                //    rect.Rect.localPosition = rect.startLocalPos;
                //}

                if (objectsToToggle.Count > 0)
                {
                    foreach (GameObject obj in objectsToToggle)
                    {
                        obj.SetActive(true);
                    }
                }
                
                if (form != null)
                    form.SetActive(true);
            }
        }

        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}

//Todo: fix the onEndEdit event occluding the OnSubmit event on views
//In order to set this script independent 