using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScrollView
{
    public abstract class UIAbstractUIElementWithScroll : MonoBehaviour
    {
        [SerializeField] protected GameObject viewPort;
        [SerializeField] protected GameObject content;
        [SerializeField] protected UIDragManager dragBagManager;
        [SerializeField] protected int maxCards = 12;
        [SerializeField] protected float toleranceToDeselectObjectByDragging = 1f; //Tolerance to deselect a selected object if you drag too much
        [SerializeField] protected float limitAmountCardSpaceToSwap = 3f; //Width card * _limitWidthCardToSwap to Swap objects
        [SerializeField] private bool useGrid = false;

        protected Canvas canvas;

        protected List<System.Object> listObjects;
        protected List<UIAbstractCardController> listCards;
        protected UIAbstractCardController cardSelected;

        protected int index;

        protected float spacing = 0;
        protected System.Object bagElementSelected;
        protected float widthCard;
        protected float heightCard;

        protected abstract void ReturnObjectToPool(UIAbstractCardController card);
        protected abstract UIAbstractCardController GetNewCard();
        public abstract List<System.Object> GetListElements();

        protected abstract void MouseDownElement(System.Object element, UIAbstractCardController cardController);
        protected abstract void MouseUpElement(System.Object element, UIAbstractCardController cardController);

        protected virtual void Awake()
        {
            index = 0;

            //canvas = transform.root.GetComponent<Canvas>();
            canvas = GetParentCanvasRecursively();
            CalculateSpacing();
        }

        Canvas GetParentCanvasRecursively()
        {
            GameObject currentGameObject = this.gameObject;
            Canvas canvasToFind = currentGameObject.GetComponent<Canvas>();
            while (currentGameObject.transform.parent!=null && canvasToFind == null)
            {
                currentGameObject = currentGameObject.transform.parent.gameObject;
                canvasToFind = currentGameObject.GetComponent<Canvas>();
            }
            return canvasToFind;
        }

        protected void CalculateSpacing()
        {
            if (useGrid)
            {
                if (dragBagManager.GetTypeDrag() == TYPE_DRAG.HORIZONTAL)
                {
                    spacing = content.GetComponent<GridLayoutGroup>().spacing.x;
                }
                else if (dragBagManager.GetTypeDrag() == TYPE_DRAG.VERTICAL)
                {
                    spacing = content.GetComponent<GridLayoutGroup>().spacing.y;
                }
            }
            else
            {
                //Common spacing for horizontal and vertical
                spacing = content.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing;
            }
        }

        int GetAmountHorizontalElementsOnGridLayout()
        {
            //Get grid layout
            GridLayoutGroup lg = content.GetComponent<GridLayoutGroup>();
            if (lg != null)
            {
                //Total width of the contentViewport
                float width = content.GetComponent<RectTransform>().rect.width;
                //Width of the card (including spacing)
                float sizeButt = lg.cellSize.x + lg.spacing.x;
                //We get the amount of cards horizontally
                int hSize = Mathf.FloorToInt(width / sizeButt);
                return hSize;
            }
            return 1;
        }

        public virtual void ShowObjects()
        {
            CleanViewportOfSnaphots();

            listObjects = GetListElements();

            int amountObjects = listObjects.Count;
            if (amountObjects > maxCards)
            {
                amountObjects = maxCards;
            }
            listCards = new List<UIAbstractCardController>();
            for (int i = 0; i < amountObjects; i++)
            {
                AddCard(i, true);
            }
        }

        protected virtual void CleanViewportOfSnaphots()
        {
            index = 0;
            int amount = content.gameObject.transform.childCount;
            for (int i = amount - 1; i >= 0; i--)
            {
                GameObject obj = content.gameObject.transform.GetChild(i).gameObject;
                UIAbstractCardController card = obj.GetComponent<UIAbstractCardController>();
                if (card != null)
                {
                    card.Destroy();
                    ReturnObjectToPool(card);
                }
            }
        }

        protected virtual void AddCard(int index, bool lastElement)
        {
            UIAbstractCardController newCard = GetNewCard();
            newCard.gameObject.SetActive(true);
            newCard.transform.SetParent(content.transform);
            newCard.transform.localScale = Vector3.one;

            System.Object bagElement = listObjects[index];
            newCard.SetUpCard(bagElement, OnClickCard);
            widthCard = newCard.GetComponent<RectTransform>().sizeDelta.x;
            heightCard = newCard.GetComponent<RectTransform>().sizeDelta.y;

            if (lastElement)
            {
                listCards.Add(newCard);
            }
            else
            {
                listCards.Insert(0, newCard);
                newCard.transform.SetAsFirstSibling();
            }

            Vector3 position = newCard.GetComponent<RectTransform>().anchoredPosition3D;
            position.z = 0;
            newCard.GetComponent<RectTransform>().anchoredPosition3D = position;
            OnAddCard(newCard);
        }

        protected virtual void OnAddCard(UIAbstractCardController card)
        {

        }

        protected virtual void OnClickCard(EventType eventType, System.Object bagElement, UIAbstractCardController card)
        {
            if (eventType == EventType.MouseDown)
            {
                cardSelected = card;
                bagElementSelected = bagElement;
                dragBagManager.MouseDown();
                MouseDownElement(bagElement, card);
            }
            else if (eventType == EventType.MouseUp)
            {
                dragBagManager.MouseUp();
                if (bagElementSelected != null)
                {
                    MouseUpElement(bagElementSelected, card);
                    bagElementSelected = null;
                }
            }
        }

        protected virtual void Update()
        {
            if (listCards == null)
            {
                return;
            }

            //If we drag so much => we deselect the element selected so the mouseUp does not take action
            if (bagElementSelected != null)
            {
                if (dragBagManager.GetTypeDrag() == TYPE_DRAG.HORIZONTAL)
                {
                    if (Mathf.Abs(dragBagManager.GetDeltaX() / canvas.scaleFactor) > widthCard * toleranceToDeselectObjectByDragging)
                    {
                        bagElementSelected = null;
                    }
                }
                else if (dragBagManager.GetTypeDrag() == TYPE_DRAG.VERTICAL)
                {
                    if (Mathf.Abs(dragBagManager.GetDeltaY() / canvas.scaleFactor) > heightCard * toleranceToDeselectObjectByDragging)
                    {
                        bagElementSelected = null;
                    }
                }
            }

            CheckSwapping();
        }

        protected void CheckSwapping()
        {
            if (dragBagManager.GetTypeDrag() == TYPE_DRAG.HORIZONTAL)
            {
                if (dragBagManager.GetDirection() == DIRECTION_DRAG_BAG.LEFT)
                {
                    if (listCards.Count > 0)
                    {
                        //If the left x position < amountLimit Cards * width (including spacing) => Swap Left
                        if (content.GetComponent<RectTransform>().anchoredPosition.x < -(widthCard + spacing) * limitAmountCardSpaceToSwap)
                        {
                            SwappingLeft();
                        }
                    }
                }
                else if (dragBagManager.GetDirection() == DIRECTION_DRAG_BAG.RIGHT)
                {
                    //If the right x position (posx + widthContent) > right position of viewPort + amountLimit Cards * width (including spacing) => Swap Right
                    if (listCards.Count > 0)
                    {
                        float posx = content.GetComponent<RectTransform>().anchoredPosition.x;
                        float width = content.GetComponent<RectTransform>().rect.width;
                        if (posx + width > viewPort.GetComponent<RectTransform>().rect.width + (widthCard + spacing) * limitAmountCardSpaceToSwap)
                        {
                            SwappingRight();
                        }
                    }
                }
            }
            else if (dragBagManager.GetTypeDrag() == TYPE_DRAG.VERTICAL)
            {
                if (dragBagManager.GetDirection() == DIRECTION_DRAG_BAG.UP)
                {
                    if (listCards.Count > 0)
                    {
                        //(y axis inverted in Unity) If the top y position > amountLimit Cards * height (including spacing) => Swap Top 
                        if (content.GetComponent<RectTransform>().anchoredPosition.y > (heightCard + spacing) * limitAmountCardSpaceToSwap)
                        {
                            SwappingTop();
                        }
                    }
                }
                else if (dragBagManager.GetDirection() == DIRECTION_DRAG_BAG.DOWN)
                {
                    if (listCards.Count > 0)
                    {
                        //(y axis inverted in Unity) If the bototm y position (posy + heightContent) < bottom position of viewPort + amountLimit Cards * height (including spacing) => Swap Down

                        float posy = content.GetComponent<RectTransform>().anchoredPosition.y;
                        float height = content.GetComponent<RectTransform>().rect.height;
                        if (posy - height < -viewPort.GetComponent<RectTransform>().rect.height - (heightCard + spacing) * limitAmountCardSpaceToSwap)
                        {
                            SwappingDown();
                        }
                    }
                }
            }
        }

        public virtual void SwappingTop()
        {
            //Delete Card from top and add one at the bottom (If we have cards to add from the botton)
            if (index + maxCards >= listObjects.Count)
            {
                return;
            }

            float heightCard = 0;
            int amountCardsToSwap = 1;

            //If we are using a gridLayout we need to know how many elements we have horizontally to remove/add them all together
            if (useGrid)
            {
                amountCardsToSwap = GetAmountHorizontalElementsOnGridLayout();
            }

            //We remove the amount of cards that we have horizontally and add cards as we can at the bottom (checking every time that we have cards to add)
            for (int i = 0; i < amountCardsToSwap; i++)
            {
                //Get First Card
                UIAbstractCardController card = listCards[0];
                //Remove First Card and return to pool
                card.Destroy();
                listCards.RemoveAt(0);
                ReturnObjectToPool(card);
                //Get height card
                heightCard = card.GetComponent<RectTransform>().sizeDelta.y;

                //Check if it is not out of range then we add the new element in the last position
                if (index + maxCards < listObjects.Count)
                {
                    AddCard(index + maxCards, true);
                    index++;
                }
            }

            //Move Offset container
            content.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, heightCard + spacing);
            dragBagManager.ForceUpdate(-(heightCard + spacing));
        }

        public virtual void SwappingDown()
        {
            //Delete Card from right andd add one at the left
            if (index <= 0)
            {
                return;
            }

            float heightCard = 0;
            //This is for horizontally and vertically layout
            int amountCardsToSwap = 1;
            int amountToRemove = 1;

            //If it is gridLayout the we change some settings
            if (useGrid)
            {
                //We need to know how many elements we have horizontally to remove/add them all together
                amountCardsToSwap = GetAmountHorizontalElementsOnGridLayout();
                amountToRemove = amountCardsToSwap;

                //If we are at the last row of elements we need special setting to control how many elements we have at the last row
                if (index + maxCards == listObjects.Count)
                {
                    //if we have different amount of elements in the last as the GetAmountHorizontalElementsOnGridLayout, we change the settings
                    //If we have the same amount then we keep the settings: amountToRemove = amountCardsToSwap;
                    if (index % amountCardsToSwap != 0)
                    {
                        amountToRemove = index % amountCardsToSwap; //We get the amount of elements in the last row
                        index = index + (amountCardsToSwap - amountToRemove); //We move the index to the last element of the same row to not loose elemente to add
                    }
                }
            }

            for (int i = 0; i < amountCardsToSwap; i++)
            {
                //Remove element from bottom only if we still have elements to remove in the last row. Depending on the amount of elements in the last row
                if (i < amountToRemove)
                {
                    int lastIndexItem = listCards.Count - 1;
                    //Get Last Card
                    UIAbstractCardController card = listCards[lastIndexItem];
                    //Remove First Card and return to pool
                    card.Destroy();
                    listCards.RemoveAt(lastIndexItem);
                    ReturnObjectToPool(card);

                    //Get width card
                    heightCard = card.GetComponent<RectTransform>().sizeDelta.y;
                }

                if (index > 0)
                {
                    //Add cards on top
                    AddCard(index - 1, false);
                    index--;
                }
            }

            //Move Offset container
            content.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, heightCard + spacing);
            dragBagManager.ForceUpdate((heightCard + spacing));
        }

        public void SwappingLeft()
        {
            //Delete Card from left andd add one at the right
            if (index + maxCards >= listObjects.Count)
            {
                return;
            }

            float widthCard = 0;
            int amountCardsToSwap = 1;
            for (int i = 0; i < amountCardsToSwap; i++)
            {
                //Get First Card
                UIAbstractCardController card = listCards[0];
                //Remove First Card and return to pool
                card.Destroy();
                listCards.RemoveAt(0);
                ReturnObjectToPool(card);

                //Get width card
                widthCard = card.GetComponent<RectTransform>().sizeDelta.x;
            }

            //Move Offset container
            content.GetComponent<RectTransform>().anchoredPosition += new Vector2(widthCard + spacing, 0);

            AddCard(index + maxCards, true);
            index++;
            dragBagManager.ForceUpdate(widthCard + spacing);
        }

        public void SwappingRight()
        {
            //Delete Card from right andd add one at the left
            if (index <= 0)
            {
                return;
            }

            float widthCard = 0;
            int amountCardsToSwap = 1;
            for (int i = 0; i < amountCardsToSwap; i++)
            {
                int lastIndexItem = listCards.Count - 1;
                //Get Last Card
                UIAbstractCardController card = listCards[lastIndexItem];
                //Remove First Card and return to pool
                card.Destroy();
                listCards.RemoveAt(lastIndexItem);
                ReturnObjectToPool(card);

                //Get width card
                widthCard = card.GetComponent<RectTransform>().sizeDelta.x;
            }

            //Move Offset container
            content.GetComponent<RectTransform>().anchoredPosition -= new Vector2(widthCard + spacing, 0);

            AddCard(index - 1, false);
            index--;
            dragBagManager.ForceUpdate(-(widthCard + spacing));
        }

        public GameObject GetCardByIndex(int index)
        {
            if (listCards == null || index < 0 || index >= listCards.Count)
            {
                return null;
            }

            return listCards[index].gameObject;
        }

        public int GetAmountElements()
        {
            if (listCards != null)
            {
                return listCards.Count;
            }
            return 0;
        }

        public void ResetPosition()
        {
            dragBagManager.ResetPosition();
        }
    }
}