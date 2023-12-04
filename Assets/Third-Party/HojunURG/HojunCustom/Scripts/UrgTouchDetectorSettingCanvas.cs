using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorSettingCanvas : MonoBehaviour
{
    [SerializeField] private UrgTouchDetector urgTouchDetector;
    
    [SerializeField] private Button debugOnOffButton;
    [SerializeField] private Button settingOnOffButton;
    [SerializeField] private Button ignoreOnOffButton;
    
    [SerializeField] private GameObject debugRoot;
    [SerializeField] private GameObject settingRoot;
    [SerializeField] private GameObject ignoreRoot;
    
    [SerializeField] private InputField hokuyoIP;
    [SerializeField] private InputField screenAreaSizeX;
    [SerializeField] private InputField screenAreaSizeY;
    [SerializeField] private InputField screenAreaOffsetX;
    [SerializeField] private InputField screenAreaOffsetY;
    [SerializeField] private InputField realAreaSizeX;
    [SerializeField] private InputField realAreaSizeY;
    [SerializeField] private InputField realAreaOffsetX;
    [SerializeField] private InputField realAreaOffsetY;
    [SerializeField] private InputField objThreshold;
    [SerializeField] private InputField minWidth;
    [SerializeField] private InputField xOffsetFromSensedPoint;
    [SerializeField] private InputField yOffsetFromSensedPoint;
    
    [SerializeField] private Button curButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button reconnectButton;

    [SerializeField] private UrgTouchDetectorSettingGridView gridView;

    Canvas _canvas;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        applyButton.onClick.AddListener(Apply);
        curButton.onClick.AddListener(CurValue);
        cancelButton.onClick.AddListener(Cancel);
        reconnectButton.onClick.AddListener(Reconnect);
        debugOnOffButton.onClick.AddListener(() =>
        {
            debugRoot.gameObject.SetActive(!debugRoot.gameObject.activeSelf);
        });
        settingOnOffButton.onClick.AddListener(() =>
        {
            settingRoot.gameObject.SetActive(!settingRoot.gameObject.activeSelf);
        });
        ignoreOnOffButton.onClick.AddListener(() =>
        {
            ignoreRoot.gameObject.SetActive(!ignoreRoot.gameObject.activeSelf);
        });
    }

    private void Start()
    {
        _canvas.targetDisplay = urgTouchDetector.TargetDisplay;
        gridView.CreateGridLayout(urgTouchDetector.TouchGridCellSize, urgTouchDetector.TouchGrids);
        gridView.OnClickGrid += OnClickGrid;
        CurValue();
    }

    private void OnClickGrid(Vector2 arg1, bool arg2)
    {
        urgTouchDetector.SetGridDataSetting(arg1, arg2);
    }
    
    private void Reconnect()
    {
        urgTouchDetector.UrgControl.StartTcp(urgTouchDetector.HokuyoIP);
    }

    private void CurValue()
    {
        hokuyoIP.text = urgTouchDetector.HokuyoIP;
        screenAreaSizeX.text = urgTouchDetector.UrgSensing.sensingAreaSize.x.ToString();
        screenAreaSizeY.text = urgTouchDetector.UrgSensing.sensingAreaSize.y.ToString();
        screenAreaOffsetX.text = urgTouchDetector.UrgSensing.sensingAreaOffset.x.ToString();
        screenAreaOffsetY.text = urgTouchDetector.UrgSensing.sensingAreaOffset.y.ToString();
        realAreaSizeX.text = urgTouchDetector.UrgSensing.actuallySensingAreaSize.x.ToString();
        realAreaSizeY.text = urgTouchDetector.UrgSensing.actuallySensingAreaSize.y.ToString();
        realAreaOffsetX.text = urgTouchDetector.UrgSensing.actuallySensingAreaOffset.x.ToString();
        realAreaOffsetY.text = urgTouchDetector.UrgSensing.actuallySensingAreaOffset.y.ToString();
        objThreshold.text = urgTouchDetector.UrgSensing.objThreshold.ToString();
        minWidth.text = urgTouchDetector.UrgSensing.minWidth.ToString();
        xOffsetFromSensedPoint.text = urgTouchDetector.UrgSensing.xOffsetFromDetectPos.ToString();
        yOffsetFromSensedPoint.text = urgTouchDetector.UrgSensing.yOffsetFromDetectPos.ToString();

        gridView.Load(urgTouchDetector.UrgGridDataSettingArray);
    }

    private void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    private void Apply()
    {
        try
        {
            Debug.Log($"{gameObject.name}, Apply, {hokuyoIP.text}");
            urgTouchDetector.HokuyoIP = hokuyoIP.text;
            urgTouchDetector.UrgSensing.sensingAreaSize = new Vector2(float.Parse(screenAreaSizeX.text), float.Parse(screenAreaSizeY.text));
            urgTouchDetector.UrgSensing.sensingAreaOffset = new Vector2(float.Parse(screenAreaOffsetX.text), float.Parse(screenAreaOffsetY.text));
            urgTouchDetector.UrgSensing.actuallySensingAreaSize = new Vector2(float.Parse(realAreaSizeX.text), float.Parse(realAreaSizeY.text));
            urgTouchDetector.UrgSensing.actuallySensingAreaOffset = new Vector2(float.Parse(realAreaOffsetX.text), float.Parse(realAreaOffsetY.text));
            urgTouchDetector.UrgSensing.minWidth = float.Parse(minWidth.text);
            urgTouchDetector.UrgSensing.objThreshold = float.Parse(objThreshold.text);
            urgTouchDetector.UrgSensing.xOffsetFromDetectPos = float.Parse(xOffsetFromSensedPoint.text);
            urgTouchDetector.UrgSensing.yOffsetFromDetectPos = float.Parse(yOffsetFromSensedPoint.text);

            urgTouchDetector.DataSave();
        }
        catch (Exception e)
        {
            
        }
    }
}
