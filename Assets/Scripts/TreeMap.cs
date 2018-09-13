using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TreeMap
{
    public static List<Vector3> findTrees(float[,] heightMap, int treeRange, float waterBiome)
    {
        int mapHeight = heightMap.GetLength(1);
        int mapWidth = heightMap.GetLength(0);

        List<Vector3> trees = new List<Vector3>();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = heightMap[x, y];
                float max = 0;
                // there are more efficient algorithms than this
                for (int yn = y - treeRange; yn <= y + treeRange; yn++)
                {
                    for (int xn = x - treeRange; xn <= x + treeRange; xn++)
                    {
                        if (yn > 0 && yn < mapHeight && xn > 0 && xn < mapWidth)
                        {
                            float e = heightMap[xn, yn];
                            if (e > max) { max = e; }
                        }
                    }
                }
                if (heightMap[x, y] == max && heightMap[x, y] > waterBiome)
                {
                    trees.Add(new Vector3(x, max, y));
                }
            }
        }
        return trees;
    }
}
