using System.Collections;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class BasicPathFollower : MonoBehaviour
    {
        public PathCreator pathCreatorLeft;
        public PathCreator pathCreatorMid;
        public PathCreator pathCreatorRight;
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 0;
        public float distanceTravelled = 5f;
        public RacingPath currentPathSelected;
        private Road road;
        private RacingMinigameGameplayManager RMGM;

        public bool StartonStart;

        private void Awake()
        {
            road = FindObjectOfType<Road>();
            RMGM = FindObjectOfType<RacingMinigameGameplayManager>();
        }

        private void Start()
        {
            if (StartonStart)
                SetPaths(currentPathSelected);
        }

        public void SetPaths(RacingPath pathSelected)
        {
            StartCoroutine(ISetPaths(pathSelected));
        }

        IEnumerator ISetPaths(RacingPath pathSelected)
        {
            while (!RMGM.raceStarted)
                yield return null;

            currentPathSelected = pathSelected;
            pathCreatorMid = road.pathCreatorMid;
            pathCreatorRight = road.pathCreatorRight;
            pathCreatorLeft = road.pathCreatorLeft;
            pathCreator = currentPathSelected == RacingPath.Left ? road.pathCreatorLeft : currentPathSelected == RacingPath.Right ? road.pathCreatorRight : road.pathCreatorMid;
            OnPathChanged();
            SetPosition();
            yield return new WaitForSeconds(0.05f);
            this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, -90f);
            Destroy(this);
        }


        private void Update()
        {
            if (RMGM.raceStarted)
                SetPosition();
        }


        public void SetPosition()
        {
            try
            {
                if (this == null)
                    return;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);


            }
            catch
            {

            }

        }

        internal void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}