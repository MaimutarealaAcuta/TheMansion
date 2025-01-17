using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public static FlockManager FM;
    public GameObject batPrefab;
    public int batsNumber = 20;
    public GameObject[] bats;
    public Vector3 flyLimits = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 goalPosition = Vector3.zero;

    [Header("Bat Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed;
    [Range(0.0f, 5.0f)] public float maxSpeed;
    [Range(1.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 5.0f)] public float rotationSpeed;

    [SerializeField] private bool goAroundPlayer;
    [SerializeField] private Transform player;

    private Vector3 initialPosition;

    void Start()
    {

        bats = new GameObject[batsNumber];

        for (int i = 0; i < batsNumber; ++i)
        {

            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                Random.Range(-flyLimits.y, flyLimits.y),
                Random.Range(-flyLimits.z, flyLimits.z));

            bats[i] = Instantiate(batPrefab, pos, Quaternion.identity);
            Animator batAnimator = bats[i].GetComponentInChildren<Animator>();

            //Change walk offset parameter so the agents' steps varies
            batAnimator.SetFloat("walkOffset", Random.Range(0.0f, 2.0f));

            //Change the speed of the walking
            float speedMult = Random.Range(0.5f, 1.5f);
            batAnimator.SetFloat("speedMultiplier", speedMult);

        }

        FM = this;
        goalPosition = this.transform.position;

        initialPosition = transform.position;
    }


    void Update()
    {

        if (Random.Range(0, 100) < 10)
        {

            goalPosition = player.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                Random.Range(-flyLimits.y, flyLimits.y),
                Random.Range(-flyLimits.z, flyLimits.z));
        }

        if (goAroundPlayer)
        {
            transform.position = new Vector3(player.position.x, player.position.y*2, player.position.z);
        }
        else
        {
            transform.position = initialPosition;
        }
    }
}
