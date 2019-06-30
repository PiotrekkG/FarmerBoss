using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PaletteController : MonoBehaviour
{
    public Transform[] takePositions;
    public NavMeshObstacle navMeshObstacle;
    //public bool currentlyTaken = false;
    public bool canBeTaken = true;

    public Transform middleOnTop;
    public GameObject[] onTopObjectPrefabs;

    public int choosedPrefab = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (choosedPrefab == -1)
        {
            //if (Random.Range(0, 50) > 10)
            GameObject GO = Instantiate(onTopObjectPrefabs[Random.Range(0, onTopObjectPrefabs.Length)], middleOnTop.position, middleOnTop.rotation, middleOnTop);
            GO.tag = "PaletteObject";
        }
        else if (choosedPrefab >= 0)
        {
            GameObject GO = Instantiate(onTopObjectPrefabs[choosedPrefab], middleOnTop.position, middleOnTop.rotation, middleOnTop);
            GO.tag = "PaletteObject";
        }
    }
}
