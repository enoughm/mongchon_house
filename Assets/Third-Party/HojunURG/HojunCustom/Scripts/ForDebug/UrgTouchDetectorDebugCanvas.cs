using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvas : MonoBehaviour
{
    [SerializeField] private UrgTouchDetector _urgTouchDetector;

    
    [SerializeField] private Transform _touchItemRoot;
    [SerializeField] private UrgTouchDetectorDebugCanvasTouchItem _touchItemPrefab;
    [SerializeField] private List<UrgTouchDetectorDebugCanvasTouchItem> _touchItemList;
    
    
    [SerializeField] private UrgTouchDetectorDebugCanvasGridView _drawGridDatas;
    [SerializeField] private UrgTouchDetectorDebugCanvasDrawLines _drawLines;
    [SerializeField] private UrgTouchDetectorDebugCanvasSensingAreas _drawArea;

    //private UrgTouchData[,] _touchGridDatas;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _canvas.targetDisplay = _urgTouchDetector.TargetDisplay;
        _drawGridDatas.CreateGridLayout(_urgTouchDetector.TouchGridCellSize, _urgTouchDetector.TouchGrids);
    }

    private void LateUpdate()
    {
        //DRAW HOKUYO LINE
        _drawLines.SetData(_urgTouchDetector.UrgDeviceEthernet.distances, _urgTouchDetector.UrgControl.angleDelta, _urgTouchDetector.UrgControl.angleOffset);
  
        //DRAW HOKUYO AREAS
        _drawArea.SetData(
            _urgTouchDetector.UrgSensing.sensingAreaSize,
            _urgTouchDetector.UrgSensing.actuallySensingAreaSize,
            _urgTouchDetector.UrgSensing.sensingAreaOffset,
            _urgTouchDetector.UrgSensing.actuallySensingAreaOffset
        );
        
        //DRAW GRID DATAS
        _drawGridDatas.SetData(_urgTouchDetector.UrgGridItems);
        
        
        //DRAW TOUCH DATAS
        var touchDatas = _urgTouchDetector.AllScreenTouchList;
        //DRAW TOUCH DATA
        if (_touchItemRoot.gameObject.activeSelf)
        {
            while (_touchItemList.Count < touchDatas.Count)
            {
                _touchItemList.Add(Instantiate(_touchItemPrefab, _touchItemRoot));
            }

            for (int i = 0; i < _touchItemList.Count; i++)
            {
                bool useCursor = i < touchDatas.Count;
                if (i < touchDatas.Count)
                {
                    _touchItemList[i].ActiveCursor(useCursor);
                    _touchItemList[i].SetData(touchDatas[i], new Vector2(_urgTouchDetector.ScreenWidth, _urgTouchDetector.ScreenHeight));
                }
                else
                {
                    _touchItemList[i].ActiveCursor(useCursor);
                }
            }
        }
        

    }
}
