using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class PlayerController : CharacterController
{
    #region - Variables -
    [Header("Movement")]
    [HideInInspector] public float moveSpeed;
    public float jumpForce = 5f;
    float acceleration = 10f;
    float movementMultiplier = 10;
    float airMultiplier = 0.4f;

    [HideInInspector] public bool sprinting = false;
    [HideInInspector] public bool toggleWalk = false; //Sets default movement speed to walk instead of run
    bool overburdened = false;  //later add so jump force is less

    [Header("Turning")]
    [HideInInspector] public Vector2 turn;
    public float xSensitivity;
    public float ySensitivity;
    public float xTurnSensitivity = 2f;
    public float yTiltSensitivity = 2f;
    public float aimingXTurnSensitivity = 1f;
    public float aimingYTiltSensitivity = 1f;
    public GameObject followTarget;
    public float minY;
    public float maxY;
    float mouseX;
    float mouseY;

    [Header("Rolling")]
    public float rollCooldown = 1f;
    private float lastRoll;

    [HideInInspector] public float moveAmount;
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 cameraInput;
    Vector3 movementDirection;

    [SerializeField] Transform player;
    [HideInInspector] public Camera cam3P;
    VFXManager vfx;

    #region - Crouching -
    [Header("Crouching")]
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] CapsuleCollider playerFrictionlessCollider;
    [Space]
    public float colliderCenter = 0.9f;
    public float colliderCrouchCenter = 0.45f;
    public float crouchChangeTime = 5f;
    [Space]
    public float collider1Height = 1.8f;
    public float collider1CrouchHeight = 0.9f;
    [Space]
    public float collider2Height = 1.4f;
    public float collider2CrouchHeight = 0.7f;
    #endregion

    [Header("Testing")]
    [SerializeField] GameObject stepRayLower;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] float stepHeight = 0.3f;
    //[SerializeField] float stepSmooth = 0.1f; //lower value means smoother jump

    #endregion

    protected override void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        characterCombat = GetComponent<PlayerCombat>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    protected override void Start()
    {
        base.Start();
        vfx = VFXManager.instance;
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;

        //Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        //Hide the cursor
        Cursor.visible = false;
    }

    public override void Update()
    {
        base.Update();
        if (!acceptInput)
            return;

        MyInput();
        ControlSpeed();
    }

    private void FixedUpdate()
    {
        if (!acceptInput)
            return;

        MovePlayer();
        StepClimb();
    }

    //Set animation parameter values
    public override void AnimationsControl()
    {
        base.AnimationsControl();

        animator.SetFloat("Horizontal", horizontalMovement);
        animator.SetFloat("Vertical", verticalMovement);

        float speedPercent = rb.velocity.magnitude / sprintSpeed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
    }

    #region - Movement Controls -
    void MyInput()
    {
        horizontalMovement = movementInput.x;
        verticalMovement = movementInput.y;
        movementDirection = player.forward * verticalMovement + player.right * horizontalMovement;

        mouseX = cameraInput.x * Time.deltaTime;
        mouseY = cameraInput.y * Time.deltaTime;

        if (characterCombat.isBlocking || characterCombat.bowDrawn || characterCombat.castingSpell_Primary || characterCombat.castingSpell_Secondary)
        {
            turn.x += mouseX * aimingXTurnSensitivity;
            turn.y -= mouseY * aimingYTiltSensitivity;
        }
        else
        {
            turn.x += mouseX * xTurnSensitivity;
            turn.y -= mouseY * yTiltSensitivity;
        }

        turn.y = Mathf.Clamp(turn.y, minY, maxY);
    }

    public void Sprint()
    {
        //if (!acceptInput)
            //return;

        sprinting = !sprinting;

        if (sprinting && isCrouching)
        {
            Crouch();
        }
    }

    public void Jump()
    {
        if (!isGrounded || isCrouching || !acceptInput)
            return;

        animator.Play("Jump", 0);
        rb.velocity = new Vector3(rb.velocity.x, 2, rb.velocity.z);
        rb.AddForce(transform.up * (Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y)), ForceMode.VelocityChange);
        Instantiate(vfx.jump_FX, groundCheck.transform.position, groundCheck.transform.rotation);
    }

    public void Dodge()
    {
        if (Time.time - lastRoll < rollCooldown || verticalMovement == 0 || !isGrounded || isCrouching || !acceptInput)
            return;

        lastRoll = Time.time;
       
        if (verticalMovement > 0)
        {
            //Add forwards push or apply rootmotion
            rb.AddForce(transform.forward * 500f);
            animator.Play("Roll Forwards", 0);
            animator.Play("Roll Forwards", 1);
        }
        else
        {
            //Add backwards push
            rb.AddForce(transform.forward * -500f);
            animator.Play("Roll Backwards", 0);
            animator.Play("Roll Backwards", 1);
        }
    }

    public void Crouch()
    {
        if (!acceptInput)
            return;

        isCrouching = !isCrouching;
        
        if (!isCrouching)
        {
            playerCollider.height = Mathf.Lerp(collider1Height, collider1CrouchHeight, crouchChangeTime * Time.deltaTime);
            playerCollider.center = new Vector3(0f, colliderCenter, 0f);

            playerFrictionlessCollider.height = Mathf.Lerp(collider2Height, collider2CrouchHeight, crouchChangeTime * Time.deltaTime);
            playerFrictionlessCollider.center = new Vector3(0f, colliderCenter, 0f);
        }
        else
        {
            playerCollider.height = Mathf.Lerp(collider1CrouchHeight, collider1Height, crouchChangeTime * Time.deltaTime);
            playerCollider.center = new Vector3(0f, colliderCrouchCenter, 0f);

            playerFrictionlessCollider.height = Mathf.Lerp(collider2CrouchHeight, collider2Height, crouchChangeTime * Time.deltaTime);
            playerFrictionlessCollider.center = new Vector3(0f, colliderCrouchCenter, 0f);
        }
        
    }

    void ControlSpeed()
    {
        //Burdened (blocking, aiming, or overencumbered)
        if (overburdened)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, burdenedSpeed, acceleration * Time.deltaTime);
        }
        //Crouching
        else if (isCrouching)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration * Time.deltaTime);
        }
        //Sprinting
        else if (sprinting && verticalMovement == 1 && horizontalMovement == 0)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        //Walking
        else if (toggleWalk || characterCombat.bowDrawn || characterCombat.isBlocking || characterCombat.castingSpell_Primary)
        {
           moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
        //Running
        else if (!toggleWalk)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            Debug.Log("Speed Error");
        }
    }
    
    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(movementDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(movementDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        if (rb.velocity.magnitude > 0.01 || PlayerCombat.instance.bowDrawn || PlayerCombat.instance.castingSpell_Primary)
        {
            transform.rotation = Quaternion.Euler(0, turn.x, 0);
        }

        followTarget.transform.rotation = Quaternion.Euler(turn.y, turn.x, 0);
    }

    void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.3f))
        {
            //Debug.Log("Ledge Detected");
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.4f))
            {
                //Debug.Log("Ledge Climbable, adding force");
                rb.AddForce(Vector3.up * 20);
                //rb.velocity -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }
    }
    #endregion

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem is Weapon newWeapon)
        {
            if (newWeapon != null && newWeapon.equipSlot == EquipmentSlot.Right_Hand)
            {
                animator.SetLayerWeight(3, 1);
            }
            else if (newWeapon == null && oldItem.equipSlot == EquipmentSlot.Right_Hand)
            {
                animator.SetLayerWeight(3, 0);
            }

            if (newWeapon != null && newWeapon.equipSlot == EquipmentSlot.Left_Hand)
            {
                animator.SetLayerWeight(4, 1);
            }
            else if (newWeapon == null && oldItem.equipSlot == EquipmentSlot.Left_Hand)
            {
                animator.SetLayerWeight(4, 0);
            }
        }
    }

    public override void Die()
    {
        characterCombat.enabled = false;
        this.enabled = false;
    }
}