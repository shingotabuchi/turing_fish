using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFishBehaviour : MonoBehaviour
{
    public float sprintForce;
    public float sprintMinTime;
    public float sprintMaxTime;
    public float sprintProbability;
    public float sprintInterval;
    public float horizontalTurnTorque;
    public float horizontalTurnMinTime;
    public float horizontalTurnMaxTime;
    public float horizontalTurnInterval;
    public float horizontalTurnProbability;
    public float turn180Probability;
    public float turn180Time;
    public float turn180Dist;
    public float levelSmoothTime;
    public Animator animator;
    public float animatorMaxSpeed;
    public float animatorMinSpeed;
    public float animatorAccel;
    float zRotVelocity = 0;
    float xRotVelocity = 0;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Sprint());
        StartCoroutine(HorizontalTurn());
    }

    private void FixedUpdate() 
    {
        float zAngle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, 0, ref zRotVelocity, levelSmoothTime);
        float xAngle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, 0, ref xRotVelocity, levelSmoothTime);
        transform.rotation = Quaternion.Euler(xAngle,transform.rotation.eulerAngles.y,zAngle);
        if(animator.speed > animatorMinSpeed) animator.speed -= animatorAccel*Time.deltaTime/2f;
    }   

    IEnumerator HorizontalTurn()
    {
        float rnd;
        while(true)
        {
            rnd = Random.Range(0f,1f);
            if(rnd <= horizontalTurnProbability)
            {
                StartCoroutine(HorizontalTurnForce());
            }
            // yield return new WaitForSeconds(horizontalTurnInterval);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator HorizontalTurnForce()
    {
        float timer = 0f;
        float limitTime = Random.Range(horizontalTurnMinTime,horizontalTurnMaxTime);
        float leftOrRight = 1f-Random.Range(0,2)*2f; 
        // Debug.DrawRay(transform.position, transform.forward*turn180Dist, Color.blue,1f);
        if(Physics.Raycast(transform.position, transform.forward, turn180Dist*transform.localScale.z, 1 << LayerMask.NameToLayer("TankWall")))
        {
            if(Random.Range(0f,1f) <= turn180Probability) limitTime = turn180Time;
        }
        while(timer < limitTime)
        {
            if(animator.speed < animatorMaxSpeed) animator.speed += animatorAccel*Time.deltaTime;
            rb.AddTorque(transform.up * horizontalTurnTorque * leftOrRight);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Sprint()
    {
        float rnd;
        while (true)
        {
            rnd = Random.Range(0f,1f);
            if(rnd <= sprintProbability)
            {
                StartCoroutine(SprintForce());
            }
            // yield return new WaitForSeconds(sprintInterval);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SprintForce()
    {
        float limitTime = Random.Range(sprintMinTime,sprintMaxTime);
        float timer = 0f;
        while(timer < limitTime)
        {
            if(animator.speed < animatorMaxSpeed) animator.speed += animatorAccel*Time.deltaTime;
            rb.AddForce(sprintForce * transform.forward);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
