using System.Collections;
using System.Collections.Generic;
using UI.ScrollView;
using UnityEngine;

public class UIDataPickerScrollView : UIAbstractUIElementWithScroll
{
    public enum TYPE_DATE_PICKER { DAY, MONTH, YEAR }
    public enum STATE_FIX_POSITON { NONE, RUNNING, COMPLETE }

    [SerializeField] private UIDataPickerCardPool dataPickerCardPool;
    [SerializeField] private TYPE_DATE_PICKER typeDatePicker;
    [SerializeField] private int repeatWheelNumber = 100;
    [SerializeField] private float heightItem = 30;

    public delegate void ChangeData(UIDataPickerCardController card);
    public event ChangeData OnChangeData;

    private string[] monthsStrings = new string[] {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December",};

    private RectTransform contentRectTransform;

    private STATE_FIX_POSITON flagFixPosition;
    private float finalYPositionFix;

    private int amountItemsInDatePicker;

    void Start()
    {
        contentRectTransform = content.GetComponent<RectTransform>();
        flagFixPosition = STATE_FIX_POSITON.NONE;
        Show();
    }

    public void Show()
    {
        if (typeDatePicker == TYPE_DATE_PICKER.DAY)
        {
            amountItemsInDatePicker = 31;
            ShowObjectsByIndex(amountItemsInDatePicker * repeatWheelNumber / 2);
        }
        else if (typeDatePicker == TYPE_DATE_PICKER.MONTH)
        {
            amountItemsInDatePicker = 12;
            ShowObjectsByIndex(amountItemsInDatePicker * repeatWheelNumber / 2);
        }
        else if (typeDatePicker == TYPE_DATE_PICKER.YEAR)
        {
            amountItemsInDatePicker = 90;
            ShowObjectsByIndex(amountItemsInDatePicker - 10);
        }
    }

    public void ChangeDayAmount(int newAmount)
    {
        if (typeDatePicker != TYPE_DATE_PICKER.DAY)
        {
            return;
        }

        if (newAmount == amountItemsInDatePicker)
        {
            return;
        }

        amountItemsInDatePicker = newAmount;

        ShowObjectsByIndex(amountItemsInDatePicker * repeatWheelNumber / 2);
    }

    public void ShowObjectsByIndex(int initialIndex)
    {
        CleanViewportOfSnaphots();

        listObjects = GetListElements();

        int amountObjects = listObjects.Count - initialIndex;
        if (amountObjects > maxCards)
        {
            amountObjects = (maxCards + initialIndex);
        }
        listCards = new List<UIAbstractCardController>();
        for (int i = initialIndex; i < amountObjects; i++)
        {
            AddCard(i, true);
        }
        index = initialIndex;
    }

    protected override void Update()
    {
        base.Update();

        ControlFixPosition();
        UpdateTextSizes();
    }

    public void UpdateTextSizes()
    {
        UIDataPickerCardController card = GetCenterCard();
        if (card==null)
        {
            return;
        }

        int indexCard = GetIndexCard(card);
        if (indexCard==-1)
        {
            return;
        }
        card.SetSize(UIDataPickerCardController.SIZE_DATE.BIG);

        card = GetCardDateByIndex(indexCard - 1);
        if (card!=null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.MEDIUM);
        }

        card = GetCardDateByIndex(indexCard - 2);
        if (card != null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.SMALL);
        }

        card = GetCardDateByIndex(indexCard + 1);
        if (card != null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.MEDIUM);
        }

        card = GetCardDateByIndex(indexCard + 2);
        if (card != null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.SMALL);
        }

        card = GetCardDateByIndex(indexCard - 3);
        if (card != null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.SMALL);
        }

        card = GetCardDateByIndex(indexCard + 3);
        if (card != null)
        {
            card.SetSize(UIDataPickerCardController.SIZE_DATE.SMALL);
        }
    }

    private UIDataPickerCardController GetCardDateByIndex(int index)
    {
        if (listCards == null || index < 0 || index >= listCards.Count)
        {
            return null;
        }

        return listCards[index] as UIDataPickerCardController;
    }

    private int GetIndexCard(UIDataPickerCardController card)
    {
        int amount = listCards.Count;
        for (int i=0; i<amount; i++)
        {
            if (card.dataPickerData.Id == ((UIDataPickerCardController) listCards[i]).dataPickerData.Id )
            {
                return i;
            }
        }
        return -1;
    }

    public DataPickerData GetDataPicerDataSelected()
    {
        if (!dragBagManager.HasReachFinalPosition())
        {
            return null;
        }

        if (flagFixPosition == STATE_FIX_POSITON.RUNNING)
        {
            return null;
        }

        UIDataPickerCardController card = GetCenterCard();
        if (card != null)
        {
            return card.dataPickerData;
        }
        return null;
    }

    public UIDataPickerCardController GetCenterCard()
    {
        float amountCardsOutside = (contentRectTransform.anchoredPosition.y / heightItem);
        int amountCardsOutsideRounded = Mathf.RoundToInt(amountCardsOutside);

        int centerIndex = amountCardsOutsideRounded + 2;
        if (centerIndex >= listCards.Count || centerIndex < 0)
        {
            return null;
        }

        UIDataPickerCardController card = listCards[centerIndex] as UIDataPickerCardController;

        //Debug.Log(contentRectTransform.anchoredPosition + " - " + amountCardsOutside + " - " + amountCardsOutsideRounded + " - " + card.dataPickerData.Id);

        return card;
    }

    private void ControlFixPosition()
    {
        if (dragBagManager.HasReachFinalPosition() && !dragBagManager.IsDragging())
        {
            if (flagFixPosition == STATE_FIX_POSITON.NONE)
            {
                finalYPositionFix = Mathf.Round(contentRectTransform.anchoredPosition.y / heightItem) * heightItem;
                flagFixPosition = STATE_FIX_POSITON.RUNNING;
            }
        }
        else
        {
            flagFixPosition = STATE_FIX_POSITON.NONE;
        }
        FixPosition();
    }

    private void FixPosition()
    {
        if (flagFixPosition!=STATE_FIX_POSITON.RUNNING)
        {
            return;
        }

        float delta = (finalYPositionFix - contentRectTransform.anchoredPosition.y);
        float partialPosition = contentRectTransform.anchoredPosition.y + delta * 0.1f;
        Vector2 currentPosition = contentRectTransform.anchoredPosition;
        currentPosition.y = partialPosition;

        if (delta < 0.5f)
        {
            currentPosition.y = finalYPositionFix;

            Debug.Log("--STOP:" + GetCenterCard().dataPickerData.TxtButton);

            flagFixPosition = STATE_FIX_POSITON.COMPLETE;

            if (OnChangeData != null)
            {
                OnChangeData(GetCenterCard());
            }
        }

        contentRectTransform.anchoredPosition = currentPosition;
    }

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UIDataPickerCardController cardController = card as UIDataPickerCardController;
        if (card != null)
        {
            dataPickerCardPool.ReturnToPool(cardController);
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UIDataPickerCardController newCard = dataPickerCardPool.Get();

        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        List<System.Object> listItems = new List<System.Object>();

        for (int j=0; j< repeatWheelNumber; j++)
        {
            if (typeDatePicker == TYPE_DATE_PICKER.DAY)
            {
                for (int i = 0; i < amountItemsInDatePicker; i++)
                {
                    listItems.Add(new DataPickerData(typeDatePicker, i + 1, (i + 1).ToString()));
                }
            }
            else if (typeDatePicker == TYPE_DATE_PICKER.MONTH)
            {
                for (int i = 0; i < monthsStrings.Length; i++)
                {
                    listItems.Add(new DataPickerData(typeDatePicker, i + 1, monthsStrings[i]));
                }
            }
            else if (typeDatePicker == TYPE_DATE_PICKER.YEAR)
            {
                for (int i = 1921; i < 2011; i++)
                {
                    listItems.Add(new DataPickerData(typeDatePicker, i + 1, (i + 1).ToString()));
                }
                listItems.Add(new DataPickerData(typeDatePicker , - 1, ""));
                listItems.Add(new DataPickerData(typeDatePicker , - 1, ""));
            }
        }

        return listItems;
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {

    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        UIDataPickerCardController cardItem = element as UIDataPickerCardController;
        if (element != null)
        {
            Debug.Log("MouseDown");
        }
    }
}
