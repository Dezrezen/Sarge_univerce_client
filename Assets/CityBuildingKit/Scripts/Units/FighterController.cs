using System;
using System.Collections;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;
using UnityEngine.Serialization;

public class FighterController : MonoBehaviour
{
    public enum UnitState
    {
        PathWalk,
        Attack,
        Idle,
        Disperse
    } //controlls all soldiers

    public FighterPath path;
    public float speed = 30.0f, mass = 5.0f, unitZ = -3;

    public Vector3 targetPoint, velocity;

    [FormerlySerializedAs("currentState")] public UnitState _currentState = UnitState.Idle;

    public bool hasShadow = true; //if shadow is on a separate sprite

    private int
        //for progress bar percentage
        curPathIndex;

    private float curSpeed, pathLength;

    private string direction = "N";
    private AnimController fighterAnimController, shadowAnimController;

    private tk2dUIProgressBar healthBar;

    private int
        maxLife = 100;

    private bool
        pause,
        breakAction;


    public UnitState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    public UnitCategory UnitData { get; set; }

    public int Life { get; set; }

    public int DamagePoints { get; set; } = 0;

    public Shooter Shooter { get; private set; }

    public Vector2 GrassTargetCoords { get; set; } = new();

    public GrassSelector GrassTarget { get; set; }

    private Action _onTargetDestroyed;

    private void Awake()
    {
        Shooter = GetComponent<Shooter>();
        if (!Shooter) Debug.LogError("Can't get the Shooter component'");
    }

    private void Start()
    {
        if (UnitData == null) return;

        path = GetComponent<FighterPath>();

        speed = UnitData.GetMovementSpeed();
        path.RangeOfAttack = UnitData.GetRange();
        maxLife = UnitData.HP;
        Life = UnitData.HP;
        GetComponent<Shooter>().fireRate = UnitData.GetFireRate();


        fighterAnimController = GetComponent<AnimController>();

        if (hasShadow)
            shadowAnimController = transform.GetChild(2).GetComponent<AnimController>();


        healthBar = GetComponentInChildren<tk2dUIProgressBar>();

        pathLength = path ? path.Length : 0;
        curPathIndex = 0;
        velocity = Vector3.zero; //maintain position till you get the first path point
        
        Attack();
    }

    private void FixedUpdate()
    {
        if (!pause)
        {
            if (GrassTarget && GrassTarget.isDestroyed)
            {
                GrassTarget = null;
                RevertToIdle();
                _onTargetDestroyed?.Invoke();
            }

            switch (CurrentState)
            {
                case UnitState.PathWalk:
                    PathWalk();
                    break;
            }
        }
    }


    public void FindAndAttack()
    {
        path.pathFinder.ResetPath();
        Helios.Instance.SearchAlivePreferredBuildings(EntityType.Any, transform.position, this);
        path.pathFinder.FindPath();
        ChangeTarget();
        RevertToPathWalk();
    }

    public void FindAndAttackPreferred()
    {
        path.pathFinder.ResetPath();
        Helios.Instance.SearchAlivePreferredBuildings(UnitData.GetPreferredTarget(), transform.position, this);
        path.pathFinder.FindPath();
        ChangeTarget();
        RevertToPathWalk();
    }

    private void Prepare()
    {
        if (UnitData.GetPreferredTarget() != EntityType.Any)
            Helios.Instance.SearchAlivePreferredBuildings(UnitData.GetPreferredTarget(), transform.position, this);
        else
            Helios.Instance.FindNearestBuilding();

        path.pathFinder.FindPath();
        ChangeTarget();
        RevertToPathWalk();
    }

    public void Hit(int damagePoints, Action callback)
    {
        Life -= damagePoints;
        if (callback != null) callback();

        if (Life > 0)
            healthBar.Value = Life / (float)maxLife;
        else
            Destroy(gameObject);
    }

    public void Pause()
    {
        pause = true;
    }

    private void UpdatePath()
    {
        pause = true;
        path.UpdatePath();
        pathLength = path.Length;

        /*
        skip path entry point because 
        1.possibly inside an obstacle block, and the units go up over the rubble until they reach the middle sprite "cut" edge
        2.units appear to "go back" when they receive a new order  

        since animations have 8 angles tops, this cam create a slight/brief "moonwalking" -
        the direction of the walk animation doesn't match perfectly the unit direction 

        since at the beginning the unit goes 2 grid squares till the direction is checked again, this is
        corrected with a delayed latechangeanimation, so the movement/direction doesn't appear all wrong
        */

        if (pathLength > 1)
            curPathIndex = 1;
        else
            curPathIndex = 0;
        StartCoroutine(LateChangeAnimation(0.5f));

        pause = false;
    }

    private void PathWalk()
    {
        curSpeed = speed * 0.016f; //Time.deltaTime creates a speed hickup - replaced with constant

        //to go straight to AITarget targetPoint = path.GetEndPoint();

        targetPoint = path.GetPoint(curPathIndex);


        var pointForAttack = path.GetEndPoint();
        var distance = Vector3.Distance(transform.position, pointForAttack);

        var unitRange = 388;

        if (distance + GridManager.instance.gridCellSize < unitRange
           )
        {
            RevertToIdle();
            CurrentState = UnitState.Attack;
            GetComponent<Shooter>().shoot = true;
            StartCoroutine(LateChangeAnimation(0.1f));
            return;
        }


        //if radius to path point reached, move to next point
        var dist = Vector3.Distance(transform.position, targetPoint);
        if (dist <
            path.Radius)
        {
            if (curPathIndex < pathLength - 1)
            {
                curPathIndex++;
            }
            else
            {
                CurrentState = UnitState.Attack;
                GetComponent<Shooter>().shoot = true;
            }

            StartCoroutine(LateChangeAnimation(0.1f));
        }

        velocity += Steer(targetPoint);

        transform.position += velocity;
    }

    public Vector3 Steer(Vector3 target)
    {
        var pos = transform.position;
        var desiredVelocity = target - pos;
        var dist = desiredVelocity.magnitude; //square root of (x*x+y*y+z*z)

        desiredVelocity.Normalize(); //normalized, a vector keeps the same direction but its length is 1.0

        if (dist < 10.0f)
            desiredVelocity *= curSpeed * (dist / 10.0f); //slow down close to target
        else
            desiredVelocity *= curSpeed;

        var steeringForce = desiredVelocity - velocity;
        var acceleration = steeringForce / mass;

        transform.position +=
            velocity; //!!! Disregarding z can make character go back and forth below point, unable to "touch" it

        return acceleration;
    }

    public void ChangeTarget()
    {
        IngameUpdatePath();
    }

    private void IngameUpdatePath()
    {
        UpdatePath();
        StartCoroutine(LateChangeAnimation(0.1f));
    }

    private void ChangeToPathWalk()
    {
        if (CurrentState != UnitState.Attack || breakAction)
        {
            CurrentState = UnitState.PathWalk;
            StartCoroutine(LateChangeAnimation(0.1f));
            breakAction = false;
        }
    }

    public void RevertToPathWalk() //called by BattleProc
    {
        breakAction = true;
        GetComponent<Shooter>().shoot = false;
        ChangeToPathWalk();
    }

    public void RevertToIdle()
    {
        CurrentState = UnitState.Idle;
        GetComponent<Shooter>().shoot = false;
        ChangeAnimation();
    }

    public IEnumerator LateChangeAnimation(float time)
    {
        yield return new WaitForSeconds(time); //0.1f standard, 0.5 for late
        SetDirection();
    }


    private void ChangeAnimation()
    {
        SetDirection();
    }

    private void SetDirection()
    {
        switch (CurrentState)
        {
            case UnitState.PathWalk:
                SpeedToDirection();
                fighterAnimController.ChangeAnim("Walk");
                if (hasShadow)
                    shadowAnimController.ChangeAnim("Walk_Shadow");
                break;

            case UnitState.Attack:
                SetRelativeDirection();
                fighterAnimController.ChangeAnim("Attack");
                if (hasShadow)
                    shadowAnimController.ChangeAnim("Attack_Shadow");
                break;

            case UnitState.Idle:
                fighterAnimController.ChangeAnim("Idle");
                if (hasShadow)
                    shadowAnimController.ChangeAnim("Idle_Shadow");
                break;
        }

        fighterAnimController.Turn(direction);
        fighterAnimController.UpdateCharacterAnimation();

        if (hasShadow)
        {
            switch (CurrentState)
            {
                case UnitState.PathWalk:
                    shadowAnimController.ChangeAnim("Walk_Shadow");
                    break;

                case UnitState.Attack:
                    shadowAnimController.ChangeAnim("Attack_Shadow");
                    break;

                case UnitState.Idle:
                    shadowAnimController.ChangeAnim("Idle_Shadow");
                    break;
            }

            shadowAnimController.Turn(direction);
            shadowAnimController.UpdateCharacterAnimation();
        }
    }

    private void SpeedToDirection() //threshdold 0.05f for "if" (if run at next update cycle) 0.2f for delayed coroutine
    {
        if (Mathf.Abs(velocity.x) > 0.2f) //high X speed
        {
            if (Mathf.Abs(velocity.y) > 0.2f) //high y speed
            {
                if (velocity.x > 0)
                {
                    if (velocity.y > 0) direction = "NE";
                    else direction = "SE";
                }

                else
                {
                    if (velocity.y > 0) direction = "NW";
                    else direction = "SW";
                }
            }
            else //low y speed
            {
                if (velocity.x > 0) direction = "E";
                else direction = "W";
            }
        }

        else
        {
            if (Mathf.Abs(velocity.y) > 0.2f) //high y speed
            {
                if (velocity.y > 0) direction = "N";
                else direction = "S";
            }
            else //low y speed
            {
                if (velocity.x > 0)
                {
                    if (velocity.y > 0) direction = "NE";
                    else direction = "SE";
                }

                else
                {
                    if (velocity.y > 0) direction = "NW";
                    else direction = "SW";
                }
            }
        }
    }

    private void SetRelativeDirection() //the unit must face the center of the target
    {
        var targetCenter = new Vector3(GrassTarget.transform.position.x, GrassTarget.transform.position.y, 2);

        var xRelativePos = targetCenter.x - transform.position.x;
        var yRelativePos = targetCenter.y - transform.position.y;

        if (xRelativePos > 100)
        {
            if (yRelativePos > 100) direction = "NE";
            else if (yRelativePos < -100) direction = "SE";
            else direction = "E";
        }

        else if (xRelativePos < -100) //direction = "W";
        {
            if (yRelativePos > 100) direction = "NW";
            else if (yRelativePos < -100) direction = "SW";
            else direction = "W";
        }

        else
        {
            if (yRelativePos > 0) direction = "N";
            else direction = "S";
        }
    }

    public void SetTarget(GrassSelector target, Action callback)
    {
        GrassTarget = target;
        _onTargetDestroyed = callback;
    }

    public void Attack()
    {
        path.pathFinder.ResetPath();
        Helios.Instance.SelectTarget(transform.position, this);
        path.pathFinder.FindPath();
        ChangeTarget();
        RevertToPathWalk();
    }
}