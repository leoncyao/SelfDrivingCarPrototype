using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Main : MonoBehaviour {

    public GameObject carPrefab;
    Camera mainCamera;
    GameObject mainCar;
    float screenSize = 15f;
    Vector2 center;
    float centerX, centerY;

    public float[] axis;

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

        Vector2 startingLoc = new Vector2(centerX / 2, centerY - 1);
        //Vector2 startingLoc = center;
        mainCar = Instantiate(carPrefab, startingLoc, Quaternion.identity);

        axis = new float[] { 0, centerY, screenSize / 4 };

    }



    void Update () {

        drawAxes(axis[0], axis[1], axis[2]);

    }
    void drawAxes(float originX, float originY, float axisLength)
    {

        Debug.DrawLine(new Vector3(originX, originY-axisLength/2, 0), new Vector3(originX, originY + axisLength / 2, 0));

        Debug.DrawLine(new Vector3(originX - axisLength/2, originY, 0), new Vector3(originX + axisLength / 2, originY, 0));

    }

    public Vector2 getCenter()
    {
        return center;
    }

    public float[] getAxis()
    {
        return axis;
    }

    public float getScreenSize()
    {
        return this.screenSize;
    }

}
