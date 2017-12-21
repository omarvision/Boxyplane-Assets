using UnityEngine;
using System.Collections;

public class Dragon : MonoBehaviour
{
    public bool UseRandomSpawn = true;
    public GameObject targetobj = null;

    private NavMeshAgent nav = null;
    private float elapsed;
    private Terrain terr;
    private float terrx1;
    private float terrx2;
    private float terrz1;
    private float terrz2;
    private float terry1;
    private float terry2;

    void Start ()
    {
        nav = GetComponent<NavMeshAgent>();
        elapsed = 0.0f;

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

        //location of cow on terrain
        if (UseRandomSpawn == true)
        {
            randSpawnLocation();
        }

        //location of target on terrain (if target objecta ssigned then use it, else randomly generate)
        if (targetobj == null)
        {
            targetobj = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            randNavTarget();
        }

        //set the cow to move towards the target
        nav.SetDestination(targetobj.transform.position);
    }
	
	void Update ()
    {
        elapsed += Time.deltaTime;
    }

    private void randSpawnLocation()
    {
        //random location over the terrain (height just on terrain surface)
        Vector3 p1 = new Vector3(Random.Range(terrx1, terrx2), 0, Random.Range(terrz1, terrz2));
        float h = terr.transform.position.y + terr.SampleHeight(p1) + ((Collider)GetComponent<Collider>()).bounds.size.y;
        transform.position = new Vector3(p1.x, h, p1.z);

        //turn to face a random direction
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    private void randNavTarget()
    {
        //random cow target object over terrain
        Vector3 p1 = new Vector3(Random.Range(terrx1, terrx2), 0, Random.Range(terrz1, terrz2));
        float h = terr.transform.position.y + terr.SampleHeight(p1) + ((Collider)GetComponent<Collider>()).bounds.size.y;
        targetobj.transform.position = new Vector3(p1.x, h, p1.z);
    }

    public bool isNavReachedTarget()
    {
        bool ret = false;
        if (!nav.pathPending)
        {
            if (nav.remainingDistance <= nav.stoppingDistance)
            {
                if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                    ret = true;
            }
        }
        return ret;
    }

}
