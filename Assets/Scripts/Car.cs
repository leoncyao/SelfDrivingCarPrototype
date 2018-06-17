using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Double;

public class Car : MonoBehaviour {

    Rigidbody r;
    Vector3 lastTarget;
    float currentAngle = 0;
    float currentAngularVelocity = 0;
    float currentAngularAcceleration = 0;
    float speed = 1f;
    float lastAngle = 0;


    GUIStyle guiStyle;

    //Hashtable stats;
    Dictionary<string, double> stats;
    int t;
    void Start()
    {
        
        r = GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);

        t = 1;
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontSize = 20;


        //stats = new Hashtable();
        stats = new Dictionary<string,double>();

        stats["CurrentAngle"] = 0.0;
        stats["CurrentAngularVelocity"] = 0.0;
        stats["CurrentAngularAccerleration"] = 0.0;

    }

    private void OnGUI()
    {

        //foreach (KeyValuePair<string, double> stat in stats)
        int i = 1;
        //foreach(DictionaryEntry stat in stats)
        foreach (KeyValuePair<string, double> stat in stats)
        {
            //print((string)stat.Key);
            GUI.Label(new Rect(40, i*40, 100, 100), stat.Key + " " + (rounder(Mathf.Rad2Deg,2) * stat.Value), guiStyle);
            i++;
        }
    }

    double rounder(double toRound, int decimalPlaces)
    {
        float degree = Mathf.Pow(10, decimalPlaces);
        //print(Mathf.Round((float)toRound * degree) / degree);
        return Mathf.Round((float)toRound * degree) / degree;
    }

    void Update() {
        
        if (t % 10 == 0)
        {
            double[] statinfo = new double[] {rounder(currentAngle, 1), rounder(currentAngularVelocity,2), rounder(currentAngularAcceleration, 4) };
            ArrayList keys = new ArrayList(stats.Keys);
            for(int i = 0; i < stats.Count; i++)
            {
                //print("check" + statinfo[i]);
                stats[(string) keys[i]] = statinfo[i];   
            }
        }

        // First do checks (inputs of the model)

        // adjusts lastAngle to point to lastTarget
        if (lastTarget != null) { 
            if (t % 100 == 0)
            {
                lastAngle = calculateAngle(transform.position, lastTarget);
            }
            t += 1;
        }
        // If the left mouse is clicked, set the last angle positioning
        checkNewTarget();

        //float restoringForce = avgRestoringForce();
        float restoringForce = springRestoringForce();

        // scan for front obstacles
        float frontObsForce = checkFrontObstacle();

        // once all checks are done, do calculations with weights that prioritize the most critical action (outputs)

        float AngularForce = 0;

        float restoringForceWeight = 100;
        AngularForce += restoringForce / restoringForceWeight;

        float frontObsForceWeight = 1000;
        AngularForce += frontObsForce / frontObsForceWeight;

        float moment = 1;
        // Newton's second Law
        AngularForce /= moment;

        currentAngularAcceleration = AngularForce;
        currentAngularVelocity += currentAngularAcceleration;
        currentAngle += currentAngularVelocity;


        // actual changes
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * currentAngle));
        Vector3 movement = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);

        if (speed != 1)
        {
            speed = (1 + speed) / 2;
        }


        r.velocity = movement * speed;


        //Set angle visualization.
        drawAngleArrow(currentAngle, Color.red); //CA
        drawAngleArrow(lastAngle, Color.green); //LA
        drawJourneyLine();

    }

    public float checkFrontObstacle()
    {
        // Fires a ray directly in front of this object, and returns the angle to deviate if an obstacle is blocking
        float force = 0;

        float length = 10f;
        Vector3 direction = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);

        // Draw for debugging
        Debug.DrawRay(transform.position, length * direction);

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, length);

        if (hitSomething)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            float obstacleAngle = calculateAngle(transform.position, hit.point);
            drawAngleArrow(obstacleAngle, Color.magenta);

            // force should be proportional to 		
            // obstacle distance from car- If far away, doesnt matter immediately, so inversely prop
            // difference between obstacle angle and last angle- If the obstacle angle is not related to path, doesnt matter, inversely
            // difference between currentAngle and lastAngle, if currentAngle is close to lastAngle, means on path. directly
            float distance = (hit.point - transform.position).magnitude;
            float obsLastDifference = calculateAngleDifference(obstacleAngle, lastAngle);
            float currentLastDifference = calculateAngleDifference(currentAngle, lastAngle);

            // should eventually be proportional to car length and currentVelocity
  
            //Vector test1 = DenseVector.OfArray(new double[]{5f,1f,2f});
            //Vector test2 = DenseVector.OfArray(new double[] { 5f, 1f, 2f });




            float minAvoidDist = 1.5f;
            float distanceWeight = 5f;
            float cLWeight = 2f;
           
            // care for direction 

            if (distance < minAvoidDist)
            {
                float distanceProportion = Mathf.Exp((1 / Mathf.Abs(distanceWeight)) * 1 / (distance));
                float obsLastDifferenceProportion = -Mathf.Abs(obsLastDifference);
                float currentLastDifferenceProportion = Mathf.Exp((1 / cLWeight) * (-Mathf.Abs(currentLastDifference)));
                force = distanceProportion + obsLastDifferenceProportion + currentLastDifferenceProportion;
            }
            // if currentLastDifference * force > 0 then they have the sign polarity
            // if not then change polarity of force to match
            // directions of this force can only be CW or CCW
            force = currentLastDifference * force > 0 ? -1 * force : force;
        }

        return force;
    }


    public float makeRay(Vector3 direction, float length, float bias)
    {
        float force = 0;
        // Fires a ray in the specified direction vector, and returns the angle to deviate if an obstacle is blocking

        // Draw for debugging
        Debug.DrawRay(transform.position, length * direction);

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, length);

        if (hitSomething)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            float obstacleAngle = calculateAngle(transform.position, hit.point);
            drawAngleArrow(obstacleAngle, Color.magenta);

            // force should be proportional to 		
            // obstacle distance from car- If far away, doesnt matter immediately, so inversely prop
            // difference between obstacle angle and last angle- If the obstacle angle is not related to path, doesnt matter, inversely
            // difference between currentAngle and lastAngle, if currentAngle is close to lastAngle, means on path. directly
            float distance = (hit.point - transform.position).magnitude;
            float obsLastDifference = calculateAngleDifference(obstacleAngle, lastAngle);
            float currentLastDifference = calculateAngleDifference(currentAngle, lastAngle);

            // should eventually be proportional to car length and currentVelocity
            float minAvoidDist = 1.5f;
            float distanceWeight = 5f;
            float cLWeight = 2f;

            // care for direction 
            if (distance < minAvoidDist)
            {
                float distanceProportion = Mathf.Exp((1 / Mathf.Abs(distanceWeight)) * 1 / (distance));
                float obsLastDifferenceProportion = -Mathf.Abs(obsLastDifference);
                float currentLastDifferenceProportion = Mathf.Exp((1 / cLWeight) * (-Mathf.Abs(currentLastDifference)));
                force = distanceProportion + obsLastDifferenceProportion + currentLastDifferenceProportion;
            }
            // if currentLastDifference * force > 0 then they have the sign polarity
            // if not then change polarity of force to match
            // directions of this force can only be CW or CCW
            force = currentLastDifference * force > 0 ? -1 * force : force;
        }

        return force;
    }

    public float calculateAngleDifference(float fromAngle, float toAngle)
    {
        // returns the closest difference between two angles
        float toChange = 0;
        if (fromAngle != toAngle)
        {
            //Angle measurement of difference1 is based off the angle of fromAngle and toAngle

            float difference1 = Mathf.Abs(fromAngle - toAngle);
            float difference2 = 2 * Mathf.PI - difference1;

            if (fromAngle < toAngle)
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
            else if (fromAngle > toAngle)
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
        float toChange = calculateAngleDifference(currentAngle, lastAngle);
        return toChange / 2;
    }

    // calculates restoring force based on the model
    // mx''(t) + cx'(t) + kx(t) = 0
    // (Dampened harmonic motion)
    public float springRestoringForce()
    {
        float force = 0;
        float difference = calculateAngleDifference(currentAngle, lastAngle);
        float c = 3f, k = 0.1f;
        if (Mathf.Abs(difference) > 0.01)
        {
            float dampening = c * currentAngularVelocity;
            float springForce = k * -1 * difference;
            force = -(dampening + springForce);
        }

        if (Input.GetMouseButtonDown(1))
        {
            print("difference is " + difference + " force is " + force);
            print("currentangle is " + Mathf.Rad2Deg * currentAngle + " lastangle is " + Mathf.Rad2Deg * lastAngle);
        }

        return force;
    }

    public void checkNewTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Acquire mouse positioning
            Vector3 newTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            lastTarget = newTarget;
            lastAngle = calculateAngle(transform.position, lastTarget);
        }
    }

    public float sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-1 * x));
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
