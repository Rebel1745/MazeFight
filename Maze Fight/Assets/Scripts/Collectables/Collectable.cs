using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Rigidbody rb;
    public Collider col;
    public float AttractDistance = 2f;
    public float MoveSpeed = 10f;
    public float InitialSpeed = 100f;
    bool isAttracted = false;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
        rb.isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        rb.AddForce(force * InitialSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
            return;

        if(Vector3.Distance(player.position, transform.position) <= AttractDistance || isAttracted)
        {
            col.isTrigger = true;
            rb.useGravity = false;
            isAttracted = true;
            Vector3 newPos = Vector3.MoveTowards(transform.position, player.position, MoveSpeed * Time.deltaTime);
            transform.position = newPos;
            //transform.position = Vector3.Lerp(transform.position, player.transform.position, MoveSpeed * Time.deltaTime);
        }
    }
}
