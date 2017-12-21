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

public class BoxyPlane : MonoBehaviour 
{
    public float fwdSpeed = 4.0f;
    public float turnSpeed = 45.0f;
    public boxyPlaneCamera followCam = new boxyPlaneCamera();
    public Text displayText;
    public GameObject shot;
    public float shotRate = 0.10f;

    private Terrain terr;
    private float terrx1;
    private float terrx2;
    private float terrz1;
    private float terrz2;
    private float terry1;
    private float terry2;

    private float nextFire;
    private float fwdSpeedOffset = 1;
    private int score;
    
	private void Start () 
    {
        score = 0;

        //the dimensions of the terrain (our game world)
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
    }
	
	private void Update ()
    {
        CameraFollow();
        ReadJoystick();
        MoveForward();        
	}

    private void DisplayText(string msg)
    {
        if (displayText != null)
            displayText.text = msg;
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
            fwdSpeedOffset = 30 * v1;
        else
            fwdSpeedOffset = 0;

        string msg = "spd: " + (fwdSpeed + fwdSpeedOffset).ToString();
        msg += "\nalt: " + GetAltitude();
        msg += "\nscore: " + score.ToString();
        DisplayText(msg);
           

        // --- JOYSTICK TRIGGERS (shooting) ---
        float rightTrigger = Input.GetAxis(enumJoystickName.RightTrigger.ToString());
        if (rightTrigger < 0 && Time.time > nextFire)
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

    public void AddScore(int val)
    {
        score += val;
    }

    private string GetAltitude()
    {
        float alt = 0;

        float sh = terr.transform.position.y + terr.SampleHeight(transform.position);

        alt = transform.position.y - sh;

        return ((int)alt).ToString();
    }

    public void BoundsCheck()
    {
        float px = transform.position.x;
        float py = transform.position.y;
        float pz = transform.position.z;
        if ((px < terrx1 || px > terrx2) || (pz < terrz1 || pz > terrz2) || (py < terry1 || py > terry2))
        {
            //plane is out of bounds. what do you want to do?
        }
    }
}
