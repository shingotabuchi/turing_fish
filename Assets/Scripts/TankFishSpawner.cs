using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFishSpawner : MonoBehaviour
{
    public int carpSpawnCount;
    public GameObject[] carps;
    public TuringPatternThree turing;
    public List<Transform> spawnedCarpTransforms = new List<Transform>();
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < carpSpawnCount; i++)
        {
            GameObject newCarp = Instantiate(carps[Random.Range(0,carps.Length)],transform);
            Transform tank = transform.Find("Tank");
            newCarp.transform.position = tank.position + tank.localScale.y*5f*Vector3.up;
            newCarp.transform.rotation = Quaternion.Euler(0,Random.Range(0f,360f),0);
            spawnedCarpTransforms.Add(newCarp.transform);
            turing.renderList.Add(newCarp.transform.GetChild(0).GetChild(1).GetComponent<Renderer>());
        }
    }
}
