using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    //public Animator anim;
    public HealthAndDamage had;
    public float HealthRegen = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RegenHealth();
    }

    void RegenHealth()
    {
        //had.Heal(HealthRegen * Time.deltaTime);
    }
}
