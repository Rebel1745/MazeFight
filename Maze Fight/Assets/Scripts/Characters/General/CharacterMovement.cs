using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool CanMove = true;
    public Vector3 LastLookDirection;

    public IEnumerator DisableMovementForTime(float time)
    {
        DisableMovement();
        yield return new WaitForSeconds(time);
        EnableMovement();
    }

    public void DisableMovement()
    {
        CanMove = false;
    }

    public void EnableMovement()
    {
        CanMove = true;
    }
}
