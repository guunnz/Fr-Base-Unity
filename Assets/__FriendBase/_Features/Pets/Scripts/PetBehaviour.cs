using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using Pathfinding;
using UnityEngine.UIElements;

public class PetBehaviour : MonoBehaviour
{
    [SerializeField] private Transform AvatarToFollow;
    [SerializeField] private Animator _Animator;
    [SerializeField] private float metersOffset = 1f;
    [SerializeField] private float metersOffsetStartWalk = 1f;
    [SerializeField] private float metersPerFrame = 0.05f;
    private bool Moving = false;
    private List<Vector3> PlayerPositions = new List<Vector3>();

    [SerializeField] private int timingChangeBlinkMin = 3;
    [SerializeField] private int timingChangeBlinkMax = 10;

    [SerializeField] private int timingChangeSitMin = 3;
    [SerializeField] private int timingChangeSitMax = 10;

    private Seeker seeker;
    private GameObject destinationPointsContainer;
    private GameObject destinationPoint;
    private float lastDistance;
    private float SittingTimer;
    private float BlinkingTimer;
    private AIDestinationSetter aIDestinationSetter;
    private AIPath aiPath;
    [SerializeField] bool InOnboardingOrMenu;

    public void SetAvatarToFollow(Transform avatar)
    {
        AvatarToFollow = avatar;
    }

    private void Start()
    {
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        destinationPoint = new GameObject("Pet " + "_destinationPoint");
        if (!InOnboardingOrMenu)
        {
            if (CurrentRoom.Instance != null)
            {
                this.transform.parent = CurrentRoom.Instance.GetPetContainer();
            }
            destinationPoint.transform.parent = CurrentRoom.Instance.GetDestinationTransform();
        }
        destinationPoint.transform.position = new Vector3(AvatarToFollow.position.x, AvatarToFollow.position.y, 0);
        aIDestinationSetter.target = destinationPoint.transform;
    }

    private void OnDestroy()
    {
        if (InOnboardingOrMenu)
        {
            Destroy(destinationPoint);
        }
    }

    private void Update()
    {
        SittingTimer -= Time.deltaTime;
        BlinkingTimer -= Time.deltaTime;

        if (SittingTimer <= 0)
        {
            SittingTimer = UnityEngine.Random.Range(timingChangeSitMin, timingChangeSitMax);
            _Animator.SetTrigger(AnimationStates.PetSit);
        }
        if (BlinkingTimer <= 0)
        {
            BlinkingTimer = UnityEngine.Random.Range(timingChangeBlinkMin, timingChangeBlinkMax);

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                _Animator.SetTrigger(AnimationStates.PetHappy);
            }
            else
            {
                _Animator.SetTrigger(AnimationStates.PetSad);
            }

        }
        Move();
    }

    void Move()
    {
        if (AvatarToFollow != null)
        {
            float facing = aiPath.velocity.x < 0 ? -0.123f : 0.123f;

            if (Vector3.Distance(this.transform.position, AvatarToFollow.position) <= metersOffset)
            {
                destinationPoint.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                _Animator.SetTrigger(AnimationStates.PetStop);
            }
            else
            {
                this.transform.localScale = new Vector3(facing, this.transform.localScale.y, this.transform.localScale.z);
                _Animator.SetTrigger(AnimationStates.PetMove);
                destinationPoint.transform.position = new Vector3(AvatarToFollow.position.x, AvatarToFollow.position.y, 0);
                aIDestinationSetter.target = destinationPoint.transform;
            }
        }
    }
}