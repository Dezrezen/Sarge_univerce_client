using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Weapon;
using UnityEngine;

public class TurretControllerBase : MonoBehaviour
{
    public GameObject projectilePf;

    [SerializeField] protected StructureSelector _structureSelector;

    [HideInInspector] public GameObject targetOb; //try to remove this var

    public List<GameObject> targetList = new();

    //[HideInInspector]
    public bool fire;
    public GameObject flashLight;
    private readonly float threshold = 0.99f;
    private readonly float transitionSpeed = 0.1f;

    private readonly float turretRotSpeed = 2.0f;
    private readonly float turretSearchSpeed = 0.05f;
    private Projectile _projectile;
    private BaseStoreItemData _structure;


    private int currentTargetIndex;
    protected Vector3 currentTargetPosition;

    //Bullet shooting rate
    protected float
        fireRate,
        elapsedTime;

    //Searchlight

    private bool focus, finished = true; //refers to flashlight zoom in/out

    private bool randomRotationFinished = true;
    private Vector3 randomTargetPosition;

    protected Component soundFx;
    private float startPoint, endPoint, startTime, transitionLength;

    protected Transform
        turretTr,
        projectileSpawnPointTr;


    protected void InitializeComponents()
    {
        _structureSelector = GetComponent<StructureSelector>();

        var levels = ShopData.Instance.GetLevels(_structureSelector.Id, _structureSelector.CategoryType);
        if (levels != null)
        {
            _structure = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _structureSelector.Level);
            if (_structure != null)
                fireRate = ((IFireRate)_structure).GetFireRate();
            else
                Debug.LogError("Can't get structure");
        }
        else
        {
            Debug.LogError("Can't get levels for structure");
        }


        turretTr = transform.Find("Turret");
        if (turretTr) projectileSpawnPointTr = turretTr.Find("Tip");

        soundFx = GameObject.Find("SoundFX").GetComponent<SoundFX>();
    }

    public void Aim()
    {
        Rotate();
    }

    protected void Search()
    {
        SearchRotate();
        Focus();
    }

    protected void UpdateTarget()
    {
        if (targetList.Count == 0)
        {
            fire = false;
            return;
        }

        if (targetList[0] != null)
            currentTargetPosition = targetList[0].transform.position;
        else
            targetList.Remove(targetList[0]);
    }


    private void Rotate() //this rotates the 3d Turret
    {
        var targetDir = turretTr ? turretTr.position - currentTargetPosition : new Vector3();

        if (targetDir != Vector3.zero)
        {
            var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 10; // 
            turretTr.rotation = Quaternion.Slerp(turretTr.rotation,
                Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * turretRotSpeed); //slow gradual rotation					
        }
    }

    private void SearchRotate()
    {
        if (randomRotationFinished)
        {
            if (!turretTr)
                return;
            float rnd = Random.Range(-2000, 2000);
            randomTargetPosition = turretTr.position + new Vector3(rnd, rnd, rnd);
            randomRotationFinished = false;
            StartCoroutine("NewRandomSearch");
        }
        else
        {
            var angle = Mathf.Atan2(randomTargetPosition.y, randomTargetPosition.x) * Mathf.Rad2Deg + 10; //

            turretTr.rotation = Quaternion.Slerp(turretTr.rotation,
                Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * turretSearchSpeed); //slow gradual rotation					
        }
    }

    private void Focus()
    {
        if (finished)
        {
            if (focus)
            {
                startPoint = 1.0f;
                endPoint = 0.4f;
            }
            else
            {
                startPoint = 0.4f;
                endPoint = 1.0f;
            }

            transitionLength = Mathf.Abs(startPoint - endPoint);
            startTime = Time.time;
            finished = false;
        }
        else
        {
            RunTransition();
        }
    }

    private void RunTransition()
    {
        var transitionCovered = (Time.time - startTime) * transitionSpeed; //startTime = Time.time;
        var transitionFraction = transitionCovered / transitionLength;
        var v = Mathf.Lerp(startPoint, endPoint, transitionFraction); //lerp is clamped to 0,1

        var flashLightLocalScale = flashLight ? flashLight.transform.localScale : new Vector3();
        var currentScale = flashLightLocalScale;
        if (flashLight)
        {
            flashLight.transform.localScale = new Vector3(currentScale.x, v, currentScale.z);
            if (transitionFraction >= threshold)
            {
                focus = !focus;
                finished = true;
            }
        }
    }

    private IEnumerator NewRandomSearch() // the coroutine won't survive the the editor window loosing focus 
    {
        yield return new WaitForSeconds(10);
        randomRotationFinished = true;
    }

    public void Fire()
    {
        ((SoundFX)soundFx).CannonFire();
        LaunchProjectile();
    }

    protected void LaunchProjectile()
    {
        ((SoundFX)soundFx).CannonFire();

        var pos = projectileSpawnPointTr ? projectileSpawnPointTr.position : new Vector3();
        var rot = projectileSpawnPointTr ? projectileSpawnPointTr.rotation : new Quaternion();

        var projectile = Instantiate(projectilePf, pos, rot);
        _projectile = projectile.GetComponent<Projectile>();
        _projectile.DamagePoints = ((IDamagePoints)_structure).GetDamagePoints();
        _projectile.DamageType = ((WeaponCategory)_structure).GetDamageType();
    }

    public void AddTarget(GameObject target)
    {
        targetList.Add(target);
    }

    public void RemoveTarget(GameObject target)
    {
        targetList.Remove(target);
    }
}