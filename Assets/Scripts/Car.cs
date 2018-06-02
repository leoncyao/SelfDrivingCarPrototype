using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    Rigidbody r;
    Vector3 lastTarget;
    float currentAngle = 0;
    float lastAngle = 0;
	void Start () {
        r = GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

        if (currentAngle != lastAngle)
        {
            float difference = Mathf.Abs(currentAngle - lastAngle);
            if (2 * Mathf.PI - difference < difference)
            {
                difference = 2 * Mathf.PI - difference;
                float direction = currentAngle - lastAngle < 0 ? -1 : 1;
                difference *= direction;
            }
            currentAngle += difference / 100;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * currentAngle));
        }

        Vector3 movement = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);
        r.velocity = movement * 1f;

        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            lastTarget = mousePosition;

            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
            float angle;
            if (transform.position.magnitude < 0.01)
            {
                angle = Mathf.Atan(mousePosition.y / mousePosition.x);
            }
            else
            {
                Vector3 relativePos = mousePosition - transform.position;
                angle = Mathf.Atan(Mathf.Abs(relativePos.y / relativePos.x));
                if (relativePos.x < 0 && relativePos.y > 0)
                {
                    angle = Mathf.PI - angle;
                }
                else if (relativePos.x < 0 && relativePos.y < 0)
                {
                    angle += Mathf.PI;
                }
                else if (relativePos.x > 0 && relativePos.y < 0)
                {
                    angle =  2 * Mathf.PI - angle; 
                }
                lastAngle = angle;
            }

            //print("last Angle " + Mathf.Rad2Deg * lastAngle + "current angle " + Mathf.Rad2Deg * currentAngle);

        }
        
    }
}
