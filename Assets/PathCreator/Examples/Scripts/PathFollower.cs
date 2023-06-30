using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public Transform RightTransform;
        public Transform LeftTransform;
        public CarController carController;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 0;
        public float distanceTravelled = 5f;
        internal RacingPath lastPath;

        private void Awake()
        {
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        }

        void FixedUpdate()
        {
            if (pathCreator != null)
            {
                if (carController == null || !carController.PlayerFinishedRace())
                {
                    distanceTravelled += speed * Time.deltaTime;
                    transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                }
            }
        }

        internal void OnPathChanged()
        {
            switch (carController.currentPathSelected)
            {
                case RacingPath.Left:
                    distanceTravelled = carController.pathCreatorLeft.path.GetClosestDistanceAlongPath(LeftTransform.position);
                    break;
                case RacingPath.Right:
                    distanceTravelled = carController.pathCreatorRight.path.GetClosestDistanceAlongPath(RightTransform.position);
                    break;
                case RacingPath.Mid:
                    if (lastPath == RacingPath.Left)
                    {
                        distanceTravelled = carController.pathCreatorMid.path.GetClosestDistanceAlongPath(RightTransform.position);
                    }
                    else
                    {
                        distanceTravelled = carController.pathCreatorMid.path.GetClosestDistanceAlongPath(LeftTransform.position);
                    }
                    break;
                default:
                    break;
            }
            lastPath = carController.currentPathSelected;
        }
    }
}