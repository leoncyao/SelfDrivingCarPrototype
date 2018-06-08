using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour {

    public GameObject carPrefab;
    Camera mainCamera;
    GameObject mainCar;
    float screenSize = 10f;
    Vector2 center;
    float centerX, centerY;

    public static Main instance;

    ArrayList statinfo;
    List<Stat> stats = new List<Stat>();
    int t;
    void Start () {
        instance = this;


        // Setting world fields
        //screenSize = 10f;
        centerX = screenSize / 2;
        centerY = screenSize / 2;
        center = new Vector2(centerX, centerY);



        // Setting camera
        Vector3 cameraPos = new Vector3(centerX, centerX, - screenSize);
        mainCamera = Camera.main;
        mainCamera.transform.position = cameraPos;
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = screenSize/2;

        UI.instance.check(mainCamera);
        // good stuff

        mainCar = Instantiate(carPrefab, center, Quaternion.identity);
        statinfo = new ArrayList();
        statinfo.AddRange((new string[] { "mainCar.currentAngle", "mainCar.currentAngularVelocity", "mainCar.currentAngularAccerlation"}));
        stats = new List<Stat>();
        foreach (string name in statinfo)
        {
            stats.Add(UI.instance.makeStat(name));
        }


        t = 0;

    }

    void Update () {

        drawCenteredAxes();


        //make this update slower probably 1 once every tenth of second preferably
        //((GameObject)stats[0]).GetComponent<Stat>().updateData(mainCar.GetComponent<Car>().getCurrentAngle());
        //((GameObject)stats[1]).GetComponent<Stat>().updateData(mainCar.GetComponent<Car>().getCurrentAngularVelocity());
        //((GameObject)stats[2]).GetComponent<Stat>().updateData(mainCar.GetComponent<Car>().getCurrentAngularAcceleration());

        if (t % 10 == 0)
        {
            stats[0].updateData(mainCar.GetComponent<Car>().getCurrentAngle());
            stats[1].updateData(mainCar.GetComponent<Car>().getCurrentAngularVelocity());
            stats[2].updateData(mainCar.GetComponent<Car>().getCurrentAngularAcceleration());
        }
        t += 1;

    }

    void drawCenteredAxes()
    {

        Debug.DrawLine(new Vector3(centerX, 0, 0), new Vector3(centerX, screenSize, 0));
        Debug.DrawLine(new Vector3(0, centerY, 0), new Vector3(screenSize, centerY, 0));

    }

    public Vector2 getCenter()
    {
        return center;
    }

    public float getScreenSize()
    {
        return this.screenSize;
    }

}
