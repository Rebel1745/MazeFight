using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    [Header("Movement")]
    private bool canMove = true;
    public float WalkSpeed = 3f;
    public float RollSpeed = 5f;
    internal float currentSpeed;
    Vector2 moveInput;
    public Rigidbody rb;
    public float RotationSpeed = 500f;

    bool isCube = true;
    public Transform BodyCube;
    public Transform BodySphere;

    [Space]
    [Header("Camera Follow")]
    public Transform FollowTarget;
    public float MaxLookaheadDistance = 2f;
    public float TargetMoveSpeed = 0.02f;
    public float BounceBackSpeed = 0.05f;

    private void Start()
    {
        currentSpeed = WalkSpeed;
        isCube = true;
        BodyCube.gameObject.SetActive(true);
        BodySphere.gameObject.SetActive(false);
    }

    private void Update()
    {
        DoMovement();
        UpdateCameraTargetLocation();
    }

    void UpdateCameraTargetLocation()
    {
        // if we aren't at our max distance for the target, move
        if (Vector3.Distance(transform.position, FollowTarget.transform.position) < MaxLookaheadDistance)
        {
            Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
            FollowTarget.transform.position += dir * TargetMoveSpeed;
        }

        // if we arent moving, move the follow target towards the player
        if (moveInput == Vector2.zero)
        {
            if (Vector3.Distance(transform.position, FollowTarget.transform.position) > 0)
            {
                Vector3 dir = transform.position - FollowTarget.transform.position;
                FollowTarget.transform.position += dir * BounceBackSpeed;
            }
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void DoMovement()
    {
        if (!canMove)
            return;

        // if the player is in the default state, move them slowly
        if (isCube)
        {
            Vector3 lookDirection = new Vector3(moveInput.x, 0f, moveInput.y);

            rb.velocity = new Vector3(moveInput.x * currentSpeed, rb.velocity.y, moveInput.y * currentSpeed);

            if (moveInput != Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);
            }
        } else // we be rolling
        {
            Vector3 force = new Vector3(moveInput.x, 0f, moveInput.y);

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
            isCube = !isCube;
            BodyCube.gameObject.SetActive(!BodyCube.gameObject.activeSelf);
            BodySphere.gameObject.SetActive(!BodySphere.gameObject.activeSelf);
        }
    }
}
