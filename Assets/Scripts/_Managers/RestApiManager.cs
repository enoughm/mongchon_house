using System;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public class RestApiManager
{
    private Dictionary<string, Stack<int>> _callDic = new Dictionary<string, Stack<int>>();
    private AskApi _sampleApi = new AskApi();
    private AskApi SampleApi { get { return _sampleApi; } }

    public bool working = false;
    
    //Call 예시
    public void CallSample()
    {
        Call("SomeKey", 
            SampleApi.Ask(AskType.ask, "a", "a").Then(res => Debug.Log(res.answer)), ()=>{Debug.Log("모든 콜 처리 완료");});
    }
    
    private void Call(string key, IPromise callObj, Action lastAction)
    {
        int count = 0;
        working = true;
        
        if (!_callDic.ContainsKey(key))
        {
            Stack<int> queue = new Stack<int>();
            queue.Push(count);
            _callDic.Add(key, queue);
        }
        else
        {
            int value = _callDic[key].Peek();
            value++;
            _callDic[key].Push(value);
            count = value;
        }
        
        Debug.Log($"<color=yellow>EnqueueToWaitAllResponse key:{key} / count:{count}</color>");
        callObj.Catch(ErrorCatch).Finally(()=>LastResponse(key, lastAction));
    }

    private void LastResponse(string key, Action lastAction)
    {
        if (_callDic.ContainsKey(key))
        {
            _callDic[key].Pop();
            Debug.Log($"<color=yellow>CheckLastResponse key:{key} / count:{_callDic[key].Count}</color>");
            if (_callDic[key].Count == 0)
            {
                _callDic.Remove(key);
                Debug.Log("LastCallResponse");
                lastAction?.Invoke();
                working = false;
            }
        }
    }
    
    private void ErrorCatch(Exception obj)
    {
        Debug.LogError(obj.Message);
    }
    
    
    //파일 업로드 후 결과 전송
    // var var = KioskNetworkManager.WebApi.ApiFile
    //     .Add(kind, bytes, new FileUpload(SystemInfo.deviceUniqueIdentifier + DateTime.Now, kind, "eyedr_result_graph", "eyedr"))
    //     .Then(res =>
    //     {
    //         string imageUri = res.file.path;
    //         string questionJson = KioskGameManager.Instance.results.GetUserCheckJson();
    //         EyeDr_AddResult req = MakeAddResultRequest(measureType, imageUri, questionJson);
    //
    //         Debug.Log($"[Request(AddResult)]:\n{JsonUtility.ToJson(req, true)}");
    //         return KioskNetworkManager.WebApi.ApiVror.EyeDr_AddResult(req);
    //     })
    //     .Then(res =>
    //     {
    //         Debug.Log($"결과 전송 성공\n{res}");
    //     })
    //     .Catch(exception =>
    //     {
    //         Debug.Log($"결과 전송 실패\n{exception.Message}");
    //     });
}
