using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorSettingGridView : MonoBehaviour
{
    public Action<Vector2, bool> OnClickGrid;
    
    [SerializeField] private UrgTouchDetectorSettingGridViewItem _gridItemPrefab;
    
    private GridLayoutGroup _gridLayoutGroup;
    private Vector2 setGridVector2;
    private UrgTouchDetectorSettingGridViewItem[,] _touchGridItems;

    public void CreateGridLayout(Vector2 cellSize, Vector2 touchGrids)
    {
        if(_gridLayoutGroup == null)
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        
        Debug.Log(touchGrids);
        setGridVector2 = touchGrids;
        _gridLayoutGroup.cellSize = cellSize;
        _gridLayoutGroup.constraintCount = (int)touchGrids.x;
        _touchGridItems = new UrgTouchDetectorSettingGridViewItem[(int)touchGrids.x,(int)touchGrids.y];
        
        //instatiate grid items
        for (int x = 0; x < (int)touchGrids.x; x++)
        {
            for (int y = 0; y < (int)touchGrids.y; y++)
            {
                _touchGridItems[x,y] = Instantiate(_gridItemPrefab, _gridLayoutGroup.transform);
                _touchGridItems[x,y].OnClickGridItem += OnClickGridItem;
            }
        }
    }

    public void Load(UrgGridDataSetting[,] urgGridDataSettingArray)
    {
        //instatiate grid items
        for (int x = 0; x < (int)setGridVector2.x; x++)
        {
            for (int y = 0; y < (int)setGridVector2.y; y++)
            {
                try
                {
                    Vector2 gridPos = new Vector2(x, y);
                    var item = urgGridDataSettingArray[x, y];
                    _touchGridItems[x,y].SetData(item.gridPos, item.isAvailableArea);
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    private void OnClickGridItem(Vector2 arg1, bool arg2)
    {
        OnClickGrid?.Invoke(arg1, arg2);
    }
}
