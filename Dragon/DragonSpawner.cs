using UnityEngine;
using System.Collections;

public class DragonSpawner : MonoBehaviour
{
    public GameObject dragonPrefab;
    public int count;

	void Start ()
    {
	    for (int i = 0; i < count; i++)
        {
            Instantiate(dragonPrefab);
        }
	}
	
	void Update ()
    {
	
	}
}
