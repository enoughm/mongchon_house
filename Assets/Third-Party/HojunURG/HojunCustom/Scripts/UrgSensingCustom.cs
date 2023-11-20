using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class UrgSensingCustom : MonoBehaviour
{
    [Header("sensing params (meter)")]
    public Vector2 sensingAreaSize = new Vector2(1, 1);
    public Vector2 sensingAreaOffset = new Vector2(1, 1);
    
    //실제로 감지되는 영역
    public Vector2 actuallySensingAreaSize = new Vector2(1, 1);
    public Vector2 actuallySensingAreaOffset = new Vector2(1, 1);

    
    [Header("sensing params")]
    [Range(0.001f, 2.0f)]
    public float objThreshold = 0.5f;
    public float minWidth = 0.01f;

    private Bounds _sensingArea;
    private Bounds _sensingScreenArea;
    
    private Rect _sensingAreaRect;
    private Rect _sensingAreaScreenRect;

    [Header("sensing result")]
    public List<SensedObject> allSensedObjs;
    public List<ConvertedSensedObject> convertedAllSensedObjsInArea;
    public List<ConvertedSensedObject> convertedAllSensedObjsInRealArea;
    
    
    
    public List<SensedObject> sensedObjsInArea;
    public List<ConvertedSensedObject> convertedSensedObjsInArea;
    public List<ConvertedSensedObject> convertedSensedObjsInRealArea;
    public Material mat;
    
    [Header("Custom")]
    public bool xFlip = false;
    public bool zFlip = false;
    public bool viewPortReverse = false;


    object lockObj;

    UrgDeviceEthernetCustom urg;
    UrgControlCustom urgControl;
    private int recvCnt = 0;

    Mesh sensedObjMesh
    {
        get
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.vertices = Enumerable.Repeat(Vector3.zero, 5).ToArray();
                _mesh.SetIndices(
                    new[] {
                        0, 1, 4,
                        4, 1, 3,
                        3, 1, 2
                    },MeshTopology.Triangles, 0);
                _mesh.MarkDynamic();
            }
            return _mesh;
        }
    }
    Mesh _mesh;
    ComputeBuffer verticesBuffer;
    List<Vector3> verticesData;

    private void Start()
    {
        urgControl = GetComponent<UrgControlCustom>();
        urg = GetComponent<UrgDeviceEthernetCustom>();
        urg.onReadMD += OnReadMD;
        urg.onReadME += OnReadME;

        sensedObjsInArea = new List<SensedObject>();
        convertedSensedObjsInArea = new List<ConvertedSensedObject>();
        
        allSensedObjs = new List<SensedObject>();
        convertedAllSensedObjsInArea = new List<ConvertedSensedObject>();

        lockObj = new object();
        verticesBuffer = new ComputeBuffer(1080, sizeof(float) * 3);
        verticesData = new List<Vector3>();
    }

    private void Update()
    {
        _sensingArea.extents = new Vector3(sensingAreaSize.x * 0.5f, 0, sensingAreaSize.y * 0.5f);
        _sensingArea.center = new Vector3(sensingAreaOffset.x, 0, _sensingArea.extents.z + sensingAreaOffset.y);
        
        _sensingScreenArea.extents = new Vector3(actuallySensingAreaSize.x * 0.5f, 0, actuallySensingAreaSize.y * 0.5f);
        _sensingScreenArea.center = new Vector3(actuallySensingAreaOffset.x, 0, _sensingScreenArea.extents.z + actuallySensingAreaOffset.y);
        
        
        int zFlipValue = zFlip ? -1 : 1;
        int xFlipValue = xFlip ? -1 : 1;
        _sensingAreaRect = new Rect((_sensingArea.center.x - _sensingArea.extents.x) * xFlipValue, (_sensingArea.center.z - _sensingArea.extents.z) * zFlipValue, _sensingArea.size.x  * xFlipValue, _sensingArea.size.z * zFlipValue);
        _sensingAreaScreenRect = new Rect((_sensingScreenArea.center.x - _sensingScreenArea.extents.x) * xFlipValue, (_sensingScreenArea.center.z - _sensingScreenArea.extents.z) * zFlipValue, _sensingScreenArea.size.x  * xFlipValue, _sensingScreenArea.size.z * zFlipValue);
      
        //DrawMesh();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_sensingArea.center, _sensingArea.size);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_sensingScreenArea.center, _sensingScreenArea.size);
        
        Gizmos.color = Color.green;
        if (lockObj != null)
            lock (lockObj)
                for (var i = 0; i < sensedObjsInArea.Count; i++)
                {
                    var so = sensedObjsInArea[i];
                    Gizmos.DrawLine(so.p0, so.center);
                    Gizmos.DrawLine(so.center, so.p1);
                }
    }

    private void OnDestroy()
    {
        urg.onReadMD -= OnReadMD;
        urg.onReadME -= OnReadME;
        if (verticesBuffer != null)
            verticesBuffer.Release();
    }

    // void DrawMesh()
    // {
    //     lock (lockObj)
    //     {
    //         verticesData.Clear();
    //         for (var i = 0; i < sensedObjs.Count; i++)
    //             verticesData.AddRange(sensedObjs[i].vertices);
    //         verticesBuffer.SetData(verticesData);
    //         mat.SetInt("_VCount", sensedObjMesh.vertexCount);
    //         mat.SetBuffer("_VBuffer", verticesBuffer);
    //         var matrices = Enumerable.Repeat(transform.localToWorldMatrix, sensedObjs.Count).ToList();
    //         Graphics.DrawMeshInstanced(sensedObjMesh, 0, mat, matrices);
    //     }
    //
    // }

    void GetPointFromDistance(int step, float distance, ref Vector3 pos)
    {
        var angle = step * urgControl.angleDelta - urgControl.angleOffset + 90f;
        pos.x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
        pos.z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;
    }
    
    void OnReadMD(List<long> distances)
    {
        if (distances == null || distances.Count < 1)
            return;

        Vector3 prevP = Vector3.zero;
        Vector3 checkP = Vector3.zero;
        Vector3 currentP = Vector3.zero;
        Vector3 accum = Vector3.zero;
        int accumCount = 0;
        bool isObj = false;

        sensedObjsInArea.Clear();
        convertedSensedObjsInArea.Clear();
        
        allSensedObjs.Clear();
        convertedAllSensedObjsInArea.Clear();

        GetPointFromDistance(0, distances[0], ref prevP);
        
        for (var i = 0; i < distances.Count; i++)
        {
            var d = distances[i] * 0.001f;
            GetPointFromDistance(i, d, ref currentP);

            //all Sensed Objects
            {
                var obj = new SensedObject() { p0 = currentP, p1 = prevP, center = currentP / 1 };
                allSensedObjs.Add(obj);
                if(_sensingArea.Contains(currentP))
                    convertedAllSensedObjsInArea.Add(GetConvertedSensedObject(ref _sensingAreaRect, obj));
            }

            if (isObj)
            {
                if (objThreshold * objThreshold < (currentP - prevP).sqrMagnitude && _sensingArea.Contains(currentP))//new obj
                {
                    if (minWidth * minWidth < (prevP - checkP).sqrMagnitude)
                    {
                        var obj = new SensedObject() { p0 = checkP, p1 = prevP, center = accum / accumCount };
                        sensedObjsInArea.Add(obj);
                        convertedSensedObjsInArea.Add(GetConvertedSensedObject(ref _sensingAreaRect, obj));
                    }
                    checkP = currentP;
                    accum = currentP;
                    isObj = true;
                    accumCount = 1;
                }
                else if (!_sensingArea.Contains(currentP)) //lost obj
                {
                    if (minWidth * minWidth < (prevP - checkP).sqrMagnitude)
                    {
                        var obj = new SensedObject() { p0 = checkP, p1 = prevP, center = accum / accumCount };
                        sensedObjsInArea.Add(obj);
                        convertedSensedObjsInArea.Add(GetConvertedSensedObject(ref _sensingAreaRect, obj));
                    }
            
                    isObj = false;
                    accumCount = 0;
                }
                else//continue obj
                {
                    accum += currentP;
                    accumCount++;
                }
            }
            else
            {
                if (objThreshold * objThreshold < (currentP - prevP).sqrMagnitude && _sensingArea.Contains(currentP))//new obj
                {
                    checkP = currentP;
                    accum = currentP;
                    isObj = true;
                    accumCount = 1;
                }
            }
            prevP = currentP;
        }
        
        convertedSensedObjsInRealArea.Clear();
        convertedSensedObjsInRealArea = ExtractSensedDataOnlyInRealArea(convertedSensedObjsInArea);
        
        convertedAllSensedObjsInRealArea.Clear();
        convertedAllSensedObjsInRealArea = ExtractSensedDataOnlyInRealArea(convertedAllSensedObjsInArea);
    }

    private ConvertedSensedObject GetConvertedSensedObject(ref Rect rect, SensedObject sensedObject)
    {
        var obj = sensedObject;
        obj.p0.x = xFlip ? -obj.p0.x : obj.p0.x;
        obj.p0.z = zFlip ? -obj.p0.z : obj.p0.z;
        obj.p1.x = xFlip ? -obj.p1.x : obj.p1.x;
        obj.p1.z = zFlip ? -obj.p1.z : obj.p1.z;
        obj.center.x = xFlip ? -obj.center.x : obj.center.x;
        obj.center.z = zFlip ? -obj.center.z : obj.center.z;
        
        Vector2 viewPortPos = GetSensingAreaViewPortValue(rect, obj);
        if (viewPortReverse)
        {
            viewPortPos = new Vector2(1 - viewPortPos.x, 1 - viewPortPos.y);
        }
        ConvertedSensedObject convertedSensedObject = new ConvertedSensedObject()
        {
            origin = sensedObject,
            converted = obj,
            viewPortPos = viewPortPos,
            
        };
        return convertedSensedObject;
    }

    private List<ConvertedSensedObject> ExtractSensedDataOnlyInRealArea(List<ConvertedSensedObject> convertedSensedObjects)
    {
        List<ConvertedSensedObject> sensedObjectsInRealArea = new List<ConvertedSensedObject>();

        for (int i = 0; i < convertedSensedObjects.Count; i++)
            if (_sensingScreenArea.Contains(convertedSensedObjects[i].origin.center))
                sensedObjectsInRealArea.Add(GetConvertedSensedObject(ref _sensingAreaScreenRect, convertedSensedObjects[i].origin));

        return sensedObjectsInRealArea;
    }
    
    Vector2 GetSensingAreaViewPortValue(Rect rect, SensedObject obj)
    {
        var viewPortX = GetPercentage(rect.xMin, rect.xMax, obj.center.x);
        var viewPortY = GetPercentage(rect.yMin, rect.yMax, obj.center.z);
        return new Vector2(viewPortX, viewPortY);
    }
    
    float GetPercentage(float a, float b, float c)
    {
        // a와 b의 범위를 구합니다.
        float range = b - a;

        // c가 a와 b 사이에 위치한 비율을 계산합니다.
        float percentage = (c - a) / range;

        // 결과를 퍼센트로 변환합니다.
        float percentValue = percentage;

        return percentValue;
    }
    
    void OnReadME(List<long> distances, List<long> strengths)
    {
        OnReadMD(distances);
    }

    [System.Serializable]
    public struct SensedObject
    {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 center;

        public Vector3[] vertices
        {
            get
            {
                if (_vs == null)
                    _vs = new Vector3[5];
                var width = (p1 - p0).magnitude;
                _vs[0] = p0;
                _vs[1] = center;
                _vs[2] = p1;
                _vs[3] = p1 + center.normalized * width * 0.5f;
                _vs[4] = p0 + center.normalized * width * 0.5f;
                return _vs;
            }
        }
        
        Vector3[] _vs;
    }

    [System.Serializable]
    public struct ConvertedSensedObject
    {
        //후쿠요센서에서 바라보는 기준으로 된 원본 위치 값이다
        public SensedObject origin;
        //후쿠요센서를 바라보는 기준으로 된 위치 값이다
        public SensedObject converted;
        //Gameview 기준으로 뷰포트 값이다
        //후쿠요센서를 바라보는 기준으로 좌하단 0,0에서 우상단 1,1 이다
        public Vector2 viewPortPos;
    }
}
