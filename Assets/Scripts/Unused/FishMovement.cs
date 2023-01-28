using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed;
    public int pathSearchResolution;
    public float viewRadius;
    public float rotateSpeed;
    public float smoothDampTime;
    Vector3 smoothDampVelocity;
    public float fov;
    public float obstacleCheckTime;
    Vector3 desiredDirection;
    float timer = 0f;
    bool checkedLastFrame = false;
    public string hitWallName = "";
    // Start is called before the first frame update
    void Start()
    {
        smoothDampVelocity = Vector3.zero;
        desiredDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // if(timer > obstacleCheckTime && transform.forward == desiredDirection)
        // {
        //     timer = 0f;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, viewRadius, 1 << LayerMask.NameToLayer("TankWall")))
        {
            
            if(hitWallName != hit.transform.gameObject.name)
            {
                CheckForObstacles();
                hitWallName = hit.transform.gameObject.name;
            }
        }
        else
        {
            hitWallName = "";
        }
        
        // }
        Debug.DrawRay(transform.position, desiredDirection*viewRadius, Color.blue,0.1f);
        // if(transform.forward == desiredDirection) CheckForObstacles();
        transform.forward = Vector3.SmoothDamp(transform.forward,desiredDirection,ref smoothDampVelocity,smoothDampTime);
        // transform.forward = Vector3.RotateTowards(transform.forward,desiredDirection,Mathf.PI*(rotateSpeed/180f)*Time.deltaTime,0f);
        // transform.forward = desiredDirection;
        transform.position += speed * transform.forward * Time.deltaTime;
        // timer += Time.deltaTime;
    }

    void CheckForObstacles(){
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));
        for (int i = 0; i < pathSearchResolution; i++)
        {
            Vector3 pathFindCoefficient = new Vector3(0,0,0);
            Vector3 upPerpendicular1 = new Vector3(0,0,0);
            Vector3 upPerpendicular2 = new Vector3(0,0,0);
            for (int j = 0; j < 3; j++)
            {
                if(transform.forward[j]!=0){
                    upPerpendicular1[j] = -transform.forward[(j+1)%3]/transform.forward[j];
                    upPerpendicular1[(j+1)%3] = 1;
                    upPerpendicular1[(j+2)%3] = 0;
                    
                    for (int k = 0; k < 3; k++)
                    {
                        upPerpendicular2[k] = transform.forward[(k+1)%3]*upPerpendicular1[(k+2)%3] - transform.forward[(k+2)%3]*upPerpendicular1[(k+1)%3];
                    }
                    upPerpendicular1.Normalize();
                    upPerpendicular2.Normalize();
                    break;
                }
            }
            pathFindCoefficient.y = 1f - (((float)i)/(pathSearchResolution - 1f)) * (1-Mathf.Cos(Mathf.PI*fov/180f));
            float radius = Mathf.Sqrt(1 - pathFindCoefficient.y * pathFindCoefficient.y);
            float theta = phi * i;
            pathFindCoefficient.x = Mathf.Cos(theta) * radius;
            pathFindCoefficient.z = Mathf.Sin(theta) * radius;

            Vector3 pathFindRay = transform.forward * pathFindCoefficient.y + upPerpendicular1 * pathFindCoefficient.x + upPerpendicular2 * pathFindCoefficient.z;
            Debug.DrawRay(transform.position, pathFindRay*viewRadius, Color.red,0.1f);
            if(!Physics.Raycast(transform.position, pathFindRay, viewRadius, 1 << LayerMask.NameToLayer("TankWall")) || i == pathSearchResolution-1){
                desiredDirection = pathFindRay.normalized;
            }
        }
    }
}
