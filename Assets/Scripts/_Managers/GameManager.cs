using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera FloorCamera
    {
        get
        {
            if(_floorCamera == null)
                _floorCamera = GameObject.FindGameObjectWithTag("FloorCam").GetComponent<Camera>();
            return _floorCamera;
        }
    }

    public bool IsSomeone => _isSomeone;
    public int FloorTouchCount => _floorUrgTouchDetector.AllScreenTouchList.Count;
    public UrgTouchDetector FloorUrgTouchDetector => _floorUrgTouchDetector;
    public List<GameObject> StepList => _stepList;

    
    
    public float floorEmptyTimeMax = 10;
    public float floorInvokeTimeMax = 3;
    
    
    UrgTouchDetector _floorUrgTouchDetector;

    private bool _isSomeone = false;
    private float _floorInvokeTime = 0;
    private float _floorEmptyTime = 0;
    private Camera _floorCamera;
    private List<GameObject> _stepList = new List<GameObject>();

    
    
    private void Awake()
    {
        //find tag
        _floorUrgTouchDetector = GameObject.FindGameObjectWithTag("FloorUrgTouchDetector").GetComponent<UrgTouchDetector>();
    }

    private void Update()
    {
        FloorUpdate();
    }


    private void FloorUpdate()
    {
        if (FloorTouchCount > 0)
        {
            _floorInvokeTime += Time.deltaTime;
            if (_floorInvokeTime > floorInvokeTimeMax)
            {
                _floorEmptyTime = 0;
                _isSomeone = true;
            }
        }

        if (FloorTouchCount == 0)
        {
            _floorEmptyTime += Time.deltaTime;
            if (_floorEmptyTime > floorEmptyTimeMax)
            {
                _floorInvokeTime = 0;
                _isSomeone = false;
            }
        }



        if (IsSomeone)
        {
            var allTouch = Managers.Game.FloorUrgTouchDetector.AllScreenTouchList;
        
        
            while (_stepList.Count < allTouch.Count)
            {
                var step = Managers.Resource.Instantiate("Floor/Step");
                _stepList.Add(step);
            }

            if (_stepList.Count > allTouch.Count)
            {
                for (int i = 0; i < _stepList.Count; i++)
                {
                    if (i > allTouch.Count - 1)
                    {
                        Managers.Resource.Destroy(_stepList[i]);
                        _stepList.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < allTouch.Count; i++)
            {
                var pos = _stepList[i].transform.position;
                var getPos = (Vector2)Managers.Game.FloorCamera.ViewportToWorldPoint(allTouch[i].viewPortPos);
                _stepList[i].transform.position = new Vector3(getPos.x, getPos.y, pos.z);   
            }
        }
        else
        {
            for (int i = 0; i < _stepList.Count; i++)
            {
                Managers.Resource.Destroy(_stepList[i]);
                _stepList.RemoveAt(i);
            }
        }
    }
}
