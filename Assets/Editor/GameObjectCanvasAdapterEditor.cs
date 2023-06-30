using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameObjectCanvasAdapter))]
public class GameObjectCanvasAdapterEditor : UnityEditor.Editor
{
    private Vector2 lastCanvasSize;
    private bool isSubscribed;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        bool hasChanged = EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

        GameObjectCanvasAdapter adapter = (GameObjectCanvasAdapter)target;

        if (hasChanged)
        {
            adapter.UpdatePosition();
        }

        if (!isSubscribed)
        {
            EditorApplication.update += CheckForCanvasSizeChange;
            isSubscribed = true;
        }
    }

    private void CheckForCanvasSizeChange()
    {
        GameObjectCanvasAdapter adapter = (GameObjectCanvasAdapter)target;
        if (adapter.canvas != null)
        {
            RectTransform canvasRectTransform = adapter.canvas.GetComponent<RectTransform>();
            if (canvasRectTransform != null)
            {
                Vector2 currentCanvasSize = canvasRectTransform.rect.size;
                if (currentCanvasSize != lastCanvasSize)
                {
                    lastCanvasSize = currentCanvasSize;
                    adapter.UpdatePosition();
                }
            }
        }
    }

    private void OnDestroy()
    {
        EditorApplication.update -= CheckForCanvasSizeChange;
    }
}
