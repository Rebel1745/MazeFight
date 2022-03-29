using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRedirector : MonoBehaviour
{

    PlayerInputAttack pa;
    PlayerInputMovement pm;

    // Start is called before the first frame update
    void Awake()
    {
        pa = GetComponentInParent<PlayerInputAttack>();
        pm = GetComponentInParent<PlayerInputMovement>();
    }
    
    public void FireRangedAttack()
    {
        pa.FireRangedPrefab();
    }

    public void TransitionToBall()
    {
        pm.ChangeStanceModel();
    }

    public void TransitionFromBall()
    {
        pm.FinishTranstionFromBall();
    }

    public void ScaleLeftArmPunch()
    {
        pa.UpdateAppendageScale("UpperArmLeftPunch");
        pa.UpdateAppendageScale("FistLeftPunch");
    }

    public void ScaleRightArmPunch()
    {
        pa.UpdateAppendageScale("UpperArmRightPunch");
        pa.UpdateAppendageScale("FistRightPunch");
    }

    /*
     * Not needed as sizing is called directly from spin function as there is no need to schedule it during animation
     * public void ScaleArmsSpin()
    {
        pa.UpdateAppendageScale("Spin");
    }*/
}
