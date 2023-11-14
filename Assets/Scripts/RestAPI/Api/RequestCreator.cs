using System.Collections.Generic;
using System.Text;
using Proyecto26;
using UnityEngine;

public static class RequestCreator
{
    private const bool enableDebug = true;
    
    public static RequestHelper RequestNobody(string uri)
    {
        var helper = new RequestHelper();
        helper.Uri = uri;
        helper.EnableDebug = enableDebug;
        
        ReqLog(helper);
        
        return helper;
    }
    
    public static RequestHelper RequestWithReq<T>(string uri, T req)
    {
        var helper = new RequestHelper();
        helper.Uri = uri;
        helper.Body = req;
        helper.EnableDebug = enableDebug;
        ReqLog(helper);
        
        return helper;
    }

    public static RequestHelper RequestWithJwtAndReq<T>(string uri, string jwt, T req)
    {
        var helper = new RequestHelper();
        helper.Uri = uri;
        helper.Headers = new Dictionary<string, string>{{"x-access-token", jwt}};
        helper.Body = req;
        helper.EnableDebug = enableDebug;
        
        ReqLog(helper);

        return helper;
    }
    
    public static RequestHelper RequestWithJwt(string uri, string jwt)
    {
        var helper = new RequestHelper();
        helper.Uri = uri;
        helper.Headers = new Dictionary<string, string>{{"x-access-token", jwt}};
        helper.EnableDebug = enableDebug;
        
        ReqLog(helper);

        return helper;
    }
    
    public static RequestHelper RequestFileUpload(string uri, string kind, byte[] file)
    {
        var upload = new RequestHelper();
        
        upload.Uri = uri;
        upload.Method = "POST";
        upload.Headers = new Dictionary<string, string>{{"kind", kind}};
        upload.EnableDebug = enableDebug;
        
        upload.FormData = new WWWForm();
        // upload.FormData.AddField("name", req.name);
        // upload.FormData.AddField("des", req.des);
        // upload.FormData.AddField("kind", req.kind);
        // upload.FormData.AddField("uploader", req.uploader);
        upload.FormData.AddBinaryData("file", file);
        
        ReqLog(upload);

        return upload;
    }

    public static RequestHelper RequestGet<T>(string uri, T req)
    {
        var get = new RequestHelper();
        get.Uri = uri;
        get.Method = "GET";
        get.Body = req;
        get.EnableDebug = enableDebug;
        
        ReqLog(get);
        
        return get;
    }

    //sample
    public static RequestHelper RequestBodyRaw(string uri)
    {
        string[] keys = new[] { "a", "b", "c", "d" };
        string json = $"{{\n" +
                      $"\"{keys[0]}\":\"{12}\",\n" +
                      $"\"{keys[0]}\":{12},\n"+
                      $"\"{keys[0]}\":{12},\n" +
                      $"\"{keys[0]}\":\"{13}\",\n" +
                      $"\"{keys[0]}\":{14}" +
                      $"}}";
        
        var helper = new RequestHelper();
        helper.Uri = uri;
        helper.Method = "POST";
        helper.BodyRaw = Encoding.UTF8.GetBytes(json);
        helper.Headers = new Dictionary<string, string>{{"Content-Type", "application/json; charset=UTF-8"}};
        helper.EnableDebug = enableDebug;
        ReqLog(helper);

        return helper;
    }

    private static void ReqLog(RequestHelper log)
    {
        string jwt = "";
        if (log.Headers.ContainsKey("x-access-token"))
        {
            jwt = log.Headers["x-access-token"];
        }
        Debug.Log($"[REQUEST] Uri: {log.Uri}({(log.Method)})\nBody: {JsonUtility.ToJson(log.Body)}\nJwt: {jwt}");
    }
}
