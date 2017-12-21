using UnityEngine;
using System.Collections.Generic;

public class CowSpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public int count;

    private Terrain terr;
    private float terrx1;
    private float terrx2;
    private float terrz1;
    private float terrz2;
    private float terry1;
    private float terry2;
    
    void Start ()
    {
        if (Terrain.activeTerrain != null)
        {
            terr = Terrain.activeTerrain;
            terrx1 = terr.GetPosition().x;
            terrz1 = terr.GetPosition().z;
            terry1 = terr.GetPosition().y;
            terrx2 = terrx1 + terr.terrainData.size.x;
            terrz2 = terrz1 + terr.terrainData.size.z;
            terry2 = terry1 + terr.terrainData.size.y;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(cowPrefab);
            obj.name = "Cow" + i.ToString("000");
        }

        
        //float xpart = (terrx2 - terrx1) / 10.0f;
        //float zpart = (terrz2 - terrz1) / 10.0f;
        //for (float nx = terrx1; nx < terrx2; nx += xpart)
        //{
        //    for (float nz = terrz1; nz < terrz2; nz += zpart)
        //    {
        //        Vector3 p1 = new Vector3(nx, 0, nz);
        //        float h = terr.transform.position.y + terr.SampleHeight(p1) + 3.0f;

        //        GameObject sp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));

        //        sp.transform.position = new Vector3(nx, h, nz);
        //        sp.GetComponent<Renderer>().material.color = Color.blue;
        //    }
        //}
	}
	
	void Update ()
    {
	
	}
}
