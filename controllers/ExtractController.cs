using ExtractApkApi.Models;
using System.IO.Compression;
using System.Text.RegularExpressions;
using ExtractApkApi.Utils;

namespace ExtractApkApi.Controllers;

class ExtractController
{
    public ResponseApi extractFile(IFormFile file){
        ResponseApi response = new ResponseApi();

        //max file 5mb
        if(file.Length > 5 * 1024 * 1024){
            response.status = 400;
            response.message = "File too large";
            return response;
        }

        //check file mime type, only .apk
        if(file.ContentType != "application/vnd.android.package-archive"){
            response.status = 400;
            response.message = "Invalid file type";
            return response;
        }
        
        
        try
        {
            string resultString = "";
            byte[] xmlByte = new byte[50 * 1024];
            using (var streamFile = file.OpenReadStream())
            using (ZipArchive zip = new ZipArchive(streamFile,ZipArchiveMode.Read))
                foreach (ZipArchiveEntry entry in zip.Entries){
                    //if name is classes*.dex
                    if(entry.Name.Contains("classes") && entry.Name.Contains(".dex")){
                        //read the file into text
                        using (var streamText = entry.Open())
                        using (var reader = new StreamReader(streamText))
                        {
                            resultString = reader.ReadToEnd();
                        }
                    }

                    if (entry.Name == "AndroidManifest.xml")
                    {


                        using (var stream = entry.Open())
                        {
                            stream.Read(xmlByte, 0, xmlByte.Length);
                        }
                        
                    }
                        
                }

            

            //regex (http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])
            var regexLink = new Regex(@"(http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])");
            var matchesLink = regexLink.Matches(resultString);

        

            var resultLinks = new string[matchesLink.Count];

            for (int i = 0; i < matchesLink.Count; i++)
            {   
                resultLinks[i] = matchesLink[i].Value;
            }

            //regex base64
            var regexBase64 = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})$");
            var matchesBase64 = regexBase64.Matches(resultString);

            var resultBase64 = new string[matchesBase64.Count];
            
            for (int i = 0; i < matchesBase64.Count; i++)
            {
                resultBase64[i] = matchesBase64[i].Value;
            }
            ResponseData responseData = new ResponseData();
            responseData.links = resultLinks;
            responseData.base64 = resultBase64;

            AndroidDecompress decompress = new AndroidDecompress();
            responseData.xml = decompress.decompressXML(xmlByte);
            response.data = responseData;
            response.status = 200;
            response.message = "Success";
        }
        catch (System.Exception e)
        {
            
            response.status = 500;
            response.message = "Internal Server Error : "+e.Message;
        }

        

        return response;
    }
}