using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stat : MonoBehaviour
{

    string name;
    float data = 0;

    void Start()
    {



    }
    void Update()
    {

        GetComponent<Text>().text = name + ": " + data.ToString();

    }

    public void updateData(float data) {

        this.data = data;

    }


}

