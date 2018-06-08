﻿using System.Collections;
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

    GameObject stat1;

	void Start () {
        instance = this;


        // Setting world fields
        //screenSize = 10f;
        centerX = screenSize / 2;
        centerY = screenSize / 2;
        center = new Vector2(centerX, centerY);

        UI.instance.check();

        // Setting camera
        Vector3 cameraPos = new Vector3(centerX, centerX, - screenSize / 2);
        mainCamera = Camera.main;
        mainCamera.transform.position = cameraPos;
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = screenSize/2;


        // good stuff

        mainCar = Instantiate(carPrefab, center, Quaternion.identity);


        stat1 = UI.instance.makeStat("mainCar.velocity", mainCamera);

    }
	
	void Update () {

        drawCenteredAxes();


        //make this update slower probably 1 once every tenth of second preferably
        stat1.GetComponent<Stat>().updateData(mainCar.GetComponent<Car>().getCurrentAngle());

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
