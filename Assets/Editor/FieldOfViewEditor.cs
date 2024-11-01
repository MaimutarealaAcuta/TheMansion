using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Anything in Editor folder won't be compiled in final build

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        //gets a reference to FieldOfView script
        FieldOfView fov = (FieldOfView)target;

        //draws circle around enemy based on the radius from FieldOfView script
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position + new Vector3(0, fov.rayHeight,0), Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        //draws the 2 lines which forms the field of view angle in front of the enemy
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);


        //draws a green line to the player if the player is in the field of view
        if (fov.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position + new Vector3(0, fov.rayHeight, 0), fov.playerRef.transform.position + new Vector3(0, fov.rayHeight, 0));
        }
        else
        {
                Handles.color = Color.red;
                Handles.DrawLine(fov.transform.position + new Vector3(0, fov.rayHeight, 0), fov.playerRef.transform.position + new Vector3(0, fov.rayHeight, 0));
            
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
