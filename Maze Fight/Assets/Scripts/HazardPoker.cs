using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardPoker : MonoBehaviour
{
    public Animator anim;
    public float InitialDelay = 0f;
    float currentDelay;
    public float TimeToStayDown = 2f;
    float currentTimeDown;
    public float RetractSpeed = 1f;
    public float TimeToStayUp = 1f;
    float currentTimeUp;
    public float ExtendSpeed = 5f;
    bool isPokerUp = false;

    void Start()
    {
        currentDelay = InitialDelay;
        currentTimeDown = TimeToStayDown;
        currentTimeUp = TimeToStayUp;
        isPokerUp = false;
    }

    void Update()
    {
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if (isPokerUp)
        {
            currentTimeUp -= Time.deltaTime;
            if (currentTimeUp <= 0)
            {
                RetractPoker();
            }
        }
        else
        {
            if (currentDelay > 0)
            {
                currentDelay -= Time.deltaTime;
            }
            else
            {
                currentTimeDown -= Time.deltaTime;
                if (currentTimeDown <= 0)
                {
                    ExtendPoker();
                }
            }
        }
    }

    void ExtendPoker()
    {
        currentTimeDown = TimeToStayDown;
        isPokerUp = true;
        anim.speed = ExtendSpeed;
        anim.Play("PokerExtend");
    }

    void RetractPoker()
    {
        currentTimeUp = TimeToStayUp;
        isPokerUp = false;
        anim.speed = RetractSpeed;
        anim.Play("PokerRetract");

    }
}
