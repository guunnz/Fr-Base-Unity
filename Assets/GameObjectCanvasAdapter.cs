using UnityEngine;

public class GameObjectCanvasAdapter : MonoBehaviour
{
    public Canvas canvas;
    public GameObject followObject;
    public bool followX = true;
    public bool followY = true;
    [Range(0f, 1f)] public float normalizedX = 0.5f;
    [Range(0f, 1f)] public float normalizedY = 0.5f;
    public Vector2 offset;

    private RectTransform canvasRectTransform;

    void Start()
    {
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (canvas != null)
        {
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        if (canvas == null) return;

        if (canvasRectTransform == null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            if (canvasRectTransform == null) return;
        }

        Vector3 targetPosition = Vector3.zero;
        if (followObject != null)
        {
            targetPosition = followObject.transform.position + new Vector3(offset.x, offset.y, 0f);
        }

        Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetPosition);

        Vector2 canvasLocalPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, targetScreenPosition, canvas.worldCamera, out canvasLocalPosition);

        if (followX) normalizedX = (canvasLocalPosition.x + canvasRectTransform.rect.width / 2) / canvasRectTransform.rect.width;
        if (followY) normalizedY = (canvasLocalPosition.y + canvasRectTransform.rect.height / 2) / canvasRectTransform.rect.height;

        Vector3 canvasPosition = new Vector3(
            canvasRectTransform.rect.width * normalizedX - canvasRectTransform.rect.width / 2,
            canvasRectTransform.rect.height * normalizedY - canvasRectTransform.rect.height / 2,
            0f
        );

        transform.position = canvasRectTransform.TransformPoint(canvasPosition);
    }
}
