using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TouchOptions", menuName = "Scriptable Object/UrgTouchDetectorSettingData", order = int.MaxValue)]
public class UrgTouchDetectorSettingData : ScriptableObject
{
    //그리드 데이터 관련
    [Header("그리드데이터 세팅값")]
    [SerializeField] public Vector2 touchGrid = new Vector2(30, 15);
    /// <summary>
    /// 그리드당 감지된 데이터 개수 평균에서 무시하지 않을 최소 임계값 (EX> 3개 이상의 데이터만 사용)
    /// </summary>
    [SerializeField] public float gridAverageAcceptThreshold = 3;
    /// <summary>
    /// 그리드 영역에 사용가능한 데이터가 몇 개 이상이여야 카운트를 하는지
    /// </summary>
    [SerializeField] public int gridAreaCountCheckThreshold = 1;
    /// <summary>
    /// 그리드 항목 간 Average의 값이 유사하여 경합이 발생 할 때 값이 몇 이상 차이가 나야 경합으로 인정할 것인지
    /// </summary>
    [SerializeField, Range(0, 1)] public float competitionApproveThreshold = 0.2f;
    /// <summary>
    /// 그리드 항목에 들어갈 감지된 데이터의 평균을 몇개까지 모을 것인지
    /// 수가 높을 수록 프레임당 데이터가 많이 필요함으로 반응이 늦어지게 됨
    /// </summary>
    [SerializeField] public int gridAverageMaxCollectCount = 3;

    /// <summary>
    /// 
    /// </summary>
    [Header("터치관련 세팅값")]
    [SerializeField] public float touchCheckSpeed = 10;
    [SerializeField] public bool touchCheckInGridArea = false;
    [SerializeField] public float touchDownCheckDuration = 0.1f;
    [SerializeField] public float touchMergeDistanceX = 0.1f;
    [SerializeField] public float touchMergeDistanceY = 0.1f;
    [SerializeField] public float touchMergeDistanceXForMerge = 0.1f;
    [SerializeField] public float touchMergeDistanceYForMerge = 0.1f;
    
    [SerializeField] public float touchClickableDistance = 0.1f;
    [SerializeField] public float touchCheckingDuration = 0.1f;
    [SerializeField] public float touchInvokeDuration = 0.15f;
    
    
    /// <summary>
    /// 값 무시 관련 
    /// </summary>
    [Header("스크린 Edge 비율에서 값 무시하기 위한 비율")]
    [SerializeField, Range(0,1f)] public float ignoreScreenPaddingXRatio;
    [SerializeField, Range(0,1f)] public float ignoreScreenPaddingYRatio;
}
