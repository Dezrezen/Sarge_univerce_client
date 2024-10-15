using System.Collections;
using UnityEngine;

public class ConstructorController : MonoBehaviour
{
    public string walkAnim = "Walk", actionAnim = "Plow";
    public float speed = 55.0f, mass = 5.0f;

    public ConstructionPath path;
    private AnimController builderAnimController; //constrolls the builder during construction

    private int curPathIndex;
    private BuilderState currentState = BuilderState.Walk;
    private float curSpeed, pathLength;
    private Vector3 targetPoint, velocity;

    private void Start()
    {
        builderAnimController = GetComponent<AnimController>();
        pathLength = path.Length;
        curPathIndex = 0;

        var pos = transform.position;
        var zDepth = pos.z - 10 / (pos.y + 3300);
        transform.position = new Vector3(pos.x, pos.y, zDepth); //zDepth correction at instantiation
        //velocity = Vector3.zero;
    }

    private void Update()
    {
        switch (currentState)
        {
            case BuilderState.Walk:
                Walk();
                break;
        }
    }

    private void Walk()
    {
        curSpeed = speed * Time.deltaTime;
        targetPoint = path.GetPoint(curPathIndex);

        //if path point reached, move to next point
        if (Vector3.Distance(transform.position, targetPoint) <
            path.Radius)
        {
            currentState = BuilderState.Action;
            StartCoroutine("Action");

            if (curPathIndex < pathLength - 1) curPathIndex++;
            else curPathIndex = 0;
        }

        velocity += Steer(targetPoint);
        transform.position += velocity;
    }

    private IEnumerator Action()
    {
        ChangeAnimation(actionAnim);
        yield return new WaitForSeconds(4);
        ChangeAnimation(walkAnim);
        currentState = BuilderState.Walk;
    }

    private void ChangeAnimation(string action)
    {
        var direction = " ";

        switch (action)
        {
            case "Walk":
                switch (curPathIndex)
                {
                    case 0:
                        direction = "SW";
                        break;
                    case 1:
                        direction = "NW";
                        break;
                    case 2:
                        direction = "NE";
                        break;
                    case 3:
                        direction = "SE";
                        break;
                }

                break;

            case "Plow":
                switch (curPathIndex)
                {
                    case 0:
                        direction = "N";
                        break;
                    case 1:
                        direction = "E";
                        break;
                    case 2:
                        direction = "S";
                        break;
                    case 3:
                        direction = "W";
                        break;
                }

                break;

            case "Seed":
                switch (curPathIndex)
                {
                    case 0:
                        direction = "N";
                        break;
                    case 1:
                        direction = "E";
                        break;
                    case 2:
                        direction = "S";
                        break;
                    case 3:
                        direction = "W";
                        break;
                }

                break;
        }

        //break;
        //}


        builderAnimController.ChangeAnim(action);
        builderAnimController.Turn(direction);
        builderAnimController.UpdateCharacterAnimation();
    }

    public Vector3 Steer(Vector3 target)
    {
        var desiredVelocity = target - transform.position;
        var dist = desiredVelocity.magnitude;

        desiredVelocity.Normalize();

        if (dist < 10.0f)
            desiredVelocity *= curSpeed * (dist / 10.0f);
        else
            desiredVelocity *= curSpeed;

        var steeringForce = desiredVelocity - velocity;
        var acceleration = steeringForce / mass;

        transform.position += velocity;

        return acceleration;
    }

    //public bool isLooping = true;

    private enum BuilderState
    {
        Walk,
        Action
    }
}