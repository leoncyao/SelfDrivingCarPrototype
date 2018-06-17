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



    int t = 0;
    public static Main instance;


    void Start () {
        instance = this;


        // Setting world fields
        //screenSize = 10f;
        centerX = screenSize / 2;
        centerY = screenSize / 2;
        center = new Vector2(centerX, centerY);



        // Setting camera
        Vector3 cameraPos = new Vector3(centerX, centerX, - screenSize/2);
        mainCamera = Camera.main;
        mainCamera.transform.position = cameraPos;
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = screenSize/2;
        mainCamera.backgroundColor = Color.black;

        UI.instance.check(mainCamera);
        // good stuff

        Vector2 startingLoc = new Vector2(centerX / 2, centerY);
        //Vector2 startingLoc = center;
        mainCar = Instantiate(carPrefab, startingLoc, Quaternion.identity);


    }



    void Update () {

        drawCenteredAxes();

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
