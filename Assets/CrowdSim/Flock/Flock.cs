using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    private float speed;
    bool turning = false;

    void Start()
    {

        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }


    void Update()
    {

        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.flyLimits * 2.0f);

        if (!b.Contains(transform.position))
        {

            turning = true;
        }
        else
        {

            turning = false;
        }

        if (turning)
        {

            Vector3 direction = FlockManager.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {


            if (Random.Range(0, 100) < 10)
        {

            //print(speed);
            //print(FlockManager.FM.maxSpeed);
            speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            print("Updated Speed2: " + speed);
        }


        if (Random.Range(0, 100) < 10)
        {
            ApplyRules();
        }
        }

        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }

    private void ApplyRules()
    {

        GameObject[] bats = FlockManager.FM.bats;

        Vector3 groupCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float groupSpeed = 0.01f;
        float mDistance;
        int groupSize = 0;

        foreach (GameObject bat in bats)
        {

            if (bat != this.gameObject)
            {

                mDistance = Vector3.Distance(bat.transform.position, this.transform.position);
                if (mDistance <= FlockManager.FM.neighbourDistance)
                {

                    groupCentre += bat.transform.position;
                    groupSize++;

                    if (mDistance < 0.2f)
                    {

                        vAvoid = vAvoid + (this.transform.position - bat.transform.position);
                    }

                    Flock anotherFlock = bat.GetComponent<Flock>();
                    groupSpeed = groupSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {

            groupCentre = groupCentre / groupSize + (FlockManager.FM.goalPosition - this.transform.position);
            speed = groupSpeed / groupSize;
            print("Updated Speed3: " + speed);
            if (speed > FlockManager.FM.maxSpeed)
            {

                speed = FlockManager.FM.maxSpeed;
            }

            Vector3 direction = (groupCentre + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

