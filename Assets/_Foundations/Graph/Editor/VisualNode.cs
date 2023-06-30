using UnityEngine;

namespace Graph.Editor
{
    public class VisualNode
    {
        public Rect rect;

        public GUIStyle style;
        public string title;

        public VisualNode(Vector2 position, float width, float height, GUIStyle nodeStyle)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw()
        {
            GUI.Box(rect, title, style);
        }

        public bool ProcessEvents(Event e)
        {
            return false;
        }
    }
}