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
	
	void Update () {
		
	}

    public Stat makeStat(string name) 
    {
        Vector2 offset = new Vector2(0, - 0.5f *  numStats * 1/10f * screenSize);
        Vector2 statloc = mainCamera.WorldToScreenPoint(startingStatLoc + offset);
        GameObject stat = Instantiate(statPrefab, statloc, Quaternion.identity);
        stat.transform.SetParent(this.transform, true);
        Stat info = stat.GetComponent<Stat>();
        info.dataName = name;
        numStats += 1;
        return info;
    }

    public void check(Camera camera)
    {
        mainCamera = camera;
        screenSize = Main.instance.getScreenSize();
        startingStatLoc = new Vector2(screenSize / 10, (9 * screenSize)/10f);
    }
}
