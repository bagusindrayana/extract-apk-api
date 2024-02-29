namespace ExtractApkApi.Models;

class ResponseData {
    public string xml { get; set; }
    public string[] links { get; set; }
    public string[] base64 { get; set; }

    
}
class ResponseApi {
    public int status { get; set; }
    public string message { get; set; }
    public ResponseData? data { get; set; }
}