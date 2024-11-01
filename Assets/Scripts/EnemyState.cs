using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all enemy states
public abstract class EnemyState : MonoBehaviour
{
    public abstract EnemyState RunCurrentState(EnemyManager enemy);
}
