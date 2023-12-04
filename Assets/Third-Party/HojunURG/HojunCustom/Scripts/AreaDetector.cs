using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaDetector : MonoBehaviour
{
    public struct JumpData
    {
        public float changeValueAverage;
        public float changeValueMax;
        public float changeValueMin;
        public float sensedCountAverage;
        public float sensedCountMax;
        public float sensedCountMin;
    }

    public RectTransform RectTr
    {
        get
        {
            if(_rt == null)
                _rt = GetComponent<RectTransform>();
            return _rt;
        }
    }

    [Header("DEBUG")] 
    public bool useDebugLog = false;
    public bool useIsSomethingDebugKey = false;
    public KeyCode isSomethingDebugKey = KeyCode.Keypad1;
    public bool useJumpDebugKey = false;
    public KeyCode jumpDebugKey = KeyCode.Alpha1;
    
    [Header("INTERFACE")]
    [SerializeField] private bool isSomething = false;
    public bool IsSomething => isSomething;
    public Action OnJumped;
    
    [Header("IS_SOMETHING OPTION")]
    [SerializeField] float isSomethingCheckDurationThreshold = 1;
    [SerializeField] float isSomethingSumThreshold = 10;
    [SerializeField] float isSomethingDisappearTimeThreshold = 1f;

    [Header("JUMP OPTION")] [SerializeField]
    private bool checkOnlyDown = false;
    [SerializeField] float jumpVelocityMaxThreshold = 15; 
    [SerializeField] float jumpVelocityThreshold = 5f; //값의 변화량, 범위마다 다르기 때문에 직접 테스트 후 적당한 값을 맞춰줘야함
    
    private RectTransform _rt;
    private UrgTouchDetector _touchDetector;
    private string _seatName = "one";

    private float _prevAverageSum;
    private bool _isUp, _prevIsUp = false;
    private bool _isDown, _prevIsDown = false;

    private Queue<float> _upVelocityQueue = new Queue<float>();
    private Queue<float> _upAverageCountQueue = new Queue<float>();
    
    private Queue<float> _downVelocityQueue = new Queue<float>();
    private Queue<float> _downAverageCountQueue = new Queue<float>();
    
    
    //썸띵체크
    private float _isSomethingTime = 0;
    private float _isSomethingDisappearTime = 0;

    private JumpData lastUpJumpData;
    private JumpData lastDownJumpData;

    private bool _set = false;

    private void Awake()
    {
    }
    
    public void SetData(string seatName, UrgTouchDetector urgTouchDetector)
    {
        _seatName = seatName;
        _touchDetector = urgTouchDetector;
        _touchDetector.RectObserveAction += OnRectObserved;
        _set = true;
    }

    private void Update()
    {
        if (!_set)
            return;
        
        if (useJumpDebugKey && Input.GetKeyDown(jumpDebugKey))
        {
            OnJumped?.Invoke();
        }
        
        if (useIsSomethingDebugKey )
        {
            if (Input.GetKeyDown(isSomethingDebugKey))
            {
                isSomething = !isSomething;
            }
        }
        else
        {
            UpdateCheckIsSomething();
        }
    }

    private void UpdateCheckIsSomething()
    {
        //영역에 무언가 있는지 체크
        if (_prevAverageSum > isSomethingSumThreshold)
        {
            _isSomethingTime += Time.deltaTime;
            _isSomethingDisappearTime = 0;
            
            if (_isSomethingTime > isSomethingCheckDurationThreshold)
            {
                isSomething = true;
            }
        }
        else
        {
            _isSomethingDisappearTime += Time.deltaTime;
            if (_isSomethingDisappearTime > isSomethingDisappearTimeThreshold)
            {
                _isSomethingTime = 0;
                isSomething = false;
            }
        }
    }
    
    
    private void OnRectObserved(string arg1, UrgGridObserverData arg2)
    {
        if (arg1 != _seatName)
        {
            return;
        }
        
        // 변화율(%) = [(나중 값 – 처음 값) / 처음 값] × 100
        //float changePercentage = (arg2.averageSum - _prevAverageSum) / arg2.averageSum * 100;
        float velocity = arg2.averageSum - _prevAverageSum;
        //bool change = arg2.averageSum > _prevAverageSum

        //Debug.Log($"arg2.averageSum: {arg2.averageSum} / prevAverageSum: {_prevAverageSum} /  velocity: {velocity}");
        if (velocity > float.MaxValue)
            velocity = 0;
        
        //curVelocity.Value = velocity;


        //현재값이 + 이면 발을 내렸다는 것이므로 _isDown = true
        if (velocity > 0)
        {
            _isUp = false;
            _isDown = true;
        }
        else if (velocity < -0)
        {
            _isUp = true;
            _isDown = false;
        }
        else
        {
            _isDown = false;
            _isUp = false;
        }
        
        
        //결과 판단
        if (_prevIsUp && !_isUp)
        {
            if (_upVelocityQueue.Count <= 0 )
            {
                Log("<color=red>[상승종료, 무시] 충분한 데이터가 없음</color>");
            }
            else{
                lastUpJumpData = new JumpData()
                {
                    changeValueAverage = _upVelocityQueue.Average(),
                    changeValueMax = _upVelocityQueue.Max(),
                    changeValueMin = _upVelocityQueue.Min(),
                    sensedCountAverage = _upAverageCountQueue.Average(),
                    sensedCountMax = _upAverageCountQueue.Max(),
                    sensedCountMin = _upAverageCountQueue.Min(),
                };
                Log($"<color=yellow>[상승종료] 상승평균변화량: {JsonUtility.ToJson(lastUpJumpData)}</color>");
            }
        }

        
        if (_prevIsDown && !_isDown)
        {
            if (_downVelocityQueue.Count  <= 0)
            {
                Log("<color=red>[하강종료, 무시] 충분한 데이터가 없음</color>");
            }
            else{
                lastDownJumpData = new JumpData()
                {
                    changeValueAverage = _downVelocityQueue.Average(),
                    changeValueMax = _downVelocityQueue.Max(),
                    changeValueMin = _downVelocityQueue.Min(),
                    sensedCountAverage = _downAverageCountQueue.Average(),
                    sensedCountMax = _downAverageCountQueue.Max(),
                    sensedCountMin = _downAverageCountQueue.Min(),
                };
                Log($"<color=green>[하강종료] 하강평균변화량: {JsonUtility.ToJson(lastDownJumpData)}</color>");
                Jumped();
            }
        }


        

        //상승 몇 개를  (시간 간격) 동안 N velocity로 통과했을 때
        //하강 몇 개가 (최소 N velocity)보타 높은 게 특정 시간 간격 동안 N velocity로 통과했을 때
        //상승 할 때
        if (_isUp)
        {
            if (!_prevIsUp)
            {
                _upVelocityQueue.Clear();
                _upAverageCountQueue.Clear();
                Log($"<color=yellow>[상승시작] velocity: {velocity})</color>");
            }
            
            _upVelocityQueue.Enqueue(Mathf.Abs(velocity));
            _upAverageCountQueue.Enqueue(arg2.averageSum);
            Log($"<color=yellow>[상승] velocity: {velocity} </color>");
        }
        
        
        //하강 할 때
        if(_isDown)
        {
            if (!_prevIsDown)
            {
                _downVelocityQueue.Clear();
                _downAverageCountQueue.Clear();
                Log($"<color=green>[하강시작] velocity: {velocity})</color>");
            }
            
            _downVelocityQueue.Enqueue(Mathf.Abs(velocity));
            _downAverageCountQueue.Enqueue(arg2.averageSum);
            Log($"<color=green>[하강] velocity: {velocity} </color>");
        }

        _prevAverageSum = arg2.averageSum;
        _prevIsUp = _isUp;
        _prevIsDown = _isDown;
    }


    private void Jumped()
    {
        if (!isSomething)
        {
            lastUpJumpData = new JumpData();
            lastDownJumpData = new JumpData();
            return;
        }

        bool success = false;
        if (checkOnlyDown)
        {
            success = lastDownJumpData.changeValueAverage > jumpVelocityThreshold
                      && lastDownJumpData.changeValueMax > jumpVelocityMaxThreshold;
        }
        else{
            success = lastUpJumpData.changeValueAverage > jumpVelocityThreshold 
                       && lastDownJumpData.changeValueAverage > jumpVelocityThreshold
                       && lastUpJumpData.changeValueMax > jumpVelocityMaxThreshold
                       && lastDownJumpData.changeValueMax > jumpVelocityMaxThreshold;
            
        }
        
        
        if (success)
        {
            OnJumped?.Invoke();
            Log($"<color=white> [ JUMP @@@ 성공 @@@ ] 상승:${JsonUtility.ToJson(lastUpJumpData)}, 하강:${JsonUtility.ToJson(lastDownJumpData)}</color>", true);
        }
        else
        {
            Log($"<color=red> [ JUMP FAIL] 상승:${JsonUtility.ToJson(lastUpJumpData)}, 하강:${JsonUtility.ToJson(lastDownJumpData)}</color>");
        }

        lastUpJumpData = new JumpData();
        lastDownJumpData = new JumpData();
    }


    private void Log(string str, bool force = false)
    {
        if (!useDebugLog && !force)
            return;
        
        Debug.Log(str);
    }
}
