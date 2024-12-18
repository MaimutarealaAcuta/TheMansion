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

    public void RotateTowardsAgent(Quaternion agentRotation)
    {
        rb.transform.rotation = agentRotation;
    }
}
