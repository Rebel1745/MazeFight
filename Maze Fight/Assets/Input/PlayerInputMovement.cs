using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    public GameManager gm;
    [SerializeField] PlayerController playerController;
    [SerializeField] AudioSource source;

    [Header("Movement")]
    private bool canMove = true;
    public float WalkSpeed = 3f;
    public AudioClip WalkClip;
    public float RollSpeed = 5f;
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
    }

    private void FixedUpdate()
    {
        DoMovement();
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
        if (!canMove)
            return;

        lookDirection = new Vector3(MoveInput.x, 0f, MoveInput.y);

        // if the player is in the default state, move them slowly
        if (isBodyStandard)
        {
            rb.velocity = new Vector3(MoveInput.x, rb.velocity.y, MoveInput.y) * WalkSpeed;

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
            
        } else // we be rolling
        {
            Vector3 force = new Vector3(MoveInput.x, 0f, MoveInput.y);

            rb.AddForce(force * RollSpeed);
        }

        if (MoveInput != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

            source.Play();
        }
    }

    public IEnumerator DisableMovementForTime(float time)
    {
        DisableMovement();
        yield return new WaitForSeconds(time);
        EnableMovement();
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void ChangeStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isBodyStandard)
            {
                isTransforming = true;
                playerController.ChangeAnimationState(playerController.PLAYER_TO_BALL);
                //ChangeStanceModel();
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
        }
        else
        {
            source.Pause();
            source.clip = null;
            isBodyStandard = true;
            BodyStandard.gameObject.SetActive(true);
            BodySphere.gameObject.SetActive(false);
            playerController.ChangeAnimationState(playerController.PLAYER_FROM_BALL);
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
}
