using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    public GameManager gm;
    [SerializeField] PlayerController playerController;

    [Header("Movement")]
    private bool canMove = true;
    public float WalkSpeed = 3f;
    public float RollSpeed = 5f;
    internal float currentSpeed;
    public Vector2 moveInput;
    public Rigidbody rb;
    public float RotationSpeed = 500f;

    public bool isBodyStandard = true;
    public Transform BodyStandard;
    public Transform BodySphere;

    int currentCellNo = -1;
    public Transform CurrentFloor;
    public MazeCell CurrentCell;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        currentSpeed = WalkSpeed;
        isBodyStandard = true;
        BodyStandard.gameObject.SetActive(true);
        BodySphere.gameObject.SetActive(false);
    }

    private void Update()
    {
        DoMovement();
    }

    public void UpdateFloor(Transform floor, int cellNo)
    {
        CurrentFloor = floor;
        currentCellNo = cellNo;
        CurrentCell = gm.mg.GetMazeCellFromInt(currentCellNo);
    }

     // TODO: fix player movement when rolling and sort the camera
    public void Movement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void DoMovement()
    {
        if (!canMove)
            return;

        Vector3 lookDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // if the player is in the default state, move them slowly
        if (isBodyStandard)
        {
            rb.velocity = new Vector3(moveInput.x * currentSpeed, rb.velocity.y, moveInput.y * currentSpeed);

            if (moveInput != Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

                playerController.ChangeAnimationState(playerController.PLAYER_WALK);
            }
            else
            {
                playerController.ChangeAnimationState(playerController.PLAYER_IDLE);
            }
        } else // we be rolling
        {
            Vector3 force = new Vector3(moveInput.x, 0f, moveInput.y);
            transform.LookAt(lookDirection);
            
            rb.AddForce(force * RollSpeed);
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
                //playerController.ChangeAnimationState(playerController.PLAYER_TO_BALL);
                ChangeStanceModel();
            }
            else
            {
                //playerController.ChangeAnimationState(playerController.PLAYER_FROM_BALL);
                ChangeStanceModel();
            }
        }
    }

    public void ChangeStanceModel()
    {
        if (isBodyStandard)
        {
            isBodyStandard = false;
            BodyStandard.gameObject.SetActive(false);
            BodySphere.gameObject.SetActive(true);
        }
        else
        {
            isBodyStandard = true;
            BodyStandard.gameObject.SetActive(true);
            BodySphere.gameObject.SetActive(false);
        }
    }
}
