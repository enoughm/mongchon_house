using Model;
using Proyecto26;
using RSG;

public class AskApi
{
    private const string RootURL = "https://ldk.vror.kr/ask";
    private const string TestURL = "https://ldk.vror.kr/ask";

    private string Url
    {
        get
        {
            return RootURL;
        }
    }
    
    //Get Sample
    public IPromise<Case> Ask(AskType type = AskType.ask, string text = "a", string old = "a")
    {
        text = string.IsNullOrEmpty(text) ? "a" : text;
        old = string.IsNullOrEmpty(old) ? "a" : old;

        return RestClient.Get<Case>(RequestCreator.RequestNobody(Url + $"/{type.ToString()}/{text}/{old}"));
    }

    // File Upload Sample
    // public IPromise<FileURL> Add(string kind, byte[] file, FileUpload req)
    // {
    //     return RestClient.Post<FileURL>(RequestCreator.RequestFileUpload(Url+"/file/add", kind, file, req));
    // }

    // Post Sample
    // public IPromise<Days> EyeDr_GetStatusResultDays(string jwt )
    // {
    //     return RestClient.Post<Days>(RequestCreator.RequestWithJwt(Url + "/eyedr/getStatusResultDays", jwt));
    // }
}