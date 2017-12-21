using UnityEngine;
using System.Collections;

public class BoxyShotExplosion : MonoBehaviour
{
    public float lifeTime = 2.0f;

    private float timeStart;

	void Start () 
    {
        timeStart = Time.time;
	}
	
	void Update () 
    {
        if (Time.time - timeStart > lifeTime)
            Destroy(this.gameObject);
	}
}
