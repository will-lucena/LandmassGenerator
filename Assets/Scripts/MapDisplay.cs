using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public void drawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, texture.height, 1);
    }

    public void drawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.createMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}