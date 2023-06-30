using System.Collections;
using UnityEngine;

namespace Managers.InRoomGems
{
    public class GemsAnimationController : MonoBehaviour
    {
        Animator animator;

        private const string EndGemTrigger = "End_Diamond";
        private const string PickGemTrigger = "Pick_Diamond";

        [SerializeField] private AnimationClip diamondStart;
        [SerializeField] private AnimationClip diamondEnd;
        private static readonly int DiamondEnd = Animator.StringToHash(EndGemTrigger);
        private static readonly int DiamondPick = Animator.StringToHash(PickGemTrigger);
        private GemsAnimationStates CurrentAnimationState { get; set; }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void PlaysNewGemSequence()
        {
            StopCoroutine(CoroutinePlayNewGemSequence());
            StartCoroutine(CoroutinePlayNewGemSequence());
        }

        public float GetEndAnimationTime()
        {
            return diamondEnd.length;
        }

        public IEnumerator PickGem(GameObject gem)
        {
            PlayNewState(GemsAnimationStates.Diamond_Pick);
            yield return new WaitForSeconds(3f);
            gem.SetActive(false);
        }

        IEnumerator CoroutinePlayNewGemSequence()
        {
            ChangeAnimationState(GemsAnimationStates.Diamond_Start);

            yield return new WaitForSeconds(diamondStart.length);

            ChangeAnimationState(GemsAnimationStates.Diamond_Idle);
        }

        void ChangeAnimationState(GemsAnimationStates newAnimationState)
        {
            if (CurrentAnimationState.Equals(newAnimationState)) return;

            PlayNewState(newAnimationState);

            CurrentAnimationState = newAnimationState;
        }

        void PlayNewState(GemsAnimationStates newAnimationState)
        {
            animator.Play(newAnimationState.ToString());
        }

        public void PlayGemEnd()
        {
            if (CurrentAnimationState.Equals(GemsAnimationStates.Diamond_End)) return;

            animator.SetTrigger(DiamondEnd);
            CurrentAnimationState = GemsAnimationStates.Diamond_End;
        }
    }

    enum GemsAnimationStates
    {
        Diamond_Start,
        Diamond_Idle,
        Diamond_End,
        Diamond_Pick
    }
}