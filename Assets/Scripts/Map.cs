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
    public GameObject backgroundWallPrefab;
    public GameObject timeWallPrefab;
    public Texture[] BackgroundTextures;
    public string path;
    public GameController gameController;
    private GameObject backgroundWalls;
    private GameObject viewer;

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
                uint blockId = data[y * width + x].AsUInt;
                int rotation = 0;
                if (blockId >= 3221225472)
                {
                    blockId -= 3221225472;
                    rotation = 2;
                }
                if (blockId >= 2684354560)
                {
                    blockId -= 2684354560;
                    rotation = 1;
                }
                if (blockId >= 1610612736)
                {
                    blockId -= 1610612736;
                    rotation = 3;
                }

                if (blockId != 0)
                {
                    GameObject prefab = blockPrefabs[blockId - 1];
                    GameObject block;
                    if (prefab.tag == "Actor")
                        block = Instantiate(prefab, new Vector3(x + 1, -y - 0.5f, -0.125f), Quaternion.Euler(90 * rotation, 90, 0), null);
                    else
                    {
                        block = Instantiate(prefab, new Vector3(x + 1, -y - 0.5f, -0.5f), Quaternion.identity, transform);

                        if (block.GetComponent<Renderer>().materials.Length > 1)
                        {
                            block.GetComponent<Renderer>().materials = new Material[] { block.GetComponent<Renderer>().materials[(y == 0 || data[(y - 1) * width + x].AsInt != blockId ? 0 : 1)] };
                        }
                        
                        block.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(gameController.maxTimeSteps / 4.0f, 1));
                    }
                    block.name = prefab.name + (blockId == 1 ? "" : "-" + x + "-" + y);

                    if (collisionStartX == -1 && blockId == 4)
                        collisionStartX = x;
                }
                if (collisionStartX != -1 && (x == width - 1 || data[y * width + x + 1] != 4))
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

        backgroundWalls = new GameObject("BackgroundWalls");
        int zoffset = 0;
        foreach (Texture texture in BackgroundTextures)
        {
            GameObject newWall = Instantiate(backgroundWallPrefab, new Vector3(0.5f, -15, zoffset), Quaternion.identity, backgroundWalls.transform);
            newWall.transform.localScale = new Vector3(width, 15, 1);
            Material material = newWall.transform.GetChild(0).GetComponent<Renderer>().material;
            material.mainTexture = texture;
            material.mainTextureScale = new Vector2(width / (2048.0f / 1546 * 15), 1);
            zoffset += 1;
        }

        GameObject timeWall = Instantiate(timeWallPrefab, new Vector3(width / 2.0f, -height / 2.0f, -gameController.maxTimeSteps / 8.0f), Quaternion.Euler(0, 0, 0));
        timeWall.transform.localScale = new Vector3(width + 10, height + 10, gameController.maxTimeSteps / 8.0f);
        timeWall.layer = 8;
        timeWall.transform.GetChild(0).gameObject.layer = 8;
        // Instantiate(timeWallPrefab, new Vector3(width + 0.5f, -height, 0), Quaternion.Euler(0, 90, 0)).transform.localScale = new Vector3(gameController.maxTimeSteps / 4.0f, height + 5, 1);
        //Instantiate(timeWallPrefab, new Vector3(0, -height, 0), Quaternion.Euler(90, 90, 0)).transform.localScale = new Vector3(gameController.maxTimeSteps / 4.0f, width + 1, 1);
        //Instantiate(timeWallPrefab, new Vector3(0, 5, 0), Quaternion.Euler(90, 90, 0)).transform.localScale = new Vector3(gameController.maxTimeSteps / 4.0f, width + 1, 1);

        transform.localScale = new Vector3(1, 1, gameController.maxTimeSteps / 4.0f);
        //transform.Find("Finish").GetComponent<Renderer>().material.mainTextureScale = new Vector2(maxTimeSteps / 4.0f, 1);

        viewer = GameObject.Find("Viewer");
    }

    void Update()
    {
        backgroundWalls.transform.position = new Vector3(0, viewer.transform.position.y + 11, 0);
    }


}
