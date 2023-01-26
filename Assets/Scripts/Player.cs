using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform[] fishTanks;
    public GameObject pressEText;
    public GameObject BG;
    public float tankDistThres;
    public Transform closestTank;
    GameObject displayFish;
    public float displayFishDistance;
    public float displayFishRotateSpeed;
    public float displayFishScale;
    public float displayFishXOffset;
    // Update is called once per frame
    void Update()
    {
        // if(displayFish!=null)
        // {
        //     // if(Input.GetMouseButton(0))
        //     // {
        //     //     float h = displayFishRotateSpeed * Input.GetAxis("Mouse X");
        //     //     float v = displayFishRotateSpeed * Input.GetAxis("Mouse Y");
        //     //     displayFish.transform.Rotate(v, h, 0);
        //     // }

        //     return;
        // }
        // foreach(Transform tank in fishTanks)
        // {
        //     float sqrDist = (transform.position - tank.position).sqrMagnitude;
        //     if(sqrDist < tankDistThres*tankDistThres)
        //     {
        //         closestTank = tank;
        //         pressEText.SetActive(true);
        //         break;
        //     }
        //     pressEText.SetActive(false);
        // }

        if(Input.GetKeyDown(KeyCode.R))
        {
            closestTank.Find("Turing").GetComponent<TuringPatternThree>().Initialize();
        }
    }

    public void OnPressE()
    {
        transform.position += -transform.forward * 1.5f;
        BG.transform.Find("Sliders").GetComponent<TuringSettingSliders>().turing = closestTank.Find("Turing").GetComponent<TuringPatternThree>();
        BG.SetActive(true);
        pressEText.SetActive(false);
        displayFish = Instantiate(closestTank.GetComponent<TankFishSpawner>().spawnedCarpTransforms[0].GetChild(0).gameObject,closestTank);
        Transform camera = transform.parent.Find("MainCamera");
        displayFish.transform.position = camera.position + camera.forward * displayFishDistance;
        displayFish.transform.forward = -camera.right;
        displayFish.transform.position += displayFishXOffset * displayFish.transform.localScale.x * displayFish.transform.forward;
        displayFish.transform.localScale = displayFish.transform.localScale * displayFishScale;
    }

    public void OnPressEReset()
    {
        pressEText.SetActive(true);
        BG.SetActive(false);
        Destroy(displayFish);
    }
}
