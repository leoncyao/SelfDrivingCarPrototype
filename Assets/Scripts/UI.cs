using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    public GameObject statPrefab;
    public static UI instance;
    int numStats;
    Vector2 startingStatLoc;

    float screenSize;
   	void Start () {
        instance = this;
        //float screenSize = Main.instance.getScreenSize();


    }
	
	void Update () {
		
	}

    public GameObject makeStat(string name, Camera forScreen) 
    {
        Vector2 offset = new Vector2(0, - 1 * numStats * 1/10 * screenSize);
        Vector2 statloc = forScreen.WorldToScreenPoint(startingStatLoc + offset);
        GameObject stat = Instantiate(statPrefab, statloc, Quaternion.identity);
        stat.transform.SetParent(this.transform, true);
        Stat info = stat.GetComponent<Stat>();
        info.name = name;
        numStats += 1;
        return stat;
    }

    public void check()
    {
        screenSize = Main.instance.getScreenSize();
        startingStatLoc = new Vector2(screenSize / 10, (9 * screenSize)/10f);
    }
}
