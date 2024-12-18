using System.Collections.Generic;
using UnityEngine;

public class TestMonsterSpawn : Interactable
{
    [SerializeField] List<GameObject> monstersList = new List<GameObject>();

    public override void OnInteract()
    {
        for (int i = 0; i < monstersList.Count; i++)
        {
            monstersList[i].GetComponent<MonsterPatrol>().StartMovementFromPause();
        }
    }

    public override void OnFocus()
    {
        
    }

    public override void OnLoseFocus()
    {
        
    }
}
