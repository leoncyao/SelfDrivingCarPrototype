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
	
	void Update () {
        if (currentAngle != lastAngle)
        {

            /*
 * 
 * if (currentAngle < lastAngle){
 * 
 * difference1 =  Mathf.Abs(currentAngle - lastAngle);
 * measures counterclockwise
 * 
 * difference2 =  2 * Mathf.PI - difference1;
 * measures clockwise
 * 
 * 
 * if ( difference1 < difference2){
 *     go ccw
 *     currentAngle += difference1 / 100;
 * } else if (difference1 > difference2){
       go cw
       currentAngle -= difference2 / 100;
 * }

 * } else if (currentAngle > lastAngle) {
 * 
 * difference1 =  Mathf.Abs(currentAngle - lastAngle);
 * measures clockwise
 * 
 * difference2 =  2 * Mathf.PI - difference1;
 * measures counterclockwise
 * 
   if ( difference1 < difference2){
 *     go cw
 *     currentAngle -= difference1 / 100;
 * } else if (difference1 > difference2){
       go ccw
       currentAngle += difference2 / 100;
 * }
 * 
 * }
 * 
 * 
 * 

 * */


            float difference1 = 0, difference2 = 0;

            if (currentAngle < lastAngle)
            {

                difference1 = Mathf.Abs(currentAngle - lastAngle);
                //measures counterclockwise

                difference2 = 2 * Mathf.PI - difference1;
                //measures clockwise

                if (difference1 < difference2)
                {
                    //go ccw
                    currentAngle += difference1 / 100;
                }
                else if (difference1 > difference2)
                {
                    //go cw
                    currentAngle -= difference2 / 100;
                }

            }
            else if (currentAngle > lastAngle)
            {

                difference1 = Mathf.Abs(currentAngle - lastAngle);
                //measures clockwise

                difference2 = 2 * Mathf.PI - difference1;
                //measures counterclockwise

                if (difference1 < difference2)
                {
                    //go cw
                    currentAngle -= difference1 / 100;
                }
                else if (difference1 > difference2)
                {
                    //go ccw
                    currentAngle += difference2 / 100;
                }

            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * currentAngle));

            if (Input.GetMouseButtonDown(1))
            {
                print("difference1 is " + Mathf.Rad2Deg * difference1 + " difference2 " + Mathf.Rad2Deg * difference2);
                print("currentangle is " + Mathf.Rad2Deg * currentAngle + " lastangle is " + Mathf.Rad2Deg * lastAngle);
            }


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

        drawAngleArrow(currentAngle, Color.red);
        drawAngleArrow(lastAngle, Color.green);



    }


    public void drawAngleArrow(float angle, Color color)
    {
        Vector2 start = Main.instance.getCenter();
        Vector2 unit = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Debug.DrawLine(start, start + unit, color);
    }


}
