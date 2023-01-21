using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform[] fishTanks;
    public GameObject pressEText;
    public GameObject BG;
    public float tankDistThres;
    Transform closestTank;
    GameObject displayFish;
    public float displayFishDistance;
    public float displayFishRotateSpeed;
    // Update is called once per frame
    void Update()
    {
        if(displayFish!=null)
        {
            if(Input.GetMouseButton(0))
            {
                float h = displayFishRotateSpeed * Input.GetAxis("Mouse X");
                float v = displayFishRotateSpeed * Input.GetAxis("Mouse Y");
                displayFish.transform.Rotate(v, h, 0);
            }
            return;
        }
        foreach(Transform tank in fishTanks)
        {
            float sqrDist = (transform.position - tank.position).sqrMagnitude;
            if(sqrDist < tankDistThres*tankDistThres)
            {
                closestTank = tank;
                pressEText.SetActive(true);
                break;
            }
            pressEText.SetActive(false);
        }
    }

    public void OnPressE()
    {
        transform.position += -transform.forward * 1.5f;
        BG.SetActive(true);
        pressEText.SetActive(false);
        displayFish = Instantiate(closestTank.GetComponent<TankFishSpawner>().spawnedCarpTransforms[0].GetChild(0).gameObject,closestTank);
        Transform camera = transform.parent.Find("MainCamera");
        displayFish.transform.position = camera.position + camera.forward * displayFishDistance;
        displayFish.transform.forward = -camera.right;
        // displayFish.transform.position += 0.2f * displayFish.transform.localScale.x * displayFish.transform.forward;
    }

    public void OnPressEReset()
    {
        BG.SetActive(false);
        Destroy(displayFish);
    }
}
