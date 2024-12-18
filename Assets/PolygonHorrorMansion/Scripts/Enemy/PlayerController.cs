using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController rb;
    [SerializeField] float speed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0, z);
        rb.Move(moveDir * Time.deltaTime*speed);
    }
}
