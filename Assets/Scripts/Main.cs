using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject carPrefab;
    Camera mainCamera;
    GameObject mainCar;
	void Start () {
        mainCar = Instantiate(carPrefab, Vector2.zero, Quaternion.identity);
        mainCamera = Camera.main;
	}
	
	void Update () {
		




	}
}
