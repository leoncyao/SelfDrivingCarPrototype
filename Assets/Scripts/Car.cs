using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    Rigidbody r;
    Vector3 lastTarget;
    float currentAngle = 0;
    float currentAngularVelocity = 0;
    float currentAngularAcceleration = 0;
    float lastAngle = 0;
	void Start () {
        r = GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);
	}
	
	void Update () {

        // First do checks (inputs of the model)

        float AngularForce = 0;
        // If the left mouse is clicked, set the last angle positioning
        AngularForce += checkNewTarget();

        // check current angle is not the same as last target angle 
        float restoringForce = restoringForceAvg();
        //// scan for front obstacles
        //float angularVelocityChange = checkFrontObstacle();



        // once all checks are done, do calculations with weights that prioritize the most critical action (outputs)
        //currentAngularAcceleration += AngularForce;
        //currentAngularVelocity += currentAngularAcceleration;
        //currentAngle += currentAngularVelocity;

        //currentAngularAcceleration /= 2;

        // x(t) =
        currentAngle += restoringForce;



        // actual changes
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * currentAngle));
        Vector3 movement = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);
        // will add varying speeds later
        r.velocity = movement * 1f;


        ////Set angle visualization.
        //drawAngleArrow(currentAngle, Color.red); //CA
        //drawAngleArrow(lastAngle, Color.green); //LA
        drawJourneyLine();

    }

    public float checkFrontObstacle()
    {
        float deviation = 0;
        // Fires a ray directly in front of this object, and returns the angle to deviate if an obstacle is blocking

        Vector3 direction = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);
        // Draw for debugging
        Debug.DrawRay(transform.position, 10 * direction);

        float length = 10f;
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, length);

        if (hitSomething)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            float obstacleAngle = calculateAngle(transform.position, hit.point);
            drawAngleArrow(obstacleAngle, Color.magenta);

            deviation = (lastAngle + obstacleAngle) / 2;
        }


        return deviation;
    }

    public float restoringForceAvg()
    {
        float toChange = 0;
        if (currentAngle != lastAngle)
        {
            float difference1 = 0, difference2 = 0;
            //Angle measurement of difference1 is based off the angle of currentAngle and lastAngle
            if (currentAngle < lastAngle)
            {

                difference1 = Mathf.Abs(currentAngle - lastAngle);
                //measures counterclockwise

                difference2 = 2 * Mathf.PI - difference1;
                //measures clockwise

                if (difference1 < difference2)
                {
                    //go ccw
                    toChange = difference1;
                }
                else if (difference1 > difference2)
                {
                    //go cw
                    toChange = -1 * difference2;
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
                    toChange = -1 * difference1;
                }
                else if (difference1 > difference2)
                {
                    //go ccw
                    toChange = difference2;
                }

            }

            // speed of rotation, might be weighted outside of function
            toChange /= 100;


            //Use for debug, (REMOVE)
            if (Input.GetMouseButtonDown(1))
            {
                print("difference1 is " + Mathf.Rad2Deg * difference1 + " difference2 " + Mathf.Rad2Deg * difference2);
                print("currentangle is " + Mathf.Rad2Deg * currentAngle + " lastangle is " + Mathf.Rad2Deg * lastAngle);
            }

        }

        return toChange;
    }

    public float restoringForceDO()
    {


        return 0;
    }

    public float checkNewTarget()
    {
        float normalPathForce = 0;
        if (Input.GetMouseButtonDown(0))
        {
            //Acquire mouse positioning
            Vector3 newTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            lastTarget = newTarget;
            lastAngle = calculateAngle(transform.position, lastTarget);
            print(lastAngle);
            normalPathForce = Mathf.Pow((lastAngle + currentAngle)/2, 2)/2;
            print(normalPathForce);
        }


        return normalPathForce;
    }

    public float calculateAngle(Vector2 from, Vector2 to)
    {
        float angle;
        if (from.magnitude < 0.01)
        {
            angle = Mathf.Atan(to.y / to.x);
        }
        else
        {
            Vector3 relativePos = to - from;
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
                angle = 2 * Mathf.PI - angle;
            }
        }
        return angle;
    }

    //Draw the angles on screen.
    public void drawAngleArrow(float angle, Color color)
    {
        float length = 3f;
        Vector2 start = Main.instance.getCenter();
        Vector2 unit = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Debug.DrawLine(start, start + length*unit, color);
    }

    public void drawJourneyLine()
    {
        if (lastTarget != Vector3.zero)
        {
            Debug.DrawLine(transform.position, lastTarget, Color.blue);
        }
    }

}
