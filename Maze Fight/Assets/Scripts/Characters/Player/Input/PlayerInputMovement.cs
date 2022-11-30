using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    public GameManager gm;
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] PlayerController playerController;
    [SerializeField] AudioSource source;

    [Header("Movement")]
    public float WalkSpeed = 3f;
    public AudioClip WalkClip;
    public float RollSpeed = 5f;
    public float RollTimeout = 5f;
    float currentRollTime = 0f;
    public float RollCooldown = 3f;
    float currentRollCooldown = 0f;
    public AudioClip RollClip;
    public Vector2 MoveInput;
    Vector3 lookDirection;
    public Rigidbody rb;
    public float RotationSpeed = 500f;

    public bool isBodyStandard = true;
    public Transform BodyStandard;
    public Transform BodySphere;
    bool isTransforming = false;

    int currentCellNo = -1;
    public Transform CurrentFloor;
    public MazeCell CurrentCell;
    int currentRoom;

    #region LockOn
    Transform lockOnTarget = null;
    public float LockOnRange = 1f;
    public float LockOnCastWidthMax = 3f;
    public float LockOnCastWidthMin = 0.5f;
    public LayerMask WhatIsEnemy;
    #endregion

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        isBodyStandard = true;
        BodyStandard.gameObject.SetActive(true);
        BodySphere.gameObject.SetActive(false);
        isTransforming = false;
        lockOnTarget = null;

        StartCoroutine(characterMovement.DisableMovementForTime(2f));
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    public void LockOn(InputAction.CallbackContext context)
    {
        // if we are rolling, we can't lock on
        if (!isBodyStandard)
            return;

        // Cancel lock on if button is released
        if (context.canceled)
        {
            CancelLockOn();
            return;
        }

        if (!lockOnTarget)
        {
            LockOnToTarget();
        }
    }

    Transform FindTargetToLockOn()
    {
        Transform newTarget;

        // fire a raycast from the player in LastLookDirection, if it hits an enemy, lock on, if not, return null
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, LockOnCastWidthMax, characterMovement.LastLookDirection, out hit, LockOnRange, WhatIsEnemy))
        {
            //Debug.Log("Hit " + hit.transform.name);
            newTarget = hit.transform;
        }
        else
        {
            //Debug.Log("Checking SphereCastAll");
            // if the target is closer than LockOnCastWidth then it won't be detected so run a SphereCastAll to check for a hit
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, LockOnCastWidthMin, characterMovement.LastLookDirection, LockOnRange, WhatIsEnemy);

            if (hits.Length > 0)
            {
                //Debug.Log("Hit " + hits[0].transform.name);
                newTarget = hits[0].transform;
            }
            else
            {
                //Debug.Log("Miss");
                newTarget = null;
            }
        }

        return newTarget;
    }

    void LockOnToTarget()
    {
        // find new target
        lockOnTarget = FindTargetToLockOn();
        if(lockOnTarget)
            lockOnTarget.GetComponent<EnemyTargeted>().MarkAsTargeted();
    }

    void CancelLockOn()
    {
        //Debug.Log("Cancel Lock On");
        if(lockOnTarget)
            lockOnTarget.GetComponent<EnemyTargeted>().RemoveTarget();
        lockOnTarget = null;
    }

    public void UpdateFloor(Transform floor, int cellNo)
    {
        CurrentFloor = floor;
        currentCellNo = cellNo;
        CurrentCell = gm.mg.GetMazeCellFromInt(currentCellNo);

        if(currentRoom != CurrentCell.roomNo)
        {
            ChangeRoom(CurrentCell.roomNo);
        }
    }

    void ChangeRoom(int roomNo)
    {
        currentRoom = roomNo;
    }
    
    public void Movement(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    void DoMovement()
    {
        if (!characterMovement.CanMove)
            return;

        // if we have a target then look at it, otherwise look in the direction we are moving
        if (lockOnTarget)
        {
            lookDirection = lockOnTarget.position - transform.position;
        }
        else
        {
            lookDirection = new Vector3(MoveInput.x, 0f, MoveInput.y);
        }            

        // if the player is in the default state, move them slowly
        if (isBodyStandard)
        {
            rb.velocity = new Vector3(MoveInput.x, 0, MoveInput.y) * WalkSpeed;

            if (!playerController.playerInputAttack.isAttacking && !isTransforming)
            {
                if (MoveInput != Vector2.zero)
                {
                    playerController.ChangeAnimationState(playerController.PLAYER_WALK);
                }
                else
                {
                    playerController.ChangeAnimationState(playerController.PLAYER_IDLE);
                }
            }

            // update the roll cooldown
            if (currentRollCooldown < RollCooldown)
                currentRollCooldown += Time.deltaTime;
        } else // we be rolling
        {
            Roll();
        }

        if (MoveInput != Vector2.zero)
        {
            // removed to make turning instantaneous
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            // removed so that turning is instantaneous rather than gradual
            /*transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);*/

            transform.rotation = toRotation;            
            source.Play();

            // set this when moveinput is non-zero so we know which direction we are facing for knockback purposes
            characterMovement.LastLookDirection = lookDirection;
        }
    }

    void Roll()
    {
        currentRollTime += Time.deltaTime;

        if(currentRollTime < RollTimeout)
        {
            Vector3 force = new Vector3(MoveInput.x, 0f, MoveInput.y);

            rb.AddForce(force * RollSpeed);
        }
        else
        {
            ChangeStanceModel();
        }
    }

    public void ChangeStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isBodyStandard)
            {
                if(currentRollCooldown >= RollCooldown)
                {
                    isTransforming = true;
                    playerController.ChangeAnimationState(playerController.PLAYER_TO_BALL);
                    //ChangeStanceModel(); this is now called from the animation of turning into a ball
                }
            }
            else
            {
                ChangeStanceModel();
            }
        }
    }

    public void ChangeStanceModel()
    {
        if (isBodyStandard)
        {
            // set this to idle to 'reset' the animation state for when you exit the ball
            playerController.ChangeAnimationState(playerController.PLAYER_IDLE);
            isBodyStandard = false;
            BodyStandard.gameObject.SetActive(false);
            BodySphere.gameObject.SetActive(true);
            source.clip = RollClip;
            source.pitch = 1f;
            CancelLockOn();
            currentRollTime = 0f;
        }
        else
        {
            source.Pause();
            source.clip = null;
            isBodyStandard = true;
            BodyStandard.gameObject.SetActive(true);
            BodySphere.gameObject.SetActive(false);
            playerController.ChangeAnimationState(playerController.PLAYER_FROM_BALL);
            currentRollCooldown = 0f;
        }
    }

    public void FinishTranstionFromBall()
    {
        isTransforming = false;
    }

    public void PlayFootstep()
    {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(WalkClip);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LockOnCastWidthMin);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, LockOnCastWidthMax);
    }*/
}
