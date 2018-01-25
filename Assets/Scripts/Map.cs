using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class Map : MonoBehaviour
{

    private int width;
    private int height;
    private JSONArray data;
    public GameObject[] blockPrefabs;
    public GameObject blockCollisionPrefab;
    public GameObject alphaWallPrefab;
    public string path;
    public GameController gameController;

    void Start()
    {
        StreamReader reader = new StreamReader(path);

        var json = JSON.Parse(reader.ReadToEnd());
        data = json["layers"][0]["data"].AsArray;
        width = json["layers"][0]["width"].AsInt;
        height = json["layers"][0]["height"].AsInt;

        reader.Close();

        build();
    }

    public void build()
    {
        for (int y = 0; y < height; y++)
        {
            int collisionStartX = -1;
            for (int x = 0; x < width; x++)
            {
                int blockId = data[y * width + x].AsInt;
                if (blockId != 0)
                {
                    GameObject prefab = blockPrefabs[blockId - 1];
                    if (prefab.tag == "Actor")
                        Instantiate(prefab, new Vector3(x + 1, -y - 0.5f, -0.125f), Quaternion.Euler(0, 90, 0), null);
                    else
                    {
                        GameObject block = Instantiate(prefab, new Vector3(x + 1, -y - 0.5f, -0.5f), Quaternion.identity, transform);

                        if (block.GetComponent<Renderer>().materials.Length > 1)
                        {
                            block.GetComponent<Renderer>().materials = new Material[] { block.GetComponent<Renderer>().materials[(y == 0 || data[(y - 1) * width + x].AsInt != blockId ? 0 : 1)] };
                        }
                        
                        block.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(gameController.maxTimeSteps / 4.0f, 1));
                    }

                    if (collisionStartX == -1 && blockId > 3)
                        collisionStartX = x;
                }
                if (collisionStartX != -1 && (x == width - 1 || data[y * width + x + 1] < 4))
                {
                    Instantiate(blockCollisionPrefab, new Vector3(collisionStartX + 1 + (x - collisionStartX) / 2.0f, -y - 0.5f, -0.5f), Quaternion.identity, transform).transform.localScale = new Vector3((x - collisionStartX + 1), 1, 1);
                    collisionStartX = -1;
                }
            }
        }
        
        Instantiate(alphaWallPrefab, new Vector3(0, -height, 0), Quaternion.identity).transform.localScale = new Vector3(width + 1, height + 5, 1);
        Instantiate(alphaWallPrefab, new Vector3(0, -height, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(gameController.maxTimeSteps / 4.0f, height + 5, 1);
        Instantiate(alphaWallPrefab, new Vector3(width + 0.5f, -height, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(gameController.maxTimeSteps / 4.0f, height + 5, 1);
        Instantiate(alphaWallPrefab, new Vector3(0, -height, -gameController.maxTimeSteps / 4.0f - 0.5f), Quaternion.identity).transform.localScale = new Vector3(width + 1, height + 5, 1);

        transform.localScale = new Vector3(1, 1, gameController.maxTimeSteps / 4.0f);
        //transform.Find("Finish").GetComponent<Renderer>().material.mainTextureScale = new Vector2(maxTimeSteps / 4.0f, 1);

    }


}
