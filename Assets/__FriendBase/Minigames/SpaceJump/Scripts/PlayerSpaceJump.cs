using DG.Tweening;
using Snapshots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceJump : AbstractPlayer
{
    [SerializeField] float speed;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float jumpHeight;
    [SerializeField] float superJumpHeight;
    [SerializeField] float specialPlatformJumpHeight;
    [SerializeField] BoxCollider2D playerBoxCollider;
    [SerializeField] Transform avatarTransform;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer playerHead;
    [SerializeField] ParticleSystem particles;
    [SerializeField] float movingThresholdToSuperJump;
    [SerializeField] Transform spaceSuit;
    private float movingThresholdToSuperJumpAux;
    private Rigidbody2D rb;
    private bool supportsGyroscope;
    private bool facingRight;
    private bool grounded;
    private bool jumping;
    private MinigameInputManager InputMinigame;
    private bool usingGyroscope;

    //Sound, temporary
    [SerializeField] AudioClip Jump1;
    [SerializeField] AudioClip Jump2;
    [SerializeField] AudioSource audioSource;


    private bool soundOff; //PLEASE REMOVE AFTER PROPER SOUND IMPLEMENTATION PLEASE

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        supportsGyroscope = SystemInfo.supportsGyroscope;
        
    }

    private void Start()
    {
        soundOff = PlayerPrefs.GetInt(Settings.PlayerPrefsValues.SoundOff) == 1;
        InputMinigame = MinigameInputManager.Singleton;
        playerHead.sprite = MinigamePlayerAvatar.Singleton.GetFace();
        usingGyroscope = PlayerPrefs.GetInt("Tilt") == 1 ? true : false;
    }

    private void Update()
    {
        IsGrounded();
        if (supportsGyroscope && usingGyroscope)
        {
            float x = Input.acceleration.x;
            if (x != 0) //If gyroscope value is 0 check buttons
            {
                Move(x);
            }
            else //Get Buttons Value
            {
                Move(InputMinigame.GetHorizontal());
            }
            return;
        }
        else
        {
            Move(InputMinigame.GetHorizontal()); //If doesnt have gyroscope
        }




  

#if UNITY_EDITOR

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Move(Input.GetAxisRaw("Horizontal")); //Testing in editor
        }
#endif
    }

    private void Move(float x)
    {
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
        if (x != 0)
        {
            movingThresholdToSuperJumpAux += Time.deltaTime;
            facingRight = x > 0;
            avatarTransform.localScale = new Vector3(facingRight ? 1 : -1, avatarTransform.localScale.y, avatarTransform.localScale.z);
        }
    }

    private IEnumerator Jump(bool superJump = false)
    {
        if (superJump)
        {
            spaceSuit.DORotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z + 360), 0.75f, RotateMode.FastBeyond360);
            rb.velocity = new Vector2(rb.velocity.x, specialPlatformJumpHeight);
        }
        else
        {
            bool shouldSuperJump = movingThresholdToSuperJumpAux >= movingThresholdToSuperJump && Mathf.Abs(rb.velocity.x) > 0;
            movingThresholdToSuperJumpAux = 0;

            if (shouldSuperJump)
            {
                spaceSuit.DORotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z + 360), 0.75f, RotateMode.FastBeyond360);
                rb.velocity = new Vector2(rb.velocity.x, superJumpHeight);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }

        jumping = true;


        if (!soundOff)
        {
            if (Random.Range(0, 1) == 0)
            {
                audioSource.clip = Jump1;
                audioSource.Play();
            }
            else
            {
                audioSource.clip = Jump2;
                audioSource.Play();
            }

        }

        while (rb.velocity.y > 0) //wait for falling
        {
            yield return null;
        }
        animator.SetBool(AnimationStates.SpaceJump_Jump, false);
        jumping = false;
    }

    private void IsGrounded()
    {
        float extraHeightText = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, extraHeightText, whatIsGround);
        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(playerBoxCollider.bounds.center + new Vector3(playerBoxCollider.bounds.extents.x, 0), Vector2.down * (playerBoxCollider.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(playerBoxCollider.bounds.center - new Vector3(playerBoxCollider.bounds.extents.x, 0), Vector2.down * (playerBoxCollider.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(playerBoxCollider.bounds.center + new Vector3(0, playerBoxCollider.bounds.extents.y), Vector2.right * (playerBoxCollider.bounds.extents.x), rayColor);
        grounded = raycastHit.collider != null;

        if (grounded && !jumping)
        {
            particles.Play();
            animator.SetBool(AnimationStates.SpaceJump_Jump, true);
            if (raycastHit.transform.CompareTag(Tags.SpecialPlatform) || raycastHit.transform.CompareTag(Tags.Platform))
                raycastHit.transform.DOPunchPosition(new Vector3(0, -0.5f, 0f), 0.3f);

            StartCoroutine(Jump(raycastHit.collider.CompareTag(Tags.SpecialPlatform)));
        }
    }

    public void UseGyroscope(bool usingGyroscope)
    {
        this.usingGyroscope = usingGyroscope;
    }
}