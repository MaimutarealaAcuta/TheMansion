using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    CharacterController rb;
    public float speed = 5f;
    public bool isMoving;

    void Start()
    {
        rb = GetComponent<CharacterController>();
    }

    void Update()
    {
        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");

        //if (isMoving)
        //{
        //    //Vector3 moveDir = new Vector3(1,0,0);
        //    Vector3 moveDir = transform.TransformDirection(Vector3.forward);
        //    rb.Move(moveDir*Time.deltaTime*speed);
        //    //transform.rotation = Quaternion.identity;
        //}
    }

    public void RotateTowardsAgent(Quaternion agentRotation)
    {
        rb.transform.rotation = agentRotation;
    }
}
