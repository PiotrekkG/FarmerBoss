using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject emptyTilePrefab;

    public Tile[] tilesInfo = new Tile[25];
    public int inRow = 5;


    void Start()
    {
        for (int id = 0; id < tilesInfo.Length; id++)
        {
            Tile tile;
            int posRow = (id % inRow) * 10, posColumn = (int)(id / inRow) * 15;

            if (tilesInfo[id] != null)
            {
                tile = tilesInfo[id];
                Debug.Log(tile.name);

                GameObject GO = Instantiate(tile.gameObject, new Vector3(transform.position.x + posRow, transform.position.y, transform.position.z + posColumn), Quaternion.identity, transform);
                GO.transform.Rotate(0, 90 * Random.Range(0, 4), 0);

                tile.id = id;
            }
            else
            {
                GameObject GO = Instantiate(emptyTilePrefab, new Vector3(transform.position.x + posRow, transform.position.y, transform.position.z + posColumn), Quaternion.identity, transform);
                GO.transform.Rotate(0, 90 * Random.Range(0, 4), 0);

                tile = GO.GetComponent<Tile>();
                tilesInfo[id] = tile;

                tile.id = id;
            }
        }
    }
}