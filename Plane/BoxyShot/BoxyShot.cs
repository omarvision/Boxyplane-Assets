using UnityEngine;
using System.Collections;

public class BoxyShotSensor
{
    public Vector3 dir;
    public Vector3 origin;
    public float len = 3.0f;
    public RaycastHit hit;
    public Color col = Color.yellow;
}

public class BoxyShot : MonoBehaviour 
{
    public float fwdSpeed = 20.0f;
    public GameObject explo;
    public float lifeTime = 3.0f;

    private BoxyShotSensor sensor = new BoxyShotSensor();
    private float timeStart;

	// Use this for initialization
	void Start ()
    {
        timeStart = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        MoveForward();
        Sensors();

        if (Time.time - timeStart > lifeTime)
            Destroy(this.gameObject);
	}

    private void MoveForward()
    {
        transform.Translate(0, 0, fwdSpeed++ * Time.deltaTime);
    }

    private void Sensors()
    {
        sensor.dir = transform.TransformDirection(Vector3.forward);
        sensor.origin = transform.position;

        Debug.DrawRay(sensor.origin, sensor.dir * sensor.len, sensor.col);

        bool collide = Physics.Raycast(sensor.origin, sensor.dir, out sensor.hit, sensor.len);
        if (collide)
        {
            if (sensor.hit.transform.gameObject.tag == "Dragon")
            {
                //dragon script = (dragon)sensor.hit.transform.gameObject.GetComponent("dragon");
                //if (script != null)
                //{
                //    if (script.DragonGotShot() == true)
                //        Destroy(this.gameObject);
                //}
            }            
            else if (sensor.hit.transform.gameObject.tag != "BoxyShot")
            {
                Instantiate(explo, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }    
        }
    }
}
