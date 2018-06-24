using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carStats {
    GUIStyle guiStyle;
    Dictionary<string, double> stats;
    Car car;
    void OnGUI()
    {
        //print("check");
        GUI.Label(new Rect(40, 40, 100, 100), "test");
        int i = 1;
        foreach (KeyValuePair<string, double> stat in stats)
        {
            GUI.Label(new Rect(40, i * 40, 100, 100), stat.Key + " " + (rounder(Mathf.Rad2Deg, 2) * stat.Value), guiStyle);
            i++;
        }
        //Set angle visualization.
        drawAngleArrow(car.getCurrentAngle(), Color.red); //CA
        drawAngleArrow(car.getLastAngle(), Color.green); //LA

        drawJourneyLine();
        //drawAngleArrow(obstacleAngle, Color.magenta);
    }

    public carStats(Car car, string[] statNames)
    {
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontSize = 20;
        this.car = car;
        stats = new Dictionary<string,double>();
        foreach(string name in statNames)
        {
            stats[name] = 0.0f;
        }
    }
  
    public void updateStat(string statName, double value)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] = value;
        }
    }

    //Draw the angles on screen.
    public void drawAngleArrow(float angle, Color color)
    {
        float[] axis = Main.instance.getAxis();
        float length = axis[2] / 3;
        Vector2 start = new Vector2(axis[0], axis[1]);
        Vector2 unit = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Debug.DrawLine(start, start + length * unit, color);
    }

    public void drawJourneyLine()
    {
        if (car.getLastTarget() != Vector3.zero)
        {
            Debug.DrawLine(car.transform.position, car.lastTarget, Color.blue);
        }
    }

    double rounder(double toRound, int decimalPlaces)
    {
        float degree = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round((float)toRound * degree) / degree;
    }

}
