using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;

public class PerimeterColliderMine2D : MonoBehaviour
{
    //used to color the grass red when in collision

    public int collisionCounter; //keeps track of how many other grass patches this one is overlapping

    public bool isExploded;

    public GameObject ExplosionPf;

    [SerializeField] private StructureSelector _structureSelector;
    [SerializeField] private SpriteRenderer _rangeSpriteRenderer;
    [SerializeField] private RectTransform _mask;
    [SerializeField] private float defaultWidthOfRangeOfDetection = 240f;
    [SerializeField] private float defaultHeightOfRangeOfDetection = 116.8f;


    private readonly int
        explosionZ = -6;

    private readonly int
        ghostZ = -9;

    private readonly List<GameObject> victims = new();

    private readonly int
        zeroZ = 0;

    private FighterController _fighterController;
    private int _index;
    private Vector3 _positionForInstantiation;

    private Helios helios;

    private SoundFX soundFX;

    private void Awake()
    {
        _structureSelector.UpdateStructureLevelEvent.AddListener(UpdateStructureLevelHandler);
    }

    private void Start()
    {
        soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
        helios = GameObject.Find("Helios").GetComponent<Helios>();

        _structureSelector.ReSelectAction.AddListener(ReSelect);
        _structureSelector.DeSelectAction.AddListener(DeSelect);


        //TODO: need to hide the mine for an enemy.
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Unit") && !isExploded)
        {
            collisionCounter++;
            victims.Add(collider.gameObject);
            StartCoroutine("Explode");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Unit" && !isExploded)
        {
            collisionCounter--;
            victims.Remove(collider.gameObject);

            if (collisionCounter == 0)
            {
                victims.Clear();
                StopCoroutine("Explode"); //bomb aborts explosion if targets have moved on
            }
        }
    }

    private void ReSelect()
    {
        ShowRange(true);
    }

    private void DeSelect()
    {
        ShowRange(false);
    }

    private void UpdateStructureLevelHandler()
    {
        SetRangeOfDetection();
    }

    private IEnumerator Explode()
    {
        //TODO: need to show the mine for an enemy.
        yield return new WaitForSeconds(GetTimeToExplosion());
        isExploded = true;
        soundFX.BuildingExplode();
        ExplosionPf.SetActive(true);

        foreach (var unit in victims)
            if (unit != null)
            {
                _fighterController = unit.GetComponent<FighterController>();
                _positionForInstantiation = unit.transform.position;
                _index = unit.GetComponent<Selector>().index;
                _fighterController.Hit(GetDamage(), Instantiate);
            }

        victims.Clear();

        //TODO: after explosion animation  - need to hide the mine for an enemy.
    }

    private void Instantiate()
    {
        helios.KillUnit(_index);
    }

    private void SetRangeOfDetection()
    {
        if (_structureSelector)
        {
            var data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Weapon,
                _structureSelector.Level);
            if (data != null && data is IRange)
            {
                var width = defaultWidthOfRangeOfDetection * ((IRange)data).GetRange();
                var height = defaultHeightOfRangeOfDetection * ((IRange)data).GetRange();
                transform.localScale = new Vector3(width, height, 1);
                if (_mask) _mask.localScale = transform.localScale;
                ShowRange(true);
            }
        }
    }

    private int GetDamage()
    {
        if (_structureSelector)
        {
            var data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Weapon,
                _structureSelector.Level);
            if (data != null && data is IDamagePoints)
                return ((IDamagePoints)data).GetDamagePoints();
            return 0;
        }

        return 0;
    }


    private float GetTimeToExplosion()
    {
        var data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Weapon,
            _structureSelector.Level);
        if (data != null && data is IRange) return ((ITimeToExplosion)data).GetTimeToExplosion();
        return 0f;
    }

    private void ShowRange(bool value)
    {
        _rangeSpriteRenderer.enabled = value;
        _mask.gameObject.SetActive(value);
    }
}