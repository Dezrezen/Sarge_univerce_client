using System.Collections;
using System.Collections.Generic;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;

public class FighterRadius : MonoBehaviour
{
    [SerializeField] private float defaultWidthOfRangeOfDetection = 240f;
    [SerializeField] private float defaultHeightOfRangeOfDetection = 116.8f;

    [SerializeField] private SpriteRenderer _rangeSprite;
    [SerializeField] private RectTransform _mask;


    [SerializeField] private bool isRangeSpriteVisible = false;
    
    public void SetRadius(UnitCategory unitData)
    {
        int range = ((IRange)unitData).GetRange();
        var width = defaultWidthOfRangeOfDetection * range;
        var height = defaultHeightOfRangeOfDetection * range;
        if (_rangeSprite)
        {
            //TODO: think about it - do we need to show a range of attack or not????
            ///in the Debug Mode we can see the range of attack from the FighterPath - OnDrawGizmos()
            _rangeSprite.transform.localScale = Vector3.one;//new Vector3(width, height, 1);
            _rangeSprite.enabled = isRangeSpriteVisible;    
        }
        

        if (_mask)
        {
            _mask.localScale = Vector3.one;//_rangeSprite.transform.localScale;
            _mask.gameObject.SetActive(true);
        }
    }
}
