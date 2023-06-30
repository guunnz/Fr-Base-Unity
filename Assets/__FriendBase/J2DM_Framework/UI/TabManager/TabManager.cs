using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace UI.TabController
{
    public class TabManager : MonoBehaviour
    {
        [SerializeField] private List<TabController> listTabController;
        [SerializeField] private float scaleFactor;
        [SerializeField] private float timeScaleFactor = 0.5f;
        [SerializeField] private bool canDeselect = true;

        public delegate void TabSelected(int index);
        public event TabSelected OnTabSelected;

        public delegate void UnselectTabSelected(int index);
        public event UnselectTabSelected OnUnselectTabSelected;

        private float _alphaDisable = 0.5f;
        public float alphaDisable
        {
            get { return _alphaDisable; }
            set { _alphaDisable = value; }
        }

        protected int _indexSelected = -1;
        public int indexSelected
        {
            get { return _indexSelected; }
        }

        protected virtual void Start()
        {
            if (listTabController == null)
            {
                return;
            }

            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                int temp = i;
                listTabController[i].GetComponent<Button>().onClick.AddListener(() => { OnClickTab(temp); });
            }
        }

        protected virtual void OnDestroy()
        {
            if (listTabController == null)
            {
                return;
            }

            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                int temp = i;
                listTabController[i].GetComponent<Button>().onClick.RemoveListener(() => { OnClickTab(temp); });
            }
        }

        void OnClickTab(int index)
        {
            SetTab(index);
        }

        public void UnselectAllTabs()
        {
            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                listTabController[i].UnselectTab();
                if (scaleFactor > 0)
                {
                    //_listTabController[i].gameObject.transform.DOScale(1, 0);
                }
            }
            _indexSelected = -1;
        }

        void TweenScaleOut(int index)
        {
            if (scaleFactor <= 0)
            {
                return;
            }
            if (index < 0 || index >= listTabController.Count)
            {
                return;
            }
            //_listTabController[index].gameObject.transform.DOScale(1, _timeScaleFactor).SetEase(Ease.OutCubic);
        }

        void TweenScaleIn(int index)
        {
            if (scaleFactor <= 0)
            {
                return;
            }
            if (index < 0 || index >= listTabController.Count)
            {
                return;
            }
            //_listTabController[index].gameObject.transform.DOScale(_scaleFactor, _timeScaleFactor).SetEase(Ease.OutCubic);
        }

        public bool SetTab(int index)
        {
            if (listTabController == null)
            {
                return false;
            }

            if (index < 0 || index >= listTabController.Count)
            {
                return false;
            }
            if (_indexSelected == index)
            {
                if (canDeselect)
                {
                    TweenScaleOut(_indexSelected);
                    if (OnUnselectTabSelected != null)
                    {
                        OnUnselectTabSelected(index);
                    }
                    _indexSelected = -1;
                }

                return false;
            }

            TweenScaleOut(_indexSelected);
            TweenScaleIn(index);

            UnselectAllTabs();

            _indexSelected = index;
            listTabController[_indexSelected].SelectTab();

            if (OnTabSelected != null)
            {
                OnTabSelected(index);
            }
            return true;
        }

        public TabController GetCurrentTabController()
        {
            if (listTabController == null)
            {
                return null;
            }
            if (_indexSelected < 0 || _indexSelected >= listTabController.Count)
            {
                return null;
            }
            return listTabController[_indexSelected];
        }

        public int GetCurrentIndex()
        {
            if (listTabController == null)
            {
                return -1;
            }
            if (_indexSelected < 0 || _indexSelected >= listTabController.Count)
            {
                return -1;
            }
            return _indexSelected;
        }

        public int GetAmountTabs()
        {
            if (listTabController == null)
            {
                return 0;
            }
            return listTabController.Count;
        }

        public TabController GetTabByIndex(int index)
        {
            if (listTabController == null)
            {
                return null;
            }
            if (index < 0 || index >= listTabController.Count)
            {
                return null;
            }
            return listTabController[index];
        }

        public void HideAllTabs()
        {
            if (listTabController == null)
            {
                return;
            }

            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                listTabController[i].gameObject.SetActive(false);
            }
        }

        public void EnableAllTabs()
        {
            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                listTabController[i].GetComponent<Button>().interactable = true;
                CanvasGroup canvasGroup = listTabController[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                }
            }
        }

        public void DisableAllTabs()
        {
            int amount = listTabController.Count;
            for (int i = 0; i < amount; i++)
            {
                listTabController[i].GetComponent<Button>().interactable = false;
                CanvasGroup canvasGroup = listTabController[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = _alphaDisable;
                }
            }
        }

        public void EnableTab(int index)
        {
            int amount = listTabController.Count;
            if (index >= 0 && index < amount)
            {
                listTabController[index].GetComponent<Button>().interactable = true;
                CanvasGroup canvasGroup = listTabController[index].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                }
            }
        }

        public void DisableTab(int index)
        {
            int amount = listTabController.Count;
            if (index >= 0 && index < amount)
            {
                listTabController[index].GetComponent<Button>().interactable = false;
                CanvasGroup canvasGroup = listTabController[index].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = _alphaDisable;
                }
            }
        }
    }
}
