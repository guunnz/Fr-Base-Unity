using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.ScrollView
{
    public enum DIRECTION_DRAG_BAG { RIGHT, LEFT, UP, DOWN };
    public enum TYPE_DRAG { HORIZONTAL, VERTICAL };

    public class UIDragManager : MonoBehaviour
    {
        [SerializeField] private GameObject viewPort;
        [SerializeField] private GameObject content;
        [SerializeField] private float elasticity = 0.05f;
        [SerializeField] private float forceFactor = 7;
        [SerializeField] private TYPE_DRAG typeDrag = TYPE_DRAG.HORIZONTAL;

        private Vector3 initialContainerPosition;
        private Vector3 clickPosition;
        private Vector3 finalPosition;
        private Vector3 lastPosition;
        private bool isDragging; //Indicate if we are dragging
        private bool controlElasticity; //When MouseUp we start control elasticity until we reach the final position
        private bool reachFinalPosition;  //It activates when we are close to the final position
        private DIRECTION_DRAG_BAG direction;
        private Canvas canvas;

        protected virtual void Awake()
        {
            //canvas = transform.root.GetComponent<Canvas>();
            canvas = GetParentCanvasRecursively();
        }

        Canvas GetParentCanvasRecursively()
        {
            GameObject currentGameObject = this.gameObject;
            Canvas canvasToFind = currentGameObject.GetComponent<Canvas>();
            while (currentGameObject.transform.parent != null && canvasToFind == null)
            {
                currentGameObject = currentGameObject.transform.parent.gameObject;
                canvasToFind = currentGameObject.GetComponent<Canvas>();
            }
            return canvasToFind;
        }

        protected virtual void Start()
        {
            isDragging = false;
            controlElasticity = false;
            clickPosition = Vector2.zero;
            reachFinalPosition = true;
        }

        public virtual void ResetPosition()
        {
            clickPosition = Vector2.zero;
            isDragging = false;
            finalPosition = Vector2.zero;
            content.GetComponent<RectTransform>().anchoredPosition = finalPosition;
        }

        protected virtual void Update()
        {
            if (!isDragging)
            {
                ControlElasticity();
                return;
            }
            Vector3 mousePosition = InputFunctions.mousePosition;
            Vector2 currentContentPosition = content.GetComponent<RectTransform>().anchoredPosition;

            if (typeDrag == TYPE_DRAG.HORIZONTAL)
            {
                finalPosition.x = initialContainerPosition.x + (mousePosition.x - clickPosition.x) / canvas.scaleFactor;
                currentContentPosition.x += (finalPosition.x - currentContentPosition.x) * elasticity;
            }
            else if (typeDrag == TYPE_DRAG.VERTICAL)
            {
                finalPosition.y = initialContainerPosition.y + (mousePosition.y - clickPosition.y) / canvas.scaleFactor;
                currentContentPosition.y += (finalPosition.y - currentContentPosition.y) * elasticity;
            }

            content.GetComponent<RectTransform>().anchoredPosition = currentContentPosition;

            CalculateDragDirection(mousePosition);

            lastPosition = mousePosition;
        }

        public void ControlElasticity()
        {
            if (!controlElasticity)
            {
                return;
            }
            Vector2 currentContentPosition = content.GetComponent<RectTransform>().anchoredPosition;

            if (typeDrag == TYPE_DRAG.HORIZONTAL)
            {
                //This is the x position where it is the end of the content
                float rightPositionX = currentContentPosition.x + content.GetComponent<RectTransform>().rect.width;
                if (currentContentPosition.x > 0)
                {
                    //If x position of the content=>0 then there is empty space on the left part and we need to put it again to 0
                    finalPosition.x = 0;
                }
                else if (rightPositionX < viewPort.GetComponent<RectTransform>().rect.width && (currentContentPosition.x < 0))
                {
                    //if the x o position where it is the end of the content is < width of Viewport => we have drag too much to the left and we need to align the end of the content to the end of the viewport
                    finalPosition.x = viewPort.GetComponent<RectTransform>().rect.width - content.GetComponent<RectTransform>().rect.width;
                }

                currentContentPosition.x += (finalPosition.x - currentContentPosition.x) * elasticity;

                if (Mathf.Abs(currentContentPosition.x - finalPosition.x) < 6)
                {
                    reachFinalPosition = true;
                }
            }
            else if (typeDrag == TYPE_DRAG.VERTICAL)
            {
                float bottomPositionY = currentContentPosition.y - content.GetComponent<RectTransform>().rect.height;

                if (currentContentPosition.y < 0)
                {
                    finalPosition.y = 0;
                }
                else if (bottomPositionY > -viewPort.GetComponent<RectTransform>().rect.height && (currentContentPosition.y > 0))
                {
                    finalPosition.y = (content.GetComponent<RectTransform>().rect.height - viewPort.GetComponent<RectTransform>().rect.height);
                }

                currentContentPosition.y += (finalPosition.y - currentContentPosition.y) * elasticity;

                if (Mathf.Abs(currentContentPosition.y - finalPosition.y) < 6)
                {
                    controlElasticity = false;
                    reachFinalPosition = true;
                }
            }

            content.GetComponent<RectTransform>().anchoredPosition = currentContentPosition;
        }

        public void ForceUpdate(float offset)
        {
            //When we need to remove card from one side and add it to the other, we need to update the position of the dragContainer
            //_isDragging = false;
            //_controlElasticity = true;
            if (typeDrag == TYPE_DRAG.HORIZONTAL)
            {
                initialContainerPosition.x += offset;
                finalPosition.x += offset;
            }
            else if (typeDrag == TYPE_DRAG.VERTICAL)
            {
                initialContainerPosition.y += offset;
                finalPosition.y += offset;
            }
        }

        public void MouseDown()
        {
            //When we start drag, this method is called from other class
            isDragging = true;
            reachFinalPosition = false;
            clickPosition = InputFunctions.mousePosition;
            lastPosition = InputFunctions.mousePosition;
            initialContainerPosition = content.GetComponent<RectTransform>().anchoredPosition;
        }

        void CalculateDragDirection(Vector3 mousePosition)
        {
            if (typeDrag == TYPE_DRAG.HORIZONTAL)
            {
                if (mousePosition.x > lastPosition.x)
                {
                    direction = DIRECTION_DRAG_BAG.RIGHT;
                }
                else if (mousePosition.x < lastPosition.x)
                {
                    direction = DIRECTION_DRAG_BAG.LEFT;
                }
            }
            else if (typeDrag == TYPE_DRAG.VERTICAL)
            {
                if (mousePosition.y > lastPosition.y)
                {
                    direction = DIRECTION_DRAG_BAG.UP;
                }
                else if (mousePosition.y < lastPosition.y)
                {
                    direction = DIRECTION_DRAG_BAG.DOWN;
                }
            }
        }

        public void MouseUp()
        {
            //When we end drag, this method is called from other class
            isDragging = false;
            controlElasticity = true;
            reachFinalPosition = false;

            Vector3 mousePosition = InputFunctions.mousePosition;
            Vector2 currentContentPosition = content.GetComponent<RectTransform>().anchoredPosition;

            finalPosition = currentContentPosition + ((Vector2)mousePosition - (Vector2)lastPosition) / canvas.scaleFactor * forceFactor;

            CalculateDragDirection(mousePosition);
        }

        public float GetDeltaX()
        {
            float deltax = 0;
            if (isDragging)
            {
                deltax = InputFunctions.mousePosition.x - clickPosition.x;
            }
            return deltax;
        }

        public float GetDeltaY()
        {
            float deltay = 0;
            if (isDragging)
            {
                deltay = InputFunctions.mousePosition.y - clickPosition.y;
            }
            return deltay;
        }

        public DIRECTION_DRAG_BAG GetDirection()
        {
            return direction;
        }

        public TYPE_DRAG GetTypeDrag()
        {
            return typeDrag;
        }

        public bool IsDragging()
        {
            return isDragging;
        }

        public bool IsControlElasticity()
        {
            return controlElasticity;
        }

        public bool HasReachFinalPosition()
        {
            return reachFinalPosition;
        }
    }
}