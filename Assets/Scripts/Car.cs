using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra.Double;

public class Car : MonoBehaviour {

    Rigidbody r;
    public Vector3 lastTarget;
    float currentAngle = 0;
    float currentAngularVelocity = 0;
    float currentAngularAcceleration = 0;
    float speed = 1f;
    float lastAngle = 0;



    //Dictionary<string, double> stats;
    int t;

    public GameObject target;

    float leftWhiskerForce = 0;
    float rightWhiskerForce = 0;
    carStats stats;
    void Start()
    {
        stats = new carStats(this, new string[] { "CurrentAngle", "CurrentAngularVelocity", "currentAngularAcceleration", "leftWhiskerForce", "rightWhiskerForce" });
        r = GetComponent<Rigidbody>();
        r.velocity = new Vector3(0, 0, 0);

        t = 0;


        
        //stats = new Hashtable();
        //stats["CurrentAngle"] = 0.0;
        //stats["CurrentAngularVelocity"] = 0.0;
        //stats["CurrentAngularAccerleration"] = 0.0;
        //stats["leftWhiskerForce"] = 0.0;
        //stats["rightWhiskerForce"] = 0.0;

        target = GameObject.Find("Target");
        if (target != null)
        {
            lastTarget = target.transform.position;
        }
    }

    private void OnGUI()
    {
        //int i = 1;
        //foreach (KeyValuePair<string, double> stat in stats)
        //{
        //    GUI.Label(new Rect(40, i*40, 100, 100), stat.Key + " " + (rounder(Mathf.Rad2Deg,2) * stat.Value), guiStyle);
        //    i++;
        //}
    }

    double rounder(double toRound, int decimalPlaces)
    {
        float degree = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round((float)toRound * degree) / degree;
    }

    void Update() {
        
        // updating stats every 1/100 seconds
        if (t % 10 == 0)
        {
            Dictionary<string, double> newStats= new Dictionary<string, double>();
            newStats["currentAngle"] = rounder(currentAngle, 1);
            newStats["currentAngularVelocity"] = rounder(currentAngularVelocity, 2);
            newStats["currentAngularAccerlation"] = rounder(currentAngularAcceleration, 4);
            newStats["leftWhiskerForce"] = leftWhiskerForce;
            newStats["rightWhiskerForce"] = rightWhiskerForce;
            //double[] statinfo = new double[] {rounder(currentAngle, 1), rounder(currentAngularVelocity,2),
            //    rounder(currentAngularAcceleration, 4), leftWhiskerForce, rightWhiskerForce};
            ArrayList keys = new ArrayList(newStats.Keys);
            for(int i = 0; i < newStats.Keys.Count; i++)
            {
                stats.updateStat(System.Convert.ToString(keys[i]), newStats[System.Convert.ToString(keys[i])]);
            }
        }

        leftWhiskerForce = 0;
        rightWhiskerForce = 0;


        // First do checks (inputs of the model)

        float currentLastDifference = calculateAngleDifference(currentAngle, lastAngle);
        // adjusts lastAngle to point to lastTarget
        if (lastTarget != Vector3.zero) { 
            if (t % 100 == 0)
            {
                lastAngle = calculateAngle(transform.position, lastTarget);
            }
            t += 1;
        }
        // If the left mouse is clicked, set the last angle positioning
        checkNewTarget();

        //float restoringForce = avgRestoringForce();

        //inputs 
        Vector springForceInput = DenseVector.OfArray(new double[] {currentLastDifference, currentAngularVelocity});
        Vector springForceWeights = DenseVector.OfArray(new double[] {-1/10f, 1f});
        float restoringForce = springRestoringForce(springForceInput, springForceWeights) ;
        
        // raycast scanning
        float AngularForce = 0;

        // currentWeights affect distance, obsLastDifference, currentLastDifference in that order

        // force should be proportional to 		
        // obstacle distance from car - If far away, doesnt matter immediately, so inversely prop
        // difference between obstacle angle and last angle - If the obstacle angle is not related to path, doesnt matter, inversely
        // difference between currentAngle and lastAngle - if currentAngle is close to lastAngle, means on path. directly


        ArrayList rays = new ArrayList();


        // left whisker
        Vector leftWhiskerWeights = DenseVector.OfArray(new double[] { 1f, 1/100f, 1/100f });
        System.ValueTuple<float, float, Vector> leftWhisker = new System.ValueTuple<float, float, Vector>(currentAngle - Mathf.PI / 4, 1.5f, leftWhiskerWeights);
        rays.Add(leftWhisker);

        //// front ray
        Vector frontRayWeights = DenseVector.OfArray(new double[] { 1 / 5f, 1f, 1 / 2f });
        System.ValueTuple<float, float, Vector> frontRay = new System.ValueTuple<float, float, Vector>(currentAngle, 1.5f, frontRayWeights);
        rays.Add(frontRay);

        // right whisker 
        Vector rightWhiskerWeights = leftWhiskerWeights;
        System.ValueTuple<float, float, Vector> rightWhisker = new System.ValueTuple<float, float, Vector>(currentAngle + Mathf.PI / 4, 1.5f, rightWhiskerWeights);
        rays.Add(rightWhisker);



        int j = 0;

        foreach (System.ValueTuple<float, float, Vector> ray in rays)
        {
            Vector3 hitPoint = fireRay(ray.Item1, ray.Item2);
            if (hitPoint != Vector3.zero)
            {
                float obstacleAngle = calculateAngle(transform.position, hitPoint);
                float distance = (hitPoint - transform.position).magnitude;
                float obsLastDifference = calculateAngleDifference(obstacleAngle, lastAngle);

                Vector inputs = DenseVector.OfArray(new double[] { distance, -Mathf.Abs(obsLastDifference), -Mathf.Abs(currentLastDifference)});
                float force = rayForceCalculation(ray.Item3, inputs);
                // care for direction, want to turn in the closest direction to get back on path
                // if obsLastDifference * force > 0 then they have the sign polarity
                // if not then change polarity of force to match
                // directions of this force can only be CW or CCW
                force = - (obsLastDifference * force > 0 ? -1 * force : force);

                AngularForce += force/1000;

                //string word = j == 1 ? "left" : "right";
                //print(word + " whisker " + force / 1000);
                if (j == 1)
                {
                    leftWhiskerForce = force;
                }
                else
                {
                    rightWhiskerForce = force;
                }
                Debug.DrawLine(transform.position, hitPoint, Color.red);




            }
            j++;
        }

        // once all checks are done, do calculations with weights that prioritize the most critical action (outputs)

        float restoringForceWeight = 100;
        AngularForce += restoringForce / restoringForceWeight;

        float moment = 1;
        // Newton's second Law
        currentAngularAcceleration = AngularForce / moment;
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



    }
    // calculates force for given ray with givens inputs and weights
    public float rayForceCalculation(Vector inputs, Vector weights)
    {
        float force = 0;
        float total = System.Convert.ToSingle(inputs.DotProduct(weights));
        force = Mathf.Exp(total);
        return force;
    }

    public Vector3 fireRay(float angle, float length)
    {
        // Fires a ray directly in the given direction, and returns the hit location
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        // Draw for debugging
        Debug.DrawRay(transform.position, length * direction);

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, length);
        return hit.point;
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

    // calculates restoring force based on the model
    // mx''(t) + cx'(t) + kx(t) = 0
    // (Dampened harmonic motion)
    public float springRestoringForce(Vector inputs, Vector weights)
    {
        float force = 0;
        //float difference = calculateAngleDifference(currentAngle, lastAngle);
        //float c = 3f, k = -1 * 0.1f;
        //Vector inputs = DenseVector.OfArray(new double[]{currentAngularVelocity, difference}));
        if (Mathf.Abs((float) inputs[0]) > 0.01)
        {
            //float dampening = c * currentAngularVelocity;
            //float springForce = k * -1 * difference;
            //force = -(dampening + springForce);
            double total = inputs.DotProduct(weights);
            force = -1 * (float) (total);

        }

        return force;
    }

    public float avgRestoringForce()
    {
        float toChange = calculateAngleDifference(currentAngle, lastAngle);
        return toChange / 2;
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

    public Vector3 getLastTarget() { return this.lastTarget; }

    public float getLastAngle() { return this.lastAngle; }

    public float getCurrentAngle() { return this.currentAngle; }

    public float getCurrentAngularVelocity() { return this.currentAngularVelocity; }

    public float getCurrentAngularAcceleration() { return this.currentAngularAcceleration; }

    //public float frontObstacleDeviation(Vector3 obsPos)
    //{
    //    // should eventually be proportional to car length and currentVelocity
    //    float force = 0;
    //    if (obsPos != Vector3.zero)
    //    {
    //        Debug.DrawLine(transform.position, obsPos, Color.red);
    //        float obstacleAngle = calculateAngle(transform.position, obsPos);
    //        //drawAngleArrow(obstacleAngle, Color.magenta);

    //        // force should be proportional to 		
    //        // obstacle distance from car - If far away, doesnt matter immediately, so inversely prop
    //        // difference between obstacle angle and last angle - If the obstacle angle is not related to path, doesnt matter, inversely
    //        // difference between currentAngle and lastAngle - if currentAngle is close to lastAngle, means on path. directly
    //        float distance = (obsPos - transform.position).magnitude;
    //        float obsLastDifference = calculateAngleDifference(obstacleAngle, lastAngle);
    //        float currentLastDifference = calculateAngleDifference(currentAngle, lastAngle);
    //        Vector weights = DenseVector.OfArray(new double[] { 1/5f, 1f, 1/2f });
    //        Vector inputs = DenseVector.OfArray(new double[] { distance, -Mathf.Abs(obsLastDifference), -Mathf.Abs(currentLastDifference)});
    //        float total = System.Convert.ToSingle(inputs.DotProduct(weights));
    //        force = Mathf.Exp(total);

    //        // care for direction, want to turn in the closest direction to get back on path
    //        // if obsLastDifference * force > 0 then they have the sign polarity
    //        // if not then change polarity of force to match
    //        // directions of this force can only be CW or CCW
    //        force = obsLastDifference * force > 0 ? -1 * force : force;
    //    }

    //    return force;

    //float minAvoidDist = 1.5f;
    //if (distance < minAvoidDist)
    //{
    //float distanceWeight = 1/5f;
    //float cLWeight = 1/2f;
    //    //float distanceProportion = distance * distanceWeight;
    //    //float obsLastDifferenceProportion = -Mathf.Abs(obsLastDifference) * 1;
    //    //float currentLastDifferenceProportion = cLWeight * -Mathf.Abs(currentLastDifference);
    //    //float total1 = distanceProportion + obsLastDifferenceProportion + currentLastDifferenceProportion;
    //}
    //}

    //public Vector3 checkFrontObstacle()
    //{
    //    // Fires a ray directly in front of this object, and returns the angle to deviate if an obstacle is blocking
    //    float length = 10f;
    //    Vector3 direction = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);

    //    // Draw for debugging
    //    Debug.DrawRay(transform.position, length * direction);

    //    RaycastHit hit;
    //    bool hitSomething = Physics.Raycast(transform.position, direction, out hit, length);
    //    return hit.point;
    //}
}
