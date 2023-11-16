using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TouchOptions", menuName = "Scriptable Object/UrgTouchDetectorSettingData", order = int.MaxValue)]
public class UrgTouchDetectorSettingData : ScriptableObject
{
    [SerializeField] public Vector2 touchGrid;
    
    [SerializeField] public float touchCheckSpeed = 10;
    [SerializeField] public bool touchCheckInGridArea = false;
    
    [SerializeField] public float touchMergeDistanceX = 0.1f;
    [SerializeField] public float touchMergeDistanceY = 0.1f;
    [SerializeField] public float touchMergeDistanceXForMerge = 0.1f;
    [SerializeField] public float touchMergeDistanceYForMerge = 0.1f;
    
    [SerializeField] public float touchClickableDistance = 0.1f;
    [SerializeField] public float touchCancelDuration = 0.1f;
    [SerializeField] public float touchInvokeDuration = 0.15f;
    
    [SerializeField, Range(0,1f)] public float ignorePaddingXRatio;
    [SerializeField, Range(0,1f)] public float ignorePaddingYRatio;

    [SerializeField, Range(0,1f)] public float ignoreScreenPaddingXRatio;
    [SerializeField, Range(0,1f)] public float ignoreScreenPaddingYRatio;
}
