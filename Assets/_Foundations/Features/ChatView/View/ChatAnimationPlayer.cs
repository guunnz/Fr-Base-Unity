using System.Collections;
using UnityEngine;

namespace ChatView.View
{
    public class ChatAnimationPlayer : MonoBehaviour
    {
        Animator animator;
        [SerializeField] private AnimationClip chatBubblePop;
        public ChatAnimationStates CurrentAnimationState { get; private set; }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void PlaysNewMessageSequence()
        {
            StopCoroutine(PlayNewMessageSequence());
            StartCoroutine(PlayNewMessageSequence());
        }

        IEnumerator PlayNewMessageSequence()
        {
            ChangeAnimationState(ChatAnimationStates.chat_bubble_pop);

            yield return new WaitForSeconds(chatBubblePop.length);
            
            ChangeAnimationState(ChatAnimationStates.chat_bubble_idle);
        }

        public void ChangeAnimationState(ChatAnimationStates newAnimationState)
        {
            if (CurrentAnimationState.Equals(newAnimationState)) return;

            PlayNewState(newAnimationState);

            CurrentAnimationState = newAnimationState;
        }

        void PlayNewState(ChatAnimationStates newAnimationState)
        {
            animator.Play(newAnimationState.ToString());
        }
    }
}