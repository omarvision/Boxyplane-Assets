using UnityEngine;
using UnityEngine.UI; //for Text
using System.Text; //for stringbuilder

public class Bounds
{
    public float x1;
    public float x2;
    public float z1;
    public float z2;
    public float y1;
    public float y2;
}

public class GameController : MonoBehaviour
{
    public Text ScreenText = null;
    public GameObject cowprefab = null;
    public int numcows;
    public GameObject dragonprefab = null;
    public int numdragons;
    
    private int score = 0;
    private int planealtitude = 0;
    private int planespeed = 0;
    private int cows = 0;
    private int dragons = 0;
    private float elapsed;
    private Terrain terr;
    private Bounds bound = new Bounds();
    private AudioSource[] audios;

    private enum sound
    {
        music1 = 0,
        music2 = 1,
        music3 = 2,
    }

    private void Start()
    {
        if (Terrain.activeTerrain != null)
        {
            terr = Terrain.activeTerrain;
            bound.x1 = terr.GetPosition().x;
            bound.z1 = terr.GetPosition().z;
            bound.y1 = terr.GetPosition().y;
            bound.x2 = bound.x1 + terr.terrainData.size.x;
            bound.z2 = bound.z1 + terr.terrainData.size.z;
            bound.y2 = bound.y1 + terr.terrainData.size.y;
        }

        for (int i = 0; i < numcows; i++)
        {
            GameObject obj = Instantiate(cowprefab);
            obj.name = "Cow" + i.ToString("000");
        }

        for (int i = 0; i < numdragons; i++)
        {
            GameObject obj = Instantiate(dragonprefab);
            obj.name = "Dragon" + i.ToString("000");
        }

        audios = this.GetComponents<AudioSource>();
        audios[(int)sound.music2].Play();

        cows = numcows;
        dragons = numdragons;
        UpdateGameText();

        DrawBoundingBox();
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
    }

    public void UpdateGameText()
    {
        if (ScreenText == null)
            return;

        StringBuilder sb1 = new StringBuilder();
        sb1.AppendLine("Score:" + score.ToString());
        sb1.AppendLine("Alt:" + planealtitude.ToString() + ", Speed:" + planespeed.ToString());
        sb1.AppendLine("Cows:" + cows.ToString());
        sb1.AppendLine("Dragons:" + dragons.ToString());

        ScreenText.text = sb1.ToString();
    }

    public void AddToScore(int points)
    {
        score += points;
        UpdateGameText();
    }

    public void DisplayPlane(int altitude, int speed)
    {
        planealtitude = altitude;
        planespeed = speed;
        UpdateGameText();
    }
        
    public void SubtractDragon()
    {
        dragons -= 1;
        UpdateGameText();
    }

    public void SubtractCow()
    {
        cows -= 1;
        UpdateGameText();
    }

    public Terrain GetTerrain()
    {
        return terr;
    }

    public Bounds GetBounds()
    {
        return bound;
    }

    public Vector3 GetRandomLocationOverTerrain(float heightaboveterrain)
    {
        Vector3 ret = new Vector3();

        if (terr != null)
        {
            Vector3 p1 = new Vector3(Random.Range(bound.x1, bound.x2), 0, Random.Range(bound.z1, bound.z2));
            float h = terr.transform.position.y + terr.SampleHeight(p1);
            ret = new Vector3(p1.x, h, p1.z);
        }        

        return ret;
    }

    public bool isOutOfBounds(Vector3 position)
    {
        if ((position.x < bound.x1 || position.x > bound.x2) 
            || (position.z < bound.z1 || position.z > bound.z2) 
            || (position.y < bound.y1 || position.y > bound.y2))
        {
            //plane is out of bounds.
            return true;
        }
        return false;
    }

    private void DrawBoundingBox()
    {
        Vector3 A = new Vector3(bound.x1, bound.y2, bound.z1);
        Vector3 B = new Vector3(bound.x2, bound.y2, bound.z1);
        Vector3 C = new Vector3(bound.x2, bound.y2, bound.z2);
        Vector3 D = new Vector3(bound.x1, bound.y2, bound.z2);

        Vector3 E = new Vector3(bound.x1, bound.y1, bound.z1);
        Vector3 F = new Vector3(bound.x2, bound.y1, bound.z1);
        Vector3 G = new Vector3(bound.x2, bound.y1, bound.z2);
        Vector3 H = new Vector3(bound.x1, bound.y1, bound.z2);

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

    public void CowDeadCheckTargetCows(string dragonname, string cowname)
    {
        GameObject[] dragons = GameObject.FindGameObjectsWithTag("Dragon");
        foreach (GameObject dragon in dragons)
        {
            if (dragon.name == dragonname)
                continue;
            Dragon dragonscript = (Dragon)dragon.GetComponent(typeof(Dragon));
            dragonscript.CowIsDeadAlert(cowname);
        }
    }
}
