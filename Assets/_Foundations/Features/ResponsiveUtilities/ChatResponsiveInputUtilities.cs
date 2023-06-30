using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ResponsiveUtilities
{
    public class ChatResponsiveInputUtilities : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] RectTransform inputRect;
        [SerializeField] RectTransform endPosition;
        [SerializeField] List<GameObject> switchVisibility;
        private TMP_InputField field;

        Vector2 fieldStartAnchorMin;
        Vector2 fieldStartAnchorMax;
        Vector3 fieldStartLocalPos;

        bool isClickOverUI;
        private bool isOnEndPos;

        private void Start()
        {
            field = this.GetComponent<TMP_InputField>();
        }


        private void Update()
        {
            ControlInputUpdate();
        }

        void ControlInputUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isClickOverUI = IsPointerOverGameObject();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!isClickOverUI)
                {
                    RelocateInputField(false);
                }

                isClickOverUI = false;
            }
        }

        bool IsPointerOverGameObject()
        {
            //check mouse
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            // check touch
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    return true;
                }
            }

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RelocateInputField(true);
        }

        public void RelocateInputField(bool moveToEndPos)
        {
            switch (moveToEndPos)
            {
                case true when !isOnEndPos:
                    {
                        field.ActivateInputField();
                        isOnEndPos = true;
                        inputRect.anchorMax = endPosition.anchorMax;
                        inputRect.anchorMin = endPosition.anchorMin;
                        inputRect.localPosition = endPosition.localPosition;

                        foreach (var go in switchVisibility)
                        {
                            go.SetActive(!go.activeInHierarchy);
                        }

                        break;
                    }
                case false when isOnEndPos:
                    {
                        isOnEndPos = false;
                        inputRect.anchorMax.Set(fieldStartAnchorMax.x, fieldStartAnchorMax.y);
                        inputRect.anchorMin.Set(fieldStartAnchorMin.x, fieldStartAnchorMin.y);
                        inputRect.localPosition = fieldStartLocalPos;

                        foreach (var go in switchVisibility)
                        {
                            go.SetActive(!go.activeInHierarchy);
                        }

                        break;
                    }
            }
        }

        protected void OnEnable()
        {
            fieldStartAnchorMax = inputRect.anchorMax;
            fieldStartAnchorMin = inputRect.anchorMin;
            fieldStartLocalPos = inputRect.localPosition;
        }

        protected void OnDisable()
        {
            RelocateInputField(false);
        }
    }
}