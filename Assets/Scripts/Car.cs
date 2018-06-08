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
    void Start() {
        r = GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);
    }

    void Update() {

        // First do checks (inputs of the model)

        float AngularForce = 0;

        // If the left mouse is clicked, set the last angle positioning
        checkNewTarget();

        // check current angle is not the same as last target angle 
        //float restoringForce = avgRestoringForce();
        float restoringForce = springRestoringForce();


        //// scan for front obstacles
        float angularVelocityChange = checkFrontObstacle();



        // once all checks are done, do calculations with weights that prioritize the most critical action (outputs)
        float restoringForceWeight = 100;
        AngularForce += restoringForce / restoringForceWeight;
        currentAngularAcceleration = AngularForce;
        currentAngularVelocity += currentAngularAcceleration;
        currentAngle += currentAngularVelocity;


        // actual changes
        //print(currentAngle);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * currentAngle));
        Vector3 movement = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);
        // will add varying speeds later
        float speed = 1f;
        r.velocity = movement * speed;


        ////Set angle visualization.
        drawAngleArrow(currentAngle, Color.red); //CA
        drawAngleArrow(lastAngle, Color.green); //LA
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


    public float angleTurn(float currentAngle, float lastAngle)
    {
        float toChange = 0;
        if (currentAngle != lastAngle)
        {
            //Angle measurement of difference1 is based off the angle of currentAngle and lastAngle

            float difference1 = Mathf.Abs(currentAngle - lastAngle);
            float difference2 = 2 * Mathf.PI - difference1;

            if (currentAngle < lastAngle)
            {
                // difference1 measures counterclockwise
                // difference2 measures clockwise
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
                // difference1 measures clockwise
                // difference2 measures counterclockwise
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
        }
        return toChange;
    }

    public float avgRestoringForce()
    {
        float toChange = angleTurn(currentAngle, lastAngle);
        return toChange / 2;
    }

    public float springRestoringForce()
    {
        float force = 0;
        float difference = angleTurn(currentAngle, lastAngle);
        // modeled by mx''(t) + cx'(t) + kx(t) = 0
        float c = 2f, m = 1, k = 0.1f;
        if (Mathf.Abs(difference) > 0.01)
        {
            float dampening = c * currentAngularVelocity;
            float springForce = k * -1*difference;
            force = -(dampening + springForce) / m;
        }

        if (Input.GetMouseButtonDown(1))
        {
            print("difference is " + difference + " force is " + force);
            print("currentangle is " + Mathf.Rad2Deg * currentAngle + " lastangle is " + Mathf.Rad2Deg * lastAngle);
        }

        return force;
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
            //normalPathForce = Mathf.Pow((lastAngle + currentAngle) / 2, 2) / 2;
        }
        return normalPathForce;
    }

    public float calculateAngle(Vector2 from, Vector2 to)
    {
        // treat from as the origin, to as the lever arm
        float angle;
        //if (from.magnitude < 0.01)
        //{
        //    angle = Mathf.Atan(to.y / to.x);
        //}
        //else
        //{
        Vector3 relativePos = to - from;
        angle = Mathf.Atan(Mathf.Abs(relativePos.y / relativePos.x));

        // edge cases for where Atan is undefined
        if (Mathf.Abs(relativePos.x) < 0.01)
        {
            if (relativePos.y > 0)
            {
                angle = Mathf.PI;
            }
            else
            {
                angle = -Mathf.PI;
            }
        } // usual cases (quadrants as tan is the same in third and first)
        else if (relativePos.x < 0 && relativePos.y > 0)
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
        //}
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

    public float getCurrentAngle() { return this.currentAngle; }

    public float getCurrentAngularVelocity() { return this.currentAngularVelocity; }

    public float getCurrentAngularAcceleration() { return this.currentAngularAcceleration; }
}
