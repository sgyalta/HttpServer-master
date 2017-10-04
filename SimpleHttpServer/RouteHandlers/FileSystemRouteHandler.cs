// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using SimpleHttpServer.Models;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleHttpServer.RouteHandlers
{
    public class FileSystemRouteHandler
    {

        public string BaseUri { get; set; }
        public bool ShowDirectories { get; set; }

        public HttpResponse Handle(HttpRequest request) {
            var urlPart = request.Path;

            // do some basic sanitization of the URL, attempting to make sure they can't read files outside the basepath
            // NOTE: this is probably not bulletproof/secure
            urlPart = urlPart.Replace("\\..\\", "/");
            urlPart = urlPart.Replace("/../", "/");
            urlPart = urlPart.Replace("//","/");
            urlPart = urlPart.Replace(@"\\",@"/");
            //urlPart = urlPart.Replace(":","");           
            //urlPart = urlPart.Replace("/",Path.DirectorySeparatorChar.ToString());
           
            // make sure the first part of the path is not 
            if (urlPart.Length > 0) {
                var firstChar = urlPart.ElementAt(0);
                if (firstChar == '/' || firstChar == '\\') {
                    urlPart = "." + urlPart;
                }
            }
            var localPath = Path.Combine(BaseUri, urlPart);
                
            if (ShowDirectories && Directory.Exists(localPath)) {
                // Console.WriteLine("FileSystemRouteHandler Dir {0}",local_path);
                return Handle_LocalDir(request, localPath);
            } else if (File.Exists(localPath)) {
                // Console.WriteLine("FileSystemRouteHandler File {0}", local_path);
                return Handle_LocalFile(localPath);
            } else {
                return new HttpResponse {
                    StatusCode = "404",
                    ReasonPhrase = $"Not Found ({localPath}) handler({request.Route.Name})",
                };
            }
        }

        private static HttpResponse Handle_LocalFile(string localPath) {        
            var fileExtension = Path.GetExtension(localPath);

            return new HttpResponse
            {
                StatusCode = "200",
                ReasonPhrase = "Ok",
                Headers = {["Content-Type"] = QuickMimeTypeMapper.GetMimeType(fileExtension)},
                Content = File.ReadAllBytes(localPath)
            };
        }

        protected virtual HttpResponse Handle_LocalDir(HttpRequest request, string localPath) {
            var output = new StringBuilder();
            output.Append($"<h1> Directory: {request.Url} </h1>");
                        
            foreach (var entry in Directory.GetFiles(localPath)) {                
                var fileInfo = new FileInfo(entry);

                var filename = fileInfo.Name;
                output.AppendFormat("<a href=\"{0}\">{0}</a> <br>", filename);                
            }

            return new HttpResponse
            {
                StatusCode = "200",
                ReasonPhrase = "Ok",
                ContentAsUtf8 = output.ToString()
            };
        }
    }

    // HTTP requires that resposnes contain the proper MIME type. This quick mapping list below
    // contains many more mimetypes than System.Web.MimeMapping

    // http://stackoverflow.com/questions/1029740/get-mime-type-from-filename-extension
}
