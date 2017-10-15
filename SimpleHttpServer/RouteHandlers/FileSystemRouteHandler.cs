
using System.Net;
using System.Net.Security;
using Server.Models;
using System.Security.Cryptography.X509Certificates;
using logger;

namespace Server.RouteHandlers
{
    public class FileSystemRouteHandler
    {

        public int GetCnt = 0;
        public int PostCnt = 0;

        public FileSystemRouteHandler()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        public HttpResponse HandleGetResponse(HttpRequest request)
        {
            return null;
        }
        public HttpResponse HandlePostResponse(HttpRequest request)
        {
            return null;
        }

        //private static HttpResponse Handle_LocalFile(string localPath) {        
        //    var fileExtension = Path.GetExtension(localPath);

        //    return new HttpResponse
        //    {
        //        StatusCode = "200",
        //        ReasonPhrase = "Ok",
        //        Headers = {["Content-Type"] = QuickMimeTypeMapper.GetMimeType(fileExtension)},
        //        Content = File.ReadAllBytes(localPath)
        //    };
        //}

        //protected virtual HttpResponse Handle_LocalDir(HttpRequest request, string localPath) {
        //    var output = new StringBuilder();
        //    output.Append($"<h1> Directory: {request.Url} </h1>");

        //    foreach (var entry in Directory.GetFiles(localPath)) {                
        //        var fileInfo = new FileInfo(entry);

        //        var filename = fileInfo.Name;
        //        output.AppendFormat("<a href=\"{0}\">{0}</a> <br>", filename);                
        //    }

        //    return new HttpResponse
        //    {
        //        StatusCode = "200",
        //        ReasonPhrase = "Ok",
        //        ContentAsUtf8 = output.ToString()
        //    };
        //}






        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            if (policyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Logger.Log("Certificate error: {0}", policyErrors);
            return false;
        }
    }

    // HTTP requires that resposnes contain the proper MIME type. This quick mapping list below
    // contains many more mimetypes than System.Web.MimeMapping

    // http://stackoverflow.com/questions/1029740/get-mime-type-from-filename-extension
}
