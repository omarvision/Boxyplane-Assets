using UnityEngine;
using System.Collections;

public class Bounds : MonoBehaviour
{
    private Terrain terr;
    private float x1;
    private float x2;
    private float z1;
    private float z2;
    private float y1;
    private float y2;

    void Start ()
    {
        terr = Terrain.activeTerrain;
        x1 = terr.GetPosition().x;
        z1 = terr.GetPosition().z;
        y1 = terr.GetPosition().y;
        x2 = x1 + terr.terrainData.size.x;
        z2 = z1 + terr.terrainData.size.z;
        y2 = y1 + terr.terrainData.size.y;
    }

    void Update()
    {
        DrawBoundingBox();
    }

    private void DrawBoundingBox()
    {
        Vector3 A = new Vector3(x1, y2, z1);
        Vector3 B = new Vector3(x2, y2, z1);
        Vector3 C = new Vector3(x2, y2, z2);
        Vector3 D = new Vector3(x1, y2, z2);

        Vector3 E = new Vector3(x1, y1, z1);
        Vector3 F = new Vector3(x2, y1, z1);
        Vector3 G = new Vector3(x2, y1, z2);
        Vector3 H = new Vector3(x1, y1, z2);

        Color col = Color.green;

        Debug.DrawLine(A, B, col);
        Debug.DrawLine(B, C, col);
        Debug.DrawLine(C, D, col);
        Debug.DrawLine(D, A, col);

        Debug.DrawLine(E, F, col);
        Debug.DrawLine(F, G, col);
        Debug.DrawLine(G, H, col);
        Debug.DrawLine(H, E, col);

        Debug.DrawLine(A, E, col);
        Debug.DrawLine(B, F, col);
        Debug.DrawLine(C, G, col);
        Debug.DrawLine(D, H, col);
    }
}
