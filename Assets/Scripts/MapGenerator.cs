using UnityEngine;
using System.Collections;

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
    public int treeRate;
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
                double max = 0;
                // there are more efficient algorithms than this
                for (int yn = y - treeRate; yn <= y + treeRate; yn++)
                {
                    for (int xn = x - treeRate; xn <= x + treeRate; xn++)
                    {
                        if (yn > 0 && yn < mapHeight && xn > 0 && xn < mapWidth)
                        {
                            double e = noiseMap[yn, xn];
                            if (e > max) { max = e; }
                        }
                    }
                }
                if (noiseMap[y, x] == max && noiseMap[y, x] > regions[0].height)
                {
                    colorMap[y * mapWidth + x] = regions[regions.Length-1].color;
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