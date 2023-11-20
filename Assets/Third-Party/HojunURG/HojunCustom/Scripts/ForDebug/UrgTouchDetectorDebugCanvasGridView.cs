using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvasGridView : MonoBehaviour
{
    [SerializeField] private UrgTouchDetectorDebugCanvasGridItem _gridItemPrefab;

    private GridLayoutGroup _gridLayoutGroup;
    private UrgTouchDetectorDebugCanvasGridItem[,] _touchGridItems;
    private Vector2 setGridVector2;
    
    void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    public void CreateGridLayout(Vector2 cellSize, Vector2 touchGrids)
    {
        Debug.Log(touchGrids);
        setGridVector2 = touchGrids;
        _gridLayoutGroup.cellSize = cellSize;
        _gridLayoutGroup.constraintCount = (int)touchGrids.x;
        _touchGridItems = new UrgTouchDetectorDebugCanvasGridItem[(int)touchGrids.x,(int)touchGrids.y];
        
        //instatiate grid items
        for (int x = 0; x < (int)touchGrids.x; x++)
        {
            for (int y = 0; y < (int)touchGrids.y; y++)
            {
                _touchGridItems[x,y] = Instantiate(_gridItemPrefab, _gridLayoutGroup.transform);
            }
        }
    }
    
    public void SetData(UrgGridData[,] data)
    {
        if (_gridLayoutGroup.gameObject.activeSelf)
        {
            for (int x = 0; x < setGridVector2.x; x++)
            {
                for (int y = 0; y < setGridVector2.y; y++)
                {
                    _touchGridItems[x, y].SetData(data[x,y]);
                }
            }
        }
    }
}
