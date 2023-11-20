using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UrgTouchDetectorDebugGui : MonoBehaviour
{
    // [SerializeField] private UrgTouchDetector _urgTouchDetector;    
    //
    // private GUIStyle emptyStyle = null;
    // private GUIStyle touchDownStyle = null;
    // private GUIStyle touchPressStyle = null;
    // private GUIStyle touchPressUpStyle = null;
    // private GUIStyle touchCancelStartStyle = null;
    //
    // private GUIStyle cursorStyle = null;
    // private GUIStyle curStyle = null;
    //
    // public float alpha = 0.3f;
    //
    // private float clickEffectDuration = 0.3f;
    //
    // public List<RealTouchData> realTouchDatas = new List<RealTouchData>();
    // public float[] realTouchDatasTime = new float[2000];
    //
    //
    // private void OnGUI()
    // {
    //     InitStyles();
    //     
    //     if (Event.current.type == EventType.Repaint)
    //     {
    //         Vector2 touchGrid = _urgTouchDetector.TouchGrids;
    //         UrgTouchData[,] touchGridItems= _urgTouchDetector.TouchGridItems;
    //         Vector2 touchGridCellSize  = _urgTouchDetector.TouchGridCellSize;
    //         int screenWidth = _urgTouchDetector.ScreenWidth;
    //         int screenHeight = _urgTouchDetector.ScreenHeight;
    //
    //         for (int x = 0; x < touchGrid.x; x++)
    //         {
    //             for (int y = 0; y < touchGrid.y; y++)
    //             {
    //                 var item = touchGridItems[x, y];
    //                 var rect = new Rect( x * touchGridCellSize.x,  screenHeight - (y+1) * touchGridCellSize.y, touchGridCellSize.x, touchGridCellSize.y);
    //                 //Color color = Color.white;
    //                 switch (item.touchState)
    //                 {
    //                     case UrgTouchState.Empty:
    //                         curStyle = emptyStyle;
    //                         GUI.Box(rect, $"x:{x},y:{y}", curStyle);
    //                         break;
    //                     case UrgTouchState.TouchDown:
    //                         curStyle = touchDownStyle;
    //                         break;
    //                     case UrgTouchState.TouchPress:
    //                         curStyle = touchPressStyle;
    //                         break;
    //                     case UrgTouchState.TouchPressUp:
    //                         curStyle = touchPressUpStyle;
    //                         break;
    //                 }
    //                 //color.a = 1 - (item.EmptyTime / touchForceEmptyDuration);
    //             }
    //         }
    //         
    //         //REAL TOUCH DATA
    //         var touchDatas = _urgTouchDetector.AllScreenTouchList;
    //         foreach (var touchData in touchDatas)
    //         {
    //             switch (touchData.touchState)
    //             {
    //                 case UrgTouchState.TouchDown:
    //                     {
    //                         realTouchDatas.Add(touchData);
    //                     }
    //                     break;
    //                 case UrgTouchState.TouchPress:
    //                     {
    //                         float width = 30;
    //                         float height = 30;
    //                         float clickX = screenWidth * touchData.viewPortPos.x - width * 0.5f;
    //                         float clickY = screenHeight - screenHeight * touchData.viewPortPos.y - height * 0.5f;
    //                         var cursorRect = new Rect( clickX,  clickY, width, height);
    //                         GUI.Box(cursorRect, "", cursorStyle);
    //                     }
    //                     break;
    //                 case UrgTouchState.TouchClicked:
    //                     {
    //                         realTouchDatas.Add(touchData);
    //                     }
    //                     break;
    //                 case UrgTouchState.TouchPressUp:
    //                     {
    //                         realTouchDatas.Add(touchData);
    //                     }
    //                     break;
    //             }
    //         }
    //
    //         
    //         List<int> removeIdx = new List<int>();
    //         for (int i = 0; i < realTouchDatas.Count; i++)
    //         {
    //             realTouchDatasTime[i] += Time.deltaTime;
    //             var touchData = realTouchDatas[i];
    //             switch (touchData.touchState)
    //             {
    //                 case UrgTouchState.TouchDown:
    //                 {
    //                     Color color = Color.white;
    //                     GUI.backgroundColor = color;
    //                     float width = 80;
    //                     float height = 80;
    //                     float clickX = screenWidth * touchData.viewPortPos.x - width * 0.5f;
    //                     float clickY = screenHeight - screenHeight * touchData.viewPortPos.y - height * 0.5f;
    //                     var clickRect = new Rect( clickX,  clickY, width, height);
    //                     GUI.Box(clickRect, "", touchDownStyle);
    //                 }
    //                 break;
    //                 case UrgTouchState.TouchPressUp:
    //                 {
    //                     Color color = Color.white;
    //                     GUI.backgroundColor = color;
    //                     float width = 120;
    //                     float height = 120;
    //                     float clickX = screenWidth * touchData.viewPortPos.x - width * 0.5f;
    //                     float clickY = screenHeight - screenHeight * touchData.viewPortPos.y - height * 0.5f;
    //                     var clickRect = new Rect( clickX,  clickY, width, height);
    //                     GUI.Box(clickRect, "", touchPressUpStyle);
    //                 }
    //                 break;
    //                 case UrgTouchState.TouchClicked:
    //                 {
    //                     Color color = Color.white;
    //                     GUI.backgroundColor = color;
    //                     float width = 250;
    //                     float height = 250;
    //                     float clickX = screenWidth * touchData.viewPortPos.x - width * 0.5f;
    //                     float clickY = screenHeight - screenHeight * touchData.viewPortPos.y - height * 0.5f;
    //                     var clickRect = new Rect( clickX,  clickY, width, height);
    //                     GUI.Box(clickRect, "", touchCancelStartStyle);
    //                 }
    //                 break;
    //             }
    //             
    //             if( realTouchDatasTime[i] > clickEffectDuration )
    //             {
    //                 removeIdx.Add(i);
    //                 realTouchDatasTime[i] = 0;
    //             }
    //         }
    //         
    //         foreach (var idx1 in removeIdx)
    //         {
    //             if(idx1 < realTouchDatas.Count)
    //                 realTouchDatas.RemoveAt(idx1);
    //         }
    //     }
    // }
    //
    // private void InitStyles()
    // {
    //     
    //     Color color = Color.white;
    //     if( emptyStyle == null )
    //     {
    //         // color = Color.clear;
    //         // emptyStyle = new GUIStyle( GUI.skin.button );
    //         // emptyStyle.normal.background = MakeTex( 2, 2,color );
    //         
    //         color = Color.black;
    //         color.a = alpha;
    //         emptyStyle = new GUIStyle( GUI.skin.button );
    //         emptyStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    //     
    //     if( touchDownStyle == null )
    //     {
    //         color = Color.black;
    //         color.a = alpha;
    //         touchDownStyle = new GUIStyle( GUI.skin.button );
    //         touchDownStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    //     
    //     if( touchPressStyle == null )
    //     {
    //         color = Color.blue;
    //         color.a = alpha;
    //         touchPressStyle = new GUIStyle( GUI.skin.button );
    //         touchPressStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    //     
    //     if( touchPressUpStyle == null )
    //     {
    //         color = Color.red;
    //         color.a = alpha;
    //         touchPressUpStyle = new GUIStyle( GUI.skin.button );
    //         touchPressUpStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    //     
    //     if( touchCancelStartStyle == null )
    //     {
    //         color = Color.cyan;
    //         color.a = alpha;
    //         touchCancelStartStyle = new GUIStyle( GUI.skin.button );
    //         touchCancelStartStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    //     
    //     if( cursorStyle == null )
    //     {
    //         color = Color.magenta;
    //         color.a = alpha;
    //         cursorStyle = new GUIStyle( GUI.skin.button );
    //         cursorStyle.normal.background = MakeTex( 2, 2, color );
    //     }
    // }
    //
    // private Texture2D MakeTex( int width, int height, Color col )
    // {
    //     Color[] pix = new Color[width * height];
    //     for( int i = 0; i < pix.Length; ++i )
    //     {
    //         pix[ i ] = col;
    //     }
    //     Texture2D result = new Texture2D( width, height );
    //     result.SetPixels( pix );
    //     result.Apply();
    //     return result;
    // }
}
