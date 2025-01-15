using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Goal Trigger Settings")]
    [SerializeField] private string goalID = "EnterMaison"; // The ID of the goal to complete

    [Tooltip("Check this if the trigger should only fire once, then destroy itself or never fire again.")]
    [SerializeField] private bool oneTimeTrigger = true;

    private bool hasFired = false;

    private void Awake()
    {
        // Ensure our BoxCollider is set as a trigger
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            box.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered is the player
        // You can rely on a tag or a component check
        if (other.CompareTag("Player") && !hasFired)
        {
            // Call the GoalManager to complete the specified goal
            GoalManager.Instance.CompleteGoal(goalID);

            if (oneTimeTrigger)
            {
                hasFired = true;
                // Optionally, destroy this GameObject
                // Destroy(gameObject);
            }
        }
    }
}
