using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;

public class UrgTouchDetectorSettingGridViewItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action<Vector2, bool> OnClickGridItem;
    
    private Vector2 _gridPos;
    private Toggle _toggle;

    private bool _entered = false;
    private bool _changed = false;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void Update()
    {
        if (_entered)
        {
            if (!_changed && Input.GetMouseButton(0))
            {
                _toggle.isOn = !_toggle.isOn;
                _changed = true;
            }
        }
        else
        {
            _changed = false;
        }
    }

    public void SetData(Vector2 gridPos, bool active)
    {
        _gridPos = gridPos;
        _toggle.isOn = active;
    }

    private void OnToggleValueChanged(bool arg0)
    {
        OnClickGridItem?.Invoke(_gridPos, arg0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _entered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _entered = false;
    }
}
