using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> monstersList = new List<GameObject>();
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string spawnSound = "minions_spawn";
    [SerializeField] private string goalID = "minions_spawn";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player and we haven't triggered yet
        if (!triggered && other.CompareTag(playerTag))
        {
            triggered = true;

            SoundManager.Instance.PlaySFX(spawnSound);

            // Activate all monsters by starting their movement
            for (int i = 0; i < monstersList.Count; i++)
            {
                if (monstersList[i] != null)
                {
                    MonsterPatrol patrol = monstersList[i].GetComponent<MonsterPatrol>();
                    if (patrol != null)
                    {
                        patrol.StartMovementFromPause();
                    }
                }
            }

            GoalManager.Instance.CompleteGoal(goalID);

            // Destroy this trigger so it only happens once
            Destroy(gameObject);
        }
    }
}
