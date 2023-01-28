using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public enum MovementMode
    {
        Swim,
        Rest,
        TurnRight,
        TurnLeft,
        SwimUp,
        SwimDown,
        Transition,
    }
    public static FishSpawner spawner;
    public MovementMode movementMode;
    private void Start() 
    {
        movementMode = MovementMode.Swim;
    }

    private void Update() 
    {
        switch (movementMode)
        {
            case MovementMode.Swim:
            transform.position += transform.forward * spawner.fishSpeed * Time.deltaTime;
            break;
        }

        PerodicBoundaryCondition();
    }

    void PerodicBoundaryCondition()
    {
        Vector3 diffVec = transform.position - spawner.floorPlane.transform.position;
        if(diffVec.x >  spawner.areaHalfWidth) transform.position -= new Vector3(spawner.areaHalfWidth*2f,0,0);
        if(diffVec.x < -spawner.areaHalfWidth) transform.position += new Vector3(spawner.areaHalfWidth*2f,0,0);
        if(diffVec.z >  spawner.areaHalfWidth) transform.position -= new Vector3(0,0,spawner.areaHalfWidth*2f);
        if(diffVec.z < -spawner.areaHalfWidth) transform.position += new Vector3(0,0,spawner.areaHalfWidth*2f);
        // if(diffVec.y < spawner.areaMinHeight) transform.position += new Vector3(0,0,spawner.areaHalfWidth*2f);
    }
}
