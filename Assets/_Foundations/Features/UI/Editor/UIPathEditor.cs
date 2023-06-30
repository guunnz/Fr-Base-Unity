using UnityEditor;
using UnityEngine;

namespace UI.Editor
{
    [CustomEditor(typeof(UIPath))]
    public class UIPathEditor : UnityEditor.Editor
    {
        private int lastEdited = -1;
        private UIPath Path => target as UIPath;

        private void OnSceneGUI()
        {
            RenderLines();
            RenderPointHandles();
            RenderAddMiddle();
        }

        private void RenderAddMiddle()
        {
        }

        private float Perspective(float size = 1)
        {
            return Camera.current.orthographicSize * size;
        }

        private void RenderPointHandles()
        {
            for (var i = 0; i < Path.pathPoints.Count; i++)
            {
                var currentPoint = Path.pathPoints[i];
                PathPoint? lastPoint = null;
                PathPoint? nextPoint = null;

                if (i - 1 >= 0)
                    lastPoint = Path.pathPoints[i - 1];


                if (i + 1 < Path.pathPoints.Count)
                    nextPoint = Path.pathPoints[i + 1];

                PointHandle(ref currentPoint, lastPoint, nextPoint, i);

                Path.pathPoints[i] = currentPoint;
            }
        }

        private void RenderLines()
        {
            for (var i = 0; i < Path.pathPoints.Count - 1; i++)
            {
                var point = Path.pathPoints[i];
                var nextPoint = Path.pathPoints[i + 1];

                Handles.color = Color.blue;
                Handles.DrawLine(Path.GetPosition(point), Path.GetPosition(nextPoint));
            }
        }

        private Vector2 BlockDirectionMovement(Vector2 originalPoint, Vector2 direction, Vector2 desiredPosition)
        {
            var newDir = Vector3.Project(desiredPosition - originalPoint, direction);
            return originalPoint + (Vector2) newDir;
        }

        private void PointHandle(ref PathPoint point, PathPoint? lastPoint, PathPoint? nextPoint, int index)
        {
            var position = Path.GetPosition(point);

            var newPos = Handles.FreeMoveHandle(position, Quaternion.identity,
                Perspective(0.05f), Vector3.zero, Handles.CircleHandleCap);

            Vector3? blocker = null;

            if (Event.current.alt && !Event.current.control) blocker = Vector2.up;

            if (!Event.current.alt && Event.current.control) blocker = Vector2.right;


            if (Event.current.alt && Event.current.control && nextPoint.HasValue && lastPoint.HasValue)
                blocker = Path.GetPosition(lastPoint.Value) - Path.GetPosition(nextPoint.Value);

            if (blocker.HasValue) newPos = BlockDirectionMovement(position, blocker.Value, newPos);


            var lastPosition = point.position;

            Path.SetPosition(ref point, newPos);

            if (blocker.HasValue && lastEdited == index)
            {
                var lastColor = Handles.color;
                Handles.color = Color.red;
                var dir = blocker.Value.normalized * Perspective(0.5f);
                Handles.DrawLine(newPos + dir, newPos - dir);
                Handles.color = lastColor;
            }

            if (lastPosition != point.position)
            {
                lastEdited = index;
                EditorUtility.SetDirty(target);
                Path.OnDirty();
            }
        }
    }
}