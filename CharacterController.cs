using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //References
    [HideInInspector] public CharacterStats characterStats;
    [HideInInspector] public CharacterCombat characterCombat;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;

    [Header("Movement Speed")]
    [Tooltip("Use this for overburdened, blocking, or while bow is drawn")]
    public float burdenedSpeed = 1f;
    public float crouchSpeed = 2f;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 8f;
    [HideInInspector]
    public const float locomotionAnimationSmoothTime = .1f;
    protected bool isCrouching = false;
    public bool acceptInput = true;

    //Drag
    [HideInInspector] public float groundDrag = 6f;
    [HideInInspector] public float airDrag = 2f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    //[HideInInspector] 
    public float groundDistance = 0.3f;
    [HideInInspector] public bool isGrounded;

    protected virtual void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        characterCombat = GetComponent<CharacterCombat>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        SetRigidbodyState(true);
        SetColliderState(false);
    }

    protected virtual void Start()
    {
        characterStats.onDeath += Die;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Detect if the character is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        ControlDrag();
        AnimationsControl();
    }

    //Set animation parameter values
    public virtual void AnimationsControl()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isCrouching", isCrouching);
    }

    //Set rigidbody drag based on if in air or not
    protected virtual void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    #region - Death & Ragdoll - 

    public virtual void Die()
    {
        characterCombat.enabled = false;
        animator.enabled = false;
        SetRigidbodyState(false);
        SetColliderState(true);
        this.enabled = false;
    }

    protected virtual void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        rb.isKinematic = !state;
    }

    protected virtual void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }

    #endregion

}
