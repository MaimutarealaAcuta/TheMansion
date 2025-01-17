using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    [Header("Goals Definition")]
    [SerializeField] private List<Goal> goals = new List<Goal>();

    [Header("UI Elements")]
    //[SerializeField] private TextMeshProUGUI goalTitleText;
    [SerializeField] private TextMeshProUGUI goalDescriptionText;

    [SerializeField] private string goalCompletedTask = "task_completed";

    private int currentGoalIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize all goals except the first as inactive
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].isActive = (i == 0);
        }

        // Display the first active goal
        UpdateGoalUI();
    }

    /// <summary>
    /// Call this when a goal is completed (via direct call or events).
    /// e.g. GoalManager.Instance.CompleteGoal("EnterMaison");
    /// </summary>
    public void CompleteGoal(string goalID)
    {
        // Find the goal in the list
        Goal goal = goals.Find(g => g.goalID == goalID && g.isActive == true);
        if (goal == null)
        {
            Debug.LogWarning($"GoalManager: Goal '{goalID}' not found or not active.");
            return;
        }

        if (!goal.isCompleted)
        {
            goal.isCompleted = true;
            Debug.Log($"Goal '{goalID}' completed!");

            SoundManager.Instance.PlaySFX(goalCompletedTask);

            // Move to the next goal in the list
            AdvanceToNextGoal();
        }
    }

    private void AdvanceToNextGoal()
    {
        // Mark current goal inactive
        goals[currentGoalIndex].isActive = false;

        // Move the index forward
        currentGoalIndex++;
        if (currentGoalIndex < goals.Count)
        {
            // Activate the next goal
            goals[currentGoalIndex].isActive = true;
            UpdateGoalUI();
        }
        else
        {
            // No more goals, or game is won
            Debug.Log("All goals completed. Game End?");
            // You might show a "Game Over" or "You Win" screen here
            HideGoalUI();
        }
    }

    private void UpdateGoalUI()
    {
        // Find the first active goal
        Goal currentGoal = goals.Find(g => g.isActive == true);
        if (currentGoal != null)
        {
            // Update the UI text
            //if (goalTitleText) goalTitleText.text = currentGoal.goalTitle;
            if (goalDescriptionText) goalDescriptionText.text = currentGoal.goalDescription;
        }
        else
        {
            HideGoalUI();
        }
    }

    private void HideGoalUI()
    {
        //if (goalTitleText) goalTitleText.text = "";
        if (goalDescriptionText) goalDescriptionText.text = "";
    }

    public void UpdateGoalDescription(string description)
    {
        goalDescriptionText.text = description;
    }
}
