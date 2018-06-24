using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    public GameObject statPrefab;
    public static UI instance;
    Camera mainCamera;
    int numStats;
    Vector2 startingStatLoc;

    float screenSize;
   	void Start () {
        instance = this;
        //float screenSize = Main.instance.getScreenSize();
    }

    public void check(Camera camera)
    {
        mainCamera = camera;
        screenSize = Main.instance.getScreenSize();
        startingStatLoc = new Vector2(screenSize / 10, (9 * screenSize)/10f);
    }
}
