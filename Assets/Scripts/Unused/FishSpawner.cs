using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public GameObject floorPlane;
    public float areaMaxHeight;
    public float areaMinHeight;
    public int spawnCount;
    public float fishSpeed;
    // public float fishAccel;
    // public float fishDeccel;
    // public float fish 
    public float areaHalfWidth;
    void Awake()
    {
        Fish.spawner = this;
        areaHalfWidth = floorPlane.transform.localScale.x * 5f;
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newFish = Instantiate(fishPrefabs[Random.Range(0,fishPrefabs.Length)],transform);
            newFish.transform.position = floorPlane.transform.position + new Vector3(
                Random.Range(-areaHalfWidth,areaHalfWidth),
                Random.Range(areaMinHeight,areaMaxHeight),
                Random.Range(-areaHalfWidth,areaHalfWidth)
            );
            newFish.transform.rotation = Quaternion.Euler(0,Random.Range(0f,360f),0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
