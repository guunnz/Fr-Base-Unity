
using UnityEngine;
using UnityEngine.UI;
using System;
using UI.TabController;

namespace UI.ScrollView
{
    public abstract class UIAbstractCardController : TabController.TabController
    {
        [SerializeField] protected Button button;

        public virtual void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
        {
        }

        protected virtual void Start()
        {
        }

        public virtual void Destroy()
        {
        }
    }
}