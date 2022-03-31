using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CharacterController
{
    [HideInInspector] public EnemyCombat enemyCombat;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public Transform playerTargetCenter;
    [HideInInspector] public Vector3 lastPlayerPosition = Vector3.zero;
    [HideInInspector] public NavMeshAgent agent;

    #region - Player Detection -
    [Header("Player Detection")]
    public bool isHostile;
    [Range(40, 90)] [SerializeField] float fieldOfView = 60f;
    [SerializeField] float sightDistance = 25f;
    [SerializeField] Transform eyes;
    //[SerializeField] Transform head; //Not currently in use, later make function to turn head towards player
    [HideInInspector] public bool playerDetected = false;
    public LayerMask detectionLayers;
    #endregion

    #region - NPC Behavior -
    [Header("Behavior")]
    public BehaviorType behaviorType;
    BehaviorType defaultBehaviorType;
    float defaultStoppingDistance = 1.5f;

    Vector3 startingPosition;
    Quaternion startingRotation;
    float returnToStartTime = 15f;
    bool resetPosition = false;

    [Header("Patrol/Wandering")]
    [Range(20, 100)] public float wanderRadius;
    public bool controlledWander = true;
    Vector3 wanderPoint;
    bool waiting = false;
    public float minWaitTime = 20f;
    public float maxWaitTime = 90f;

    public Transform[] patrolWaypoints;
    int waypointIndex;
    Transform waypointTarget;

    [HideInInspector] public bool soundDetected = false;
    [HideInInspector] public Vector3 soundSourcePosition;
    bool hasEngaged = false;
    bool moveRight = false;
    bool moveLeft = false;
    [Space]
    public bool debugging = false;
    Color rayColor;
    #endregion
    
    protected override void Start()
    {
        base.Start();

        playerTarget = PlayerManager.instance.player.transform;
        playerTargetCenter = PlayerManager.instance.playerCenter.transform;
        agent = GetComponent<NavMeshAgent>();
        enemyCombat = GetComponent<EnemyCombat>();

        defaultBehaviorType = behaviorType;
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        if (behaviorType == BehaviorType.Patrolling)
        {
            if (patrolWaypoints == null)
            {
                Debug.Log("ERROR: WAYPOINTS NOT FOUND");
                behaviorType = BehaviorType.Idle;
            }
            IterateWaypointIndex();
            UpdateDestination();
        }
        else if (behaviorType == BehaviorType.Wandering)
        {
            IterateWanderLocation();
            UpdateDestination();
        }
    }

    public override void Update()
    {
        base.Update();

        #region - Player Detection -
        //The character's field of view
        Vector3 playerDir = (playerTargetCenter.position - eyes.position);
        float angle = Vector3.Angle(playerDir, eyes.forward);

        RaycastHit hit;
        Ray ray = new Ray(eyes.position, playerDir);

        if (Physics.Raycast(ray, out hit, sightDistance, detectionLayers))
        {
            PlayerController player = hit.collider.GetComponentInParent<PlayerController>();
            if (player != null && angle <= fieldOfView)
            {
                playerDetected = true;
            }
        }
        else
        {
            //The player is not in the character's field of view
            playerDetected = false;
        }

        if (playerDetected)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(eyes.position, playerDir, rayColor);
        #endregion

        //Determine character's actions
        switch (behaviorType)
        {
            case BehaviorType.Idle: //Default behavior type
                {
                    Idle();
                    break;
                }
            case BehaviorType.Patrolling: //Default behavior type
                {
                    Patrol();
                    break;
                }
            case BehaviorType.Wandering:
                {
                    Wander();
                    break;
                }
            case BehaviorType.Investigating: //The enemy heard something
                {
                    Investigate();
                    break;
                }
            case BehaviorType.Searching: //The enemy took damage
                {
                    Search();
                    break;
                }
            case BehaviorType.Chasing: //The player has been spotted
                {
                    Chase();
                    break;
                }
            case BehaviorType.Attacking: //The player is detected and the character is engaging
                {
                    Engage();
                    break;
                }
        }

       //Reset initial rotation of character //can I move this to Idle?
        if (resetPosition == true)
        {
            if (Vector2.Distance(transform.position, startingPosition) <= 0.1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, startingRotation, Time.deltaTime * 5f);
                if (transform.rotation == startingRotation)
                {
                    resetPosition = false;
                }
            }
            
        }

        #region - Strafing -
        if (moveRight)
        {
            float newX = transform.position.x + 2;
            agent.SetDestination(new Vector3(newX, transform.position.y, transform.position.z));
            agent.speed = walkSpeed;
            agent.angularSpeed = 0;
            animator.SetFloat("Horizontal", 1f);
            animator.SetFloat("Vertical", 0);
        }
        if (moveLeft)
        {
            float newX = transform.position.x - 2;
            agent.SetDestination(new Vector3(newX, transform.position.y, transform.position.z));
            agent.speed = walkSpeed;
            agent.angularSpeed = 0;
            animator.SetFloat("Horizontal", -1);
            animator.SetFloat("Vertical", 0);
        }
        #endregion
    }

    public override void AnimationsControl()
    {
        base.AnimationsControl();

        float speedPercent = agent.velocity.magnitude / sprintSpeed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
        //animator.SetFloat("Horizontal", agent.velocity.z);
        //animator.SetFloat("Vertical", agent.velocity.x);
    }

    private void LateUpdate()
    {
        //Don't implement this until you can figure out how to make the transition smooth. It's too snappy right now
        /*
        if (playerDetected )
        {
            head.LookAt(playerTarget);
        }
        */
    }

    #region - Behaviors -
    void Idle()
    {
        //if the player is seen
        if (playerDetected && isHostile)
        {
            behaviorType = BehaviorType.Chasing;
        }
        //if the character hears something
        else if (soundDetected)
        {
            behaviorType = BehaviorType.Investigating;
        }
    }
    void Patrol()
    {
        agent.speed = burdenedSpeed;
        agent.stoppingDistance = defaultStoppingDistance;

        //The enemy has reached their waypoint
        if (Vector3.Distance(transform.position, waypointTarget.position) < 1.6)
        {

            IterateWaypointIndex();
            StartCoroutine(UpdateDestinationDelay());
        }

        if (playerDetected && isHostile)
        {
            behaviorType = BehaviorType.Chasing;
        }
        else if (soundDetected)
        {
            behaviorType = BehaviorType.Investigating;
        }

    }
    void Wander()
    {
        agent.speed = burdenedSpeed;
        agent.stoppingDistance = defaultStoppingDistance;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waiting == false)
                StartCoroutine(UpdateDestinationDelay());
        }

        //I need to create a new position when the last one is reached, but not set that destination until after the dealy
    }
    void Investigate()
    {
        FaceSoundSource();
        agent.SetDestination(soundSourcePosition);

        if (playerDetected)
        {
            behaviorType = BehaviorType.Chasing;
        }

        if (Vector3.Distance(transform.position, soundSourcePosition) <= 1 && !playerDetected)
        {
            //Add coroutine or function set to rotate left and right to scan the area
            StartCoroutine(ReturnToPreviousBehaviour());
        }
    }
    void Search()
    {
        agent.speed = runSpeed;
        agent.stoppingDistance = defaultStoppingDistance;

        if (playerDetected)
        {
            behaviorType = BehaviorType.Chasing;
        }
        else if (soundDetected)
        {
            behaviorType = BehaviorType.Investigating;
        }
        else if (lastPlayerPosition != Vector3.zero)
        {
            agent.SetDestination(lastPlayerPosition);
            FaceDamageSource();
            if (Vector3.Distance(transform.position, lastPlayerPosition) > agent.stoppingDistance)
            {
                //Include function to scan the area.
            }
            StartCoroutine(LostSightOfPlayer());
        }
        else
        {
            StartCoroutine(ReturnToPreviousBehaviour());
        }
    }
    void Chase()
    {
        FaceTarget();
        agent.SetDestination(playerTarget.position);
        float distance = Vector3.Distance(playerTarget.position, transform.position);
        //Set agent stopping distance based on weaponry
        agent.stoppingDistance = enemyCombat.weaponReach;

        //Increase speed for first engagement, then return to runSpeed to allow player to escape if needed
        if (!hasEngaged && distance > agent.stoppingDistance * 2)
        {
            agent.speed = sprintSpeed;
        }
        else
        {
            agent.speed = runSpeed;
        }

        soundDetected = false;

        if (enemyCombat.leftHand == HandState.Shield)
        {
            float aimAngle = Mathf.Abs(Vector3.Angle(playerTargetCenter.forward, transform.forward) - 180);
            if (PlayerCombat.instance.bowDrawn && aimAngle < 32 && aimAngle > 26)
            {
                animator.SetBool("Blocking", true);
                agent.speed = walkSpeed;
            }
            else
            {
                animator.SetBool("Blocking", false);
            }
        }

        if (distance <= agent.stoppingDistance)
        {
            behaviorType = BehaviorType.Attacking;
        }

        TrackPlayer();

        if (!playerDetected)
        {
            StartCoroutine(ChaseGhosts());
        }
    }
    void Engage()
    {
        FaceTarget();
        float distance = Vector3.Distance(playerTarget.position, transform.position);
        soundDetected = false;
        //The player has charged the enemy, they should switch out their bow for a melee weapon
        if (enemyCombat.hasBow && distance < 2 && enemyCombat.sideArm != null) //&& enemyCombat.hasMelee)
        {
            enemyCombat.SwitchToMelee();
            agent.stoppingDistance = enemyCombat.weaponReach;
        }

        if (playerDetected)
        {
            TrackPlayer();
        }

        //The enemy is close enough to attack the player and there is a clear line of sight
        if (playerDetected && distance <= agent.stoppingDistance)
        {
            enemyCombat.Attack_Primary();
            if (!hasEngaged)
                hasEngaged = true;
        }
        else behaviorType = BehaviorType.Chasing;
    }
    #endregion

    #region - Patrolling & Wandering -
    //Set agent destination while patrolling
    private void UpdateDestination()
    {
        if (behaviorType == BehaviorType.Patrolling)
        {
            agent.SetDestination(waypointTarget.position);
        }
        else if (behaviorType == BehaviorType.Wandering)
        {
            IterateWanderLocation();
            agent.SetDestination(wanderPoint);
            waiting = false;
        }
    }

    //Update to next destination while patrolling
    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == patrolWaypoints.Length)
        {
            waypointIndex = 0;
        }
        waypointTarget = patrolWaypoints[waypointIndex];
    }

    //Update to next destination while wandering
    public void IterateWanderLocation()
    {
        Vector3 randomPosition = Random.insideUnitSphere * wanderRadius;
        if (controlledWander)
        {
            randomPosition += startingPosition;
        }
        else
        {
            randomPosition += transform.position;
        }
        
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, wanderRadius, 1))
        {
            if (Vector3.Distance(hit.position, transform.position) > 2)
            {
                wanderPoint = hit.position;
            }
        }
    }

    IEnumerator UpdateDestinationDelay()
    {
        waiting = true;
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        UpdateDestination();
    }
    #endregion

    #region - Character Facing -
    //Rotate the face the player if they run around them
    void FaceTarget() //update this to accept a Transform transform?
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //Turn the NPC towards a sound that they heard
    public void FaceSoundSource()
    {
        Vector3 soundDirection = (soundSourcePosition - transform.position).normalized;
        Quaternion turnRotation = Quaternion.LookRotation(new Vector3(soundDirection.x, 0, soundDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, turnRotation, Time.deltaTime * 5f);
        StopAllCoroutines();
    }

    public void DamageTaken()
    {
        soundDetected = false;

        float lastX = playerTarget.position.x;
        float lastY = playerTarget.position.y;
        float lastZ = playerTarget.position.z;
        lastPlayerPosition = new Vector3(lastX, lastY, lastZ);
        behaviorType = BehaviorType.Searching;
    }

    //Turn the NPC towards a damage source they received
    public void FaceDamageSource()
    {
        Vector3 direction = (lastPlayerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    #endregion

    #region - Player Tracking and Chasing -
    //Constantly update last known location of player
    void TrackPlayer()
    {
        float lastX = playerTarget.position.x;
        float lastY = playerTarget.position.y;
        float lastZ = playerTarget.position.z;
        lastPlayerPosition = new Vector3(lastX, lastY, lastZ);
    }

    //Extends the amount of time that the enemy will chase the player
    IEnumerator ChaseGhosts()
    {
        if (playerDetected)
            yield break;

        yield return new WaitForSeconds(5f);

        if (playerDetected)
            yield break;
        
        behaviorType = BehaviorType.Searching;
    }

    //Resets the enemy's behaviour to its default state
    IEnumerator ReturnToPreviousBehaviour()
    {
        if (characterStats.isDead)
            yield break;

        soundDetected = false;

        yield return new WaitForSeconds(returnToStartTime);

        if (characterStats.isDead)
            yield break;
        Debug.Log("Returning to Previous Behavior");

        //set behavior type back to what it was at the start
        behaviorType = defaultBehaviorType;
        soundSourcePosition = Vector3.zero; 
        lastPlayerPosition = Vector3.zero;

        //return NPC to original position
        if (defaultBehaviorType == BehaviorType.Idle)
        {
            agent.SetDestination(startingPosition);
            resetPosition = true;
        }
        else if (defaultBehaviorType == BehaviorType.Patrolling)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }
        else if (defaultBehaviorType == BehaviorType.Wandering)
        {
            IterateWanderLocation();
            UpdateDestination();
        }
    }

    //Removes last position of player
    IEnumerator LostSightOfPlayer()
    {
        yield return new WaitForSeconds(10f);
        lastPlayerPosition = Vector3.zero;
    }
    #endregion

    //Cause the NPC to flee when their HP is low
    public void RunAway()
    {
        Debug.Log(transform.name + " is fleeing");
    }

    public override void Die()
    {
        base.Die();
        agent.enabled = false;
    }
}

public enum BehaviorType { Idle, Patrolling, Wandering, Investigating, Searching, Chasing, Attacking }

//Include detection for corpses