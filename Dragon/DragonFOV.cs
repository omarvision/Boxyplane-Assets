using UnityEngine;
using System.Collections;

public class DragonFOV : MonoBehaviour
{
    public Dragon parentscript = null;
    
	void Start ()
    {
	}
	
	void Update ()
    {	
	}

    private void OnDrawGizmos()
    {
        //display FOV sphere for dragon in scene view
        SphereCollider collider = this.GetComponent<SphereCollider>();
        Gizmos.color = Color.white;
        Vector3 p1 = collider.transform.position;
        Vector3 pos = new Vector3(p1.x + collider.center.x, p1.y + collider.center.y, p1.z + collider.center.z);
        Gizmos.DrawWireSphere(pos, collider.radius);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cow")
            parentscript.CowInSight(other);
    }

}
