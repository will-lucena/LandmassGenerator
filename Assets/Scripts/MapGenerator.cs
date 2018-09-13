using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        Noise,
        Color,
        Mesh
    }

    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public bool treePlacement;
    public int treeRate;
    public GameObject treePrefab;
    public Transform target;
    public Vector2 offset;
    public float meshHeightModfifier;

    public bool autoUpdate;
    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colorMap [y * mapWidth + x] = regions [i].color;
						break;
					}
				}
            }
		}

        MapDisplay display = FindObjectOfType<MapDisplay>();

        switch (drawMode)
        {
            case DrawMode.Noise:
                display.drawTexture(TextureGenerator.textureFromHeightMap(noiseMap));
                break;
            case DrawMode.Color:
                display.drawTexture(TextureGenerator.textureFromColorMap(colorMap, mapWidth, mapHeight));
                break;
            case DrawMode.Mesh:
                display.drawMesh(MeshGenerator.generateTerrainMesh(noiseMap, meshHeightModfifier), TextureGenerator.textureFromColorMap(colorMap, mapWidth, mapHeight));
                break;
        }

        if (treePlacement)
        {
            placeTrees(TreeMap.findTrees(noiseMap, treeRate, regions[1].height));
        }
    }
    
    public void placeTrees(List<Vector3> trees)
    {
        foreach (Vector3 position in trees)
        {
            Vector3 positionCorrection = new Vector3(position.x - mapWidth/2, position.y, position.z - mapHeight/2);
            GameObject go = Instantiate(treePrefab, positionCorrection, Quaternion.identity, target);
            MeshCollider meshCollider = go.GetComponent<MeshCollider>();

            Vector3 origin = new Vector3(go.transform.position.x, go.transform.position.y + meshCollider.bounds.max.y, go.transform.position.z);

            RaycastHit hit;

            if (Physics.Raycast(origin, go.transform.TransformDirection(Vector3.down), out hit))
            {
                Debug.Log(hit.normal);
            }
            go.transform.rotation = new Quaternion(hit.normal.x, hit.normal.y, hit.normal.z, Quaternion.identity.w);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + meshCollider.bounds.max.y/2, go.transform.position.z);
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        if (treeRate < 1)
        {
            treeRate = 1;
        }
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}