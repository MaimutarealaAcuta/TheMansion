using System.Collections.Generic;
using UnityEngine;

public class TestMonsterSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> monstersList = new List<GameObject>();
    [SerializeField] private string playerTag = "Player";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player and we haven't triggered yet
        if (!triggered && other.CompareTag(playerTag))
        {
            triggered = true;

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

            // Destroy this trigger so it only happens once
            Destroy(gameObject);
        }
    }
}
