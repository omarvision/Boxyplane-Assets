using UnityEngine;
using UnityEngine.UI; //for Text

[System.Serializable]
public class boxyPlaneCamera
{
    public float behind = 15.0f;
    public float above = 2.0f;
    public float followbias = 0.82f;
    public float lookahead = 10.0f;
}

public enum enumJoystickName
{
    //  http://wiki.unity3d.com/index.php?title=Xbox360Controller    
    //              InputManager Settings:
    LeftAnalogX,    //dead:0.004, sensitivity:1, type:Joystick Axis, axis:X axis
    LeftAnalogY,    //dead:0.004, sensitivity:1, type:Joystick Axis, axis:Y axis 
    RightAnalogX,   //dead:0.004, sensitivity:1, type:Joystick Axis, axis:4th axis
    RightAnalogY,   //dead:0.004, sensitivity:1, type:Joystick Axis, axis:5th axis
    AButton,        //positive button: joystick button 0, type:Key or Mouse Button
    BButton,        //positive button: joystick button 1, type:Key or Mouse Button
    XButton,        //positive button: joystick button 2, type:Key or Mouse Button
    YButton,        //positive button: joystick button 3, type:Key or Mouse Button
    RightTrigger,   //dead:0.004, sensitivity:1, type:Joystick Axis, axis:3rd axis
    LeftTrigger,    //dead:0.004, sensitivity:1, type:Joystick Axis, axis:3rd axis
}

public class BoxyPlaneSensor
{
    public Vector3 dir;
    public Vector3 origin;
    public float len = 2.0f;
    public RaycastHit hit;
    public Color col = Color.yellow;
}

public class BoxyPlane : MonoBehaviour 
{
    public float fwdSpeed = 4.0f;
    public float turnSpeed = 45.0f;
    public boxyPlaneCamera followCam = new boxyPlaneCamera();
    public GameObject shot;
    public float shotRate = 0.30f;
    public GameObject explo;
    
    private float nextFire;
    private float fwdSpeedOffset = 1;
    private GameController gamecontroller = null;
    private AudioSource[] audios;
    private BoxyPlaneSensor sensor = new BoxyPlaneSensor();

    private enum sound
    {
        thrust = 0,
        pullup = 1,
        crashing = 2,
    }

    private void Start()
    {
        gamecontroller = (GameController)GameObject.Find("GameController").GetComponent(typeof(GameController));
        audios = this.GetComponents<AudioSource>();
        audios[(int)sound.thrust].Play();
    }
	
	private void Update ()
    {
        CameraFollow();
        ReadJoystick();
        ReadKeyboard();
        MoveForward();
        Sensors();

        if (gamecontroller.isOutOfBounds(transform.position) == true)
        {
            Debug.Log("Plane out of bounds");
            transform.eulerAngles.Set(transform.eulerAngles.x, transform.eulerAngles.y + 180.0f, transform.eulerAngles.z);
        }
	}
    
    private void MoveForward()
    {
        transform.Translate(0, 0, (fwdSpeed + fwdSpeedOffset) * Time.deltaTime);
    }

    private void ReadJoystick()
    {
        // --- LEFT ANALOG (flying) ---
        //joystick axes return -1 to 1 values
        float leftRight = Input.GetAxis(enumJoystickName.LeftAnalogX.ToString()) * turnSpeed;
        float upDown = Input.GetAxis(enumJoystickName.LeftAnalogY.ToString()) * turnSpeed;            
        //direction
        transform.Rotate(Vector3.up * (leftRight * Time.deltaTime));
        transform.Rotate(Vector3.left * (upDown * Time.deltaTime));
        //bank
        if (leftRight < -15)
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 25.0f);
        else if (leftRight > 15)
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -25.0f);
        else
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // --- RIGHT ANALOG (speed) ---
        float v1 = Input.GetAxis(enumJoystickName.RightAnalogY.ToString()) * -1;  // -1 to 1    
        if (v1 < 0)
            fwdSpeedOffset = fwdSpeed * (v1 * 0.75f);
        else if (v1 > 0)
            fwdSpeedOffset = 10 * v1;
        else
            fwdSpeedOffset = 0;

        //update plane speed and altitude on screen text
        int speed = (int)(fwdSpeed + fwdSpeedOffset);
        gamecontroller.DisplayPlane(GetAltitude(), speed);
        audios[0].pitch = 1.0f + (v1/3.0f);

        // --- JOYSTICK TRIGGERS (shooting) ---
        float rightTrigger = Input.GetAxis(enumJoystickName.RightTrigger.ToString());
        if (rightTrigger < 0 && Time.time > nextFire)
        {
            nextFire = Time.time + shotRate;
            SpawnShot();
        }        
    }

    private void ReadKeyboard()
    {
        float leftRight = 0.0f;
        float upDown = 0.0f;

        // MOVEMENT KEYS = W,A,S,D -or- up,down,left,right
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            leftRight = -65.0f;
            transform.Rotate(Vector3.up * (leftRight * Time.deltaTime));
        }
        else if (Input.GetKey("d") || Input.GetKey("right"))
        {
            leftRight = 65.0f;
            transform.Rotate(Vector3.up * (leftRight * Time.deltaTime));
        }

        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            upDown = 25.0f;
            transform.Rotate(Vector3.left * (upDown * Time.deltaTime));
        }
        else if (Input.GetKey("s") || Input.GetKey("down"))
        {
            upDown = -25.0f;
            transform.Rotate(Vector3.left * (upDown * Time.deltaTime));
        }


        // SHOOT
        if (Input.GetKey("space") && Time.time > nextFire)
        {
            nextFire = Time.time + shotRate;
            SpawnShot();
        }

    }

    private void CameraFollow()
    {
        Vector3 moveCamTo = transform.position - (transform.forward * followCam.behind) + (Vector3.up * followCam.above);
        Camera.main.transform.position = (Camera.main.transform.position * followCam.followbias) + (moveCamTo * (1.0f - followCam.followbias));
        Camera.main.transform.LookAt(transform.position + (transform.forward * followCam.lookahead));
    }

    private void SpawnShot()
    {
        Instantiate(shot, transform.position, transform.rotation);
    }
    
    private int GetAltitude()
    {
        float alt = 0;
        Terrain terr = gamecontroller.GetTerrain();
        float sh = terr.transform.position.y + terr.SampleHeight(transform.position);
        alt = transform.position.y - sh;
        return ((int)alt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            //crashing into terrain
            if (audios[(int)sound.crashing].isPlaying == false)                
                audios[(int)sound.crashing].Play();
            Instantiate(explo, transform.position, transform.rotation);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        }
    }

    private void Sensors()
    {
        //getting to close to terrain
        sensor.dir = transform.TransformDirection(Vector3.down);
        sensor.origin = transform.position;

        bool collide = Physics.Raycast(sensor.origin, sensor.dir, out sensor.hit, sensor.len);
        if (collide)
        {
            if (sensor.hit.transform.gameObject.tag == "Terrain")
            {
                if (audios[(int)sound.pullup].isPlaying == false)
                    audios[(int)sound.pullup].Play();
            }
        }
    }
}
